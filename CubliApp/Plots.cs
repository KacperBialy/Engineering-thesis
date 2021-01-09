using OxyPlot;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using LineSeries = OxyPlot.Series.LineSeries;

namespace CubliApp
{
    class Plots
    {
        private static StringBuilder data_received;
        static System.Timers.Timer _timer = new System.Timers.Timer();

        private Thread updateThread { get; set; }
        private List<PlotView> sensor_plots;
        private List<LineSeries[]> lineSeries_sensor = new List<LineSeries[]>();
        private double time;
        private string[] Titles = { "AccX", "AccY", "AccZ", "GyrX", "GyrY", "GyrZ", "Roll", "Pitch" };
        static bool[] AxisEnables = new bool[8] { true, true, true, true, true, true, true, true };

        public Plots(PlotView plt_sensor_1, PlotView plt_sensor_2, PlotView plt_sensor_3)
        {
            sensor_plots = new List<PlotView>() { plt_sensor_1, plt_sensor_2, plt_sensor_3 };

            for (int i = 0; i < sensor_plots.Count; i++)
            {
                lineSeries_sensor.Add(new LineSeries[8]);
            }

            _timer.Interval = 500;
            _timer.Elapsed += UpdatePlots;
            _timer.Enabled = true;

            time = 0;
        }
        public static void setData(StringBuilder data) { data_received = data; }
        public static void setAxisEnables(bool[] axisEnables) { AxisEnables = axisEnables; }
        public void CreatePlots()
        {
            for (int i = 0; i < sensor_plots.Count; i++)
            {
                sensor_plots[i].Model = CreateModel(lineSeries_sensor[i]);
                sensor_plots[i].Controller = GetCustomController();
                if (i == 0)
                    sensor_plots[i].Model = SetUpLegend(sensor_plots[0].Model);
                else
                    sensor_plots[i].Model.IsLegendVisible = false;
            }
            updateThread = new Thread(() => UpdatePlot(lineSeries_sensor));
            updateThread.Start();
        }
        public void UpdatePlot(List<LineSeries[]> lineSeries)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                if (data_received != null)
                {
                    if (data_received.Length > 0)
                    {
                        string[] data = data_received.ToString().Split('\n');
                        data_received = new StringBuilder();
                        List<List<List<float>>> group_data = GroupData(data);

                        for (int i = 0; i < group_data.Count; i++)
                        {
                            var a = group_data[i];
                            for (int j = 0; j < a.Count; j++)
                            {
                                var b = group_data[i][j];
                                for (int k = 0; k < b.Count; k++)
                                {
                                    var c = group_data[i][j][k];
                                    DataPoint newPoint = new DataPoint(time, c);
                                    if (AxisEnables[k])
                                        lineSeries[j][k].Points.Add(newPoint);
                                    else
                                        lineSeries[j][k].Points.Clear();

                                }
                            }
                            AutomaticSliding(ref lineSeries);
                            time += stopwatch.Elapsed.Milliseconds / 1000.0;
                            stopwatch.Restart();
                        }
                    }
                }
                Bluetooth.SetStatusOfPlot(true);
            }
        }
        private void UpdatePlots(object sender, EventArgs e)
        {
            sensor_plots.ForEach(x => x.InvalidatePlot(true));
        }
        private void AutomaticSliding(ref List<LineSeries[]> listLineSeries)
        {
            for (int j = 0; j < listLineSeries.Count; j++)
            {
                for (int k = 0; k < 8; k++)
                {
                    if (AxisEnables[k])
                    {
                        if ((listLineSeries[j][k].MaxX - listLineSeries[j][k].MinX) > 10)
                            listLineSeries[j][k].Points.RemoveAt(0);
                    }
                    else
                    {
                        if (listLineSeries[j][k].Points.Count > 0)
                        {
                            listLineSeries[j][k].Points.Clear();
                        }
                    }
                }
            }

        }

        private List<List<List<float>>> GroupData(string[] data)
        {
            List<List<List<float>>> group_data = new List<List<List<float>>>();

            for (int i = 0; i < data.Length - 1; i++)
            {
                List<float> help = new List<float>();
                List<List<float>> help_all = new List<List<float>>();

                string[] line_splited = data[i].Split(';');
                if (line_splited.Length % 24 != 0)
                    break;
                int line_splited_length = line_splited.Length;
                int group_length = line_splited_length / 3;

                for (int j = 0; j < line_splited_length; j++)
                {

                    if (j < group_length)
                    {
                        if (float.TryParse(line_splited[j], NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                        {
                            help.Add(value);
                        }
                    }
                    else if (j == group_length)
                    {
                        help_all.Add(help);
                        help = new List<float>();
                        if (float.TryParse(line_splited[j], NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                        {
                            help.Add(value);
                        }
                    }
                    else if (j >= group_length & j < group_length * 2)
                    {
                        if (float.TryParse(line_splited[j], NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                        {
                            help.Add(value);
                        }
                    }
                    else if (j == group_length * 2)
                    {
                        help_all.Add(help);
                        help = new List<float>();
                        if (float.TryParse(line_splited[j], NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                        {
                            help.Add(value);
                        }
                    }
                    else
                    {
                        if (float.TryParse(line_splited[j], NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                        {
                            help.Add(value);
                        }
                    }
                }
                help_all.Add(help);
                group_data.Add(help_all);
            }
            return group_data;
        }
        private PlotModel CreateModel(LineSeries[] lineSeries)
        {
            PlotModel model = new PlotModel();

            for (int i = 0; i < lineSeries.Length; i++)
            {
                lineSeries[i] = new LineSeries();
                lineSeries[i].Title = Titles[i];
                model.Series.Add(lineSeries[i]);
            }
            return model;
        }
        private PlotModel SetUpLegend(PlotModel plotModel)
        {
            plotModel.LegendOrientation = LegendOrientation.Horizontal;
            plotModel.LegendPlacement = LegendPlacement.Outside;
            plotModel.LegendPosition = LegendPosition.TopCenter;
            plotModel.LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
            plotModel.LegendBorder = OxyColors.Black;

            return plotModel;
        }
        private PlotController GetCustomController()
        {
            var myController = new PlotController();

            //  Customizing the bindings 
            myController.UnbindMouseDown(OxyMouseButton.Right);
            //Mouse

            myController.BindMouseDown(OxyMouseButton.Left, OxyPlot.PlotCommands.ZoomRectangle);

            myController.BindMouseDown(OxyMouseButton.Right, OxyPlot.PlotCommands.Track);

            //Keyboard

            myController.BindKeyDown(OxyKey.R, OxyPlot.PlotCommands.Reset);

            myController.BindKeyDown(OxyKey.W, OxyPlot.PlotCommands.PanDown);
            myController.BindKeyDown(OxyKey.A, OxyPlot.PlotCommands.PanRight);
            myController.BindKeyDown(OxyKey.S, OxyPlot.PlotCommands.PanUp);
            myController.BindKeyDown(OxyKey.D, OxyPlot.PlotCommands.PanLeft);

            return myController;
        }
    }
}
