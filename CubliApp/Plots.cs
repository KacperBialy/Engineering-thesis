using OxyPlot;
using OxyPlot.Wpf;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using LineSeries = OxyPlot.Series.LineSeries;

namespace CubliApp
{
    class Plots
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private Thread updateThread { get; set; }
        private static StringBuilder data_received;
        private List<PlotView> sensor_plots;
        private List<LineSeries[]> lineSeries_sensor;
        private double time;
        private string[] Titles = { "AccX", "AccY", "AccZ", "GyrX", "GyrY", "GyrZ", "Roll", "Pitch" };
        private bool status;
        public Plots(PlotView plt_sensor_1, PlotView plt_sensor_2, PlotView plt_sensor_3)
        {
            sensor_plots = new List<PlotView>() { plt_sensor_1, plt_sensor_2, plt_sensor_3 };
            lineSeries_sensor = new List<LineSeries[]>();
            for (int i = 0; i < sensor_plots.Count; i++)
            {
                lineSeries_sensor.Add(new LineSeries[8]);
            }

            time = 0;
            status = true;
        }
        public static void setData(StringBuilder data) { data_received = data; }
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
            int lineseries_count = lineSeries.Count;
            int sensor_plots_count = sensor_plots.Count;
            int index = 0;
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
                                    lineSeries[j][k].Points.Add(newPoint);
                                }
                            }
                            AutomaticSliding(ref lineSeries);
                            logger.Debug($"Index:{index} time: {time}");
                            index++;
                            time += 0.05;
                        }

                        sensor_plots.ForEach(x => x.InvalidatePlot(true));
                    }
                }
                Bluetooth.SetStatusOfPlot(true);
            }
        }
        private void AutomaticSliding(ref List<LineSeries[]> listLineSeries)
        {
            if (time > 2)
            {
                for (int j = 0; j < listLineSeries.Count; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        listLineSeries[j][k].Points.RemoveAt(0);
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
                int line_splited_length = line_splited.Length;
                int group_length = line_splited_length / 3;

                for (int j = 0; j < line_splited_length; j++)
                {
                    if (j < group_length)
                        help.Add(float.Parse(line_splited[j], CultureInfo.InvariantCulture));
                    else if (j == group_length)
                    {
                        help_all.Add(help);
                        help = new List<float>();
                        help.Add(float.Parse(line_splited[j], CultureInfo.InvariantCulture));
                    }
                    else if (j >= group_length & j < group_length * 2)
                    {
                        help.Add(float.Parse(line_splited[j], CultureInfo.InvariantCulture));
                    }
                    else if (j == group_length * 2)
                    {
                        help_all.Add(help);
                        help = new List<float>();
                        help.Add(float.Parse(line_splited[j], CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        help.Add(float.Parse(line_splited[j], CultureInfo.InvariantCulture));
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
            plotModel.LegendOrientation = OxyPlot.LegendOrientation.Horizontal;
            //Orientacja pozioma
            plotModel.LegendPlacement = OxyPlot.LegendPlacement.Outside; //Poza planszą wykresu
            plotModel.LegendPosition = OxyPlot.LegendPosition.TopCenter; //Pozycja: góra, prawo
            plotModel.LegendBackground = OxyPlot.OxyColor.FromAColor(200, OxyPlot.OxyColors.White);//Tło białe
            plotModel.LegendBorder = OxyPlot.OxyColors.Black; //Ramka okna czarna

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
