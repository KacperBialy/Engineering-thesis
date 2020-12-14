using OxyPlot;
using OxyPlot.Wpf;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using LineSeries = OxyPlot.Series.LineSeries;

namespace CubliApp
{
    class Plots
    {
        private Thread updateThread { get; set; }
        private static StringBuilder data_received;
        private PlotView plot;
        private LineSeries[] lineSeries;
        private double time;
        private string[] Titles = { "AccX", "AccY", "AccZ", "GyrX", "GyrY", "GyrZ", "Roll", "Pitch" };
        public Plots(PlotView plt)
        {
            plot = plt;
            lineSeries = new LineSeries[8];
            time = 0;
        }
        public static void setData(StringBuilder data) { data_received = data; }
        public void CreatePlot()
        {
            PlotModel model = new PlotModel();
            plot.Model = model;
            model = SetUpLegend(model);
            for (int i = 0; i < lineSeries.Length; i++)
            {
                lineSeries[i] = new LineSeries();
                lineSeries[i].Title = Titles[i];
                model.Series.Add(lineSeries[i]);
            }
            plot.Controller = GetCustomController();
            updateThread = new Thread(UpdatePlot);
            updateThread.Start();
        }
        public void UpdatePlot()
        {
            while (true)
            {
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
                                help.Add(float.Parse(line_splited[j]));
                            }

                            group_data.Add(help);
                        }

                        for (int i = 0; i < group_data.Count; i++)
                        {

                            for (int j = 0; j < lineSeries.Length; j++)
                            {
                                lineSeries[j].Points.Add(new DataPoint(time, group_data[i][j]));
                            }
                            time += 0.02;

                            if (time > 2)
                            {
                                for (int j = 0; j < lineSeries.Length; j++)
                                {
                                    lineSeries[j].Points.RemoveAt(0);
                                }
                            }
                        }

                        plot.InvalidatePlot(true);
                    }
                }
            }
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
