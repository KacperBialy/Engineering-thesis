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
        private Thread updateThread { get; set; }
        private static StringBuilder data_received;
        private List<PlotView> sensor_plots;
        private List<LineSeries[]> lineSeries_sensor;
        private double time;
        private string[] Titles = { "AccX", "AccY", "AccZ", "GyrX", "GyrY", "GyrZ", "Roll", "Pitch" };
        public Plots(PlotView plt_sensor_1, PlotView plt_sensor_2, PlotView plt_sensor_3)
        {
            sensor_plots = new List<PlotView>() { plt_sensor_1, plt_sensor_2, plt_sensor_3 };
            lineSeries_sensor = new List<LineSeries[]>();
            for (int i = 0; i < sensor_plots.Count; i++)
            {
                lineSeries_sensor.Add(new LineSeries[8]);
            }

            time = 0;
        }
        public static void setData(StringBuilder data) { data_received = data; }
        public void CreatePlots()
        {
            for (int i = 0; i < sensor_plots.Count; i++)
            {
            sensor_plots[i].Model = CreateModel(lineSeries_sensor[i]);
            sensor_plots[i].Controller = GetCustomController();
            }
            for (int i = 0; i < 3; i++)
            {
                if(i==0)
                    sensor_plots[i].Model = SetUpLegend(sensor_plots[0].Model);
                else
                    sensor_plots[i].Model.IsLegendVisible = false;
            }
            updateThread = new Thread(() => UpdatePlot(lineSeries_sensor));
            updateThread.Start();
        }
        public void UpdatePlot(List<LineSeries[]> lineSeries)
        {
            while (true)
            {
                int lineseries_count = lineSeries.Count;
                int sensor_plots_count = sensor_plots.Count;
                if (data_received != null)
                {
                    if (data_received.Length > 0)
                    {
                        string[] data = data_received.ToString().Split('\n');
                        data_received = new StringBuilder();
                        List<List<float>> group_data = new List<List<float>>();
                        for (int i = 0; i < data.Length - 1; i++)
                        {
                            List<float> help = new List<float>();
                            string[] line_splited = data[i].Split(';');

                            for (int j = 0; j < line_splited.Length; j++)
                            {
                                help.Add(float.Parse(line_splited[j], CultureInfo.InvariantCulture));
                            }

                            group_data.Add(help);
                        }

                        for (int i = 0; i < group_data.Count; i++)
                        {
                            for (int j = 0; j < lineseries_count; j++)
                            {
                                for (int k = 0; k < lineseries_count; k++)
                                {
                                    lineSeries[j][k].Points.Add(new DataPoint(time, group_data[i][j]));
                                }
                            }
                            time += 0.02;

                            if (time > 2)
                            {
                                for (int j = 0; j < lineseries_count; j++)
                                {
                                    for (int k = 0; k < lineseries_count; k++)
                                    {
                                        lineSeries[j][k].Points.RemoveAt(0);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < sensor_plots_count; i++)
                        {
                        sensor_plots[i].InvalidatePlot(true);
                        }
                    }
                }
            }
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
