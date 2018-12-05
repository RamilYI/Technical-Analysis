using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms.DataVisualization.Charting;
using ZedGraph;
using AxisType = ZedGraph.AxisType;

namespace Technical_Analysis
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Tuple<DateTime, double, double, double, double, double>> arrayForDataGrid;

        StockPointList stockPointList = new StockPointList();
        private JapaneseCandleStickItem myCurve;
        public MainWindow()
        {
            InitializeComponent();
            //ZedGraphControl zedGraph = (ZedGraphControl) Host.Child;
            //GraphPane pane = zedGraph.GraphPane;
            //PointPairList pairList = testZed();
            //pane.AddJapaneseCandleStick("meme", pairList);
            //zedGraph.AxisChange();
            //zedGraph.Invalidate();

            //var s2 = new BarItem.BarSeries { Title = "Series 2", StrokeThickness = 1 };
            //s2.Items.Add(new OxyPlot.Series.BarItem { Value = 12 });
            //s2.Items.Add(new OxyPlot.Series.BarItem { Value = 14 });
            //s2.Items.Add(new OxyPlot.Series.BarItem { Value = 120 });
            //s2.Items.Add(new OxyPlot.Series.BarItem { Value = 26 });
            //Plot1.Series.Add(s2);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            //arrayForDataGrid?.Clear();
            //Plot1.InvalidatePlot();
            myCurve?.Clear();
            DataGrid.Columns[0].Header = "DATE";
            DataGrid.Columns[1].Header = "OPEN";
            DataGrid.Columns[2].Header = "HIGH";
            DataGrid.Columns[3].Header = "LOW";
            DataGrid.Columns[4].Header = "CLOSE";
            DataGrid.Columns[5].Header = "VOL";
            CollectionViewSource.GetDefaultView(DataGrid.ItemsSource).Refresh();
            arrayForDataGrid = (List<Tuple<DateTime, double, double, double, double, double>>)DataGrid.ItemsSource;
            zedFill();
        }

        private void zedFill()
        {
            stockPointList.Clear();
            for (var i = 0; i < arrayForDataGrid.Count; i++)
            {
                var t = arrayForDataGrid[i];
                stockPointList.Add((XDate)t.Item1,
                    t.Item3,
                    t.Item4,
                    t.Item2,
                    t.Item5,
                    t.Item6);
            }

            ZedGraphControl zedGraph = (ZedGraphControl)Host.Child;
            GraphPane pane = zedGraph.GraphPane;
            zedGraph.IsShowHScrollBar = true;
            zedGraph.IsShowVScrollBar = true;
            zedGraph.IsScrollY2 = true;
            zedGraph.IsAutoScrollRange = true;
            myCurve = pane.AddJapaneseCandleStick("", stockPointList);
            myCurve.Stick.IsAutoSize = true;
            myCurve.Stick.Color = Color.Green;
            pane.Chart.Fill = new Fill(Color.White, Color.Azure, 45.0f);
            pane.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45.0f);
            pane.XAxis.Type = AxisType.DateAsOrdinal;
            pane.XAxis.Scale.Min = new XDate(01,12,2017);
            zedGraph.AxisChange();
            zedGraph.Invalidate();
        }

        //private static PointPairList testZed()
        //{
        //    PointPairList point = new PointPairList();
        //    for (int i = 0; i < 10; i++)
        //    {
        //        point.Add(i, Math.Pow(i,3), 3*i);
        //    }

        //    return point;

        //}
    }
}