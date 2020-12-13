using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;

namespace CubliApp
{
    class Plots
    {
        private static StringBuilder data_received;
        PlotModel plotModel = new PlotModel();

        public static void setData(StringBuilder data) { data_received = data; }

        public void CreatePlot()
        {
            data_received = data_received.Append("3") ;
            int a = 0;
        }
        private void SetUpLegend()
        {
            plotModel.LegendTitle = "Legenda";
            plotModel.LegendOrientation = OxyPlot.LegendOrientation.Horizontal;

            //Orientacja pozioma
            plotModel.LegendPlacement = OxyPlot.LegendPlacement.Outside; //Poza planszą wykresu
            plotModel.LegendPosition = OxyPlot.LegendPosition.TopRight; //Pozycja: góra, prawo
            plotModel.LegendBackground = OxyPlot.OxyColor.FromAColor(200, OxyPlot.OxyColors.White);//Tło białe
            plotModel.LegendBorder = OxyPlot.OxyColors.Black; //Ramka okna czarna
        }
    }
}
