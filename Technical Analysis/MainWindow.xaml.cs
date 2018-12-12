using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms.Integration;
using ZedGraph;
using AxisType = ZedGraph.AxisType;

namespace Technical_Analysis
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<Tuple<DateTime, double, double, double, double, double>> arrayForDataGrid;
        double[] sma, ema, obv, macd, rsi;
        double[] close, volume;
        StockPointList stockPointList = new StockPointList();
        private JapaneseCandleStickItem myCurve;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            myCurve?.Clear();
            DataGrid.Columns[0].Header = "DATE";
            DataGrid.Columns[1].Header = "OPEN";
            DataGrid.Columns[2].Header = "HIGH";
            DataGrid.Columns[3].Header = "LOW";
            DataGrid.Columns[4].Header = "CLOSE";
            DataGrid.Columns[5].Header = "VOL";
            CollectionViewSource.GetDefaultView(DataGrid.ItemsSource).Refresh();
            arrayForDataGrid = (List<Tuple<DateTime, double, double, double, double, double>>) DataGrid.ItemsSource;
            zedFill();
            declarationVar();
            drawIndicators();
        }

        private void zedFill()
        {
            stockPointList.Clear();
            for (var i = 0; i < arrayForDataGrid.Count; i++)
            {
                var t = arrayForDataGrid[i];
                stockPointList.Add((XDate) t.Item1, t.Item3, t.Item4, t.Item2, t.Item5, t.Item6);
            }

            ZedGraphControl zedGraph = (ZedGraphControl) Host.Child;
            GraphPane pane = zedGraph.GraphPane;
            drawSettings(zedGraph);
            myCurve = pane.AddJapaneseCandleStick("", stockPointList);
            myCurve.Stick.IsAutoSize = true;
            myCurve.Stick.Color = Color.Green;
            paneSettings(pane);
            string CsvFileName;
            TechAnalysisVM setCsvFile = new TechAnalysisVM();
            setCsvFile.getCsvFile(out CsvFileName);
            pane.Title.Text = CsvFileName;
            zedGraph.AxisChange();
            zedGraph.Invalidate();
        }

        private void declarationVar()
        {
            sma = new double[stockPointList.Count];
            ema = new double[stockPointList.Count];
            close = new double[stockPointList.Count];
            volume = new double[stockPointList.Count];
            macd = new double[stockPointList.Count];
            obv = new double[stockPointList.Count];
            rsi = new double[stockPointList.Count];

            for (int i = 0; i < arrayForDataGrid.Count; i++)
            {
                close[i] = arrayForDataGrid[i].Item5;
                volume[i] = arrayForDataGrid[i].Item6;
            }

            sma = Indicators.SMA(close);
            ema = Indicators.EMA(close, 10);
            macd = Indicators.MACD(close);
            obv = Indicators.OBV(close, volume);
            rsi = Indicators.RSI(close);
        }

        private void drawIndicators()
        {
            drawMA(sma, Host2);
            drawMA(ema, Host3);
            drawMACD();
            drawOBV();
            drawRSI();
        }

        private void drawMA(double[] ma, WindowsFormsHost HostMA)
        {
            StockPointList list = new StockPointList();
            StockPointList list2 = new StockPointList();
            List<double> Buylines = new List<double>();
            List<double> Selllines = new List<double>();
            for (int i = 0; i < arrayForDataGrid.Count; i++)
            {
                list.Add((XDate) arrayForDataGrid[i].Item1, ma[i]);
                list2.Add((XDate) arrayForDataGrid[i].Item1, arrayForDataGrid[i].Item3);

                //if (Math.Abs(ma[i] - arrayForDataGrid[i].Item3) < 0.05 &&
                //    Math.Abs(Math.Abs(macd[i] - arrayForDataGrid[i].Item3)) > 0.0)
                //    lines.Add(i + 1);
            }

            for (int i = 1; i < arrayForDataGrid.Count; i++)
            {
                if (ma[i - 1] > arrayForDataGrid[i-1].Item3 && ma[i] < arrayForDataGrid[i].Item3) Buylines.Add(i);
                else if (ma[i - 1] < arrayForDataGrid[i - 1].Item3 && macd[i] > arrayForDataGrid[i].Item3) Selllines.Add(i);
            }

            ZedGraphControl zedGraphSMA = (ZedGraphControl) HostMA.Child;
            zedGraphSMA.GraphPane = new GraphPane();
            drawSettings(zedGraphSMA);
            GraphPane pane2 = zedGraphSMA.GraphPane;
            LineItem stickItem = pane2.AddCurve("MA", list, Color.Crimson, SymbolType.None);
            LineItem stickitem2 = pane2.AddCurve("Исходный график", list2, Color.ForestGreen, SymbolType.None);
            stickItem.Color = Color.Crimson;
            stickitem2.Color = Color.ForestGreen;
            paneSettings(pane2);
            drawCommonPoints(pane2, Buylines, Selllines);
            zedGraphSMA.AxisChange();
            zedGraphSMA.Invalidate();
            zedGraphSMA.Refresh();
        }

        private void drawMACD()
        {
            StockPointList list = new StockPointList();
            StockPointList list2 = new StockPointList();
            double[] expMACD = Indicators.EMA(macd, 10);
            List<double> Buylines = new List<double>();
            List<double> Selllines = new List<double>();

            for (int i = 0; i < arrayForDataGrid.Count; i++)
            {
                list.Add((XDate) arrayForDataGrid[i].Item1, macd[i]);
                list2.Add((XDate) arrayForDataGrid[i].Item1, expMACD[i]);

                //if (Math.Abs(macd[i] - expMACD[i]) < 0.05 && Math.Abs(Math.Abs(macd[i] - expMACD[i])) > 0.0)
                //    lines.Add(i + 1);
            }

            for (int i = 1; i < arrayForDataGrid.Count; i++)
            {
                if (macd[i-1] > expMACD[i-1] && macd[i] < expMACD[i] ) Buylines.Add(i);
                else if (macd[i - 1] < expMACD[i - 1] && macd[i] > expMACD[i]) Selllines.Add(i);
            }

            ZedGraphControl zedGraphMACD = (ZedGraphControl) Host4.Child;
            zedGraphMACD.GraphPane = new GraphPane();
            drawSettings(zedGraphMACD);
            GraphPane pane2 = zedGraphMACD.GraphPane;
            LineItem stickItem = pane2.AddCurve("MACD", list, Color.Crimson, SymbolType.None);
            paneSettings(pane2);
            stickItem.Color = Color.Crimson;
            LineItem stickItem2 =
                pane2.AddCurve("экспоненциальная средняя MACD", list2, Color.Green,
                    SymbolType.None); //экспоненц средняя MACD
            stickItem2.Color = Color.Green;
            drawCommonPoints(pane2, Buylines, Selllines);
            zedGraphMACD.AxisChange();
            zedGraphMACD.Invalidate();
        }

        private void drawOBV()
        {
            StockPointList list = new StockPointList();
            StockPointList list2 = new StockPointList();
            double[] expOBV = Indicators.EMA(obv, 10);
            for (int i = 0; i < arrayForDataGrid.Count; i++)
            {
                list.Add((XDate) arrayForDataGrid[i].Item1, obv[i]);
                list2.Add((XDate) arrayForDataGrid[i].Item1, expOBV[i]);
            }

            ZedGraphControl zedGraphOBV = (ZedGraphControl) Host5.Child;
            zedGraphOBV.GraphPane = new GraphPane();
            drawSettings(zedGraphOBV);
            GraphPane pane2 = zedGraphOBV.GraphPane;
            StickItem stickItem = pane2.AddStick("OBV", list, Color.Crimson);
            paneSettings(pane2);
            stickItem.Color = Color.Crimson;
            LineItem stickItem2 =
                pane2.AddCurve("экспоненциальная средняя OBV", list2, Color.Green,
                    SymbolType.None); //экспоненц средняя OBV
            stickItem2.Color = Color.Green;
            zedGraphOBV.AxisChange();
            zedGraphOBV.Invalidate();
        }

        private void drawRSI()
        {
            StockPointList list = new StockPointList();
            StockPointList list2 = new StockPointList();
            for (int i = 0; i < arrayForDataGrid.Count; i++)
            {
                list.Add((XDate) arrayForDataGrid[i].Item1, rsi[i]);
                list2.Add((XDate) arrayForDataGrid[i].Item1, arrayForDataGrid[i].Item3);
            }

            ZedGraphControl zedGraphRSI = (ZedGraphControl) Host6.Child;
            zedGraphRSI.GraphPane = new GraphPane();
            drawSettings(zedGraphRSI);
            GraphPane pane2 = zedGraphRSI.GraphPane;
            LineItem stickItem = pane2.AddCurve("RSI", list, Color.Crimson, SymbolType.None);
            paneSettings(pane2);
            double minOBV = 30;
            LineObj line1 = new LineObj(0, minOBV, list.Count, minOBV);

            double maxOBV = 70;
            LineObj line2 = new LineObj(0, maxOBV, list.Count, maxOBV);
            pane2.GraphObjList.Add(line1);
            pane2.GraphObjList.Add(line2);
            stickItem.Color = Color.Crimson;
            zedGraphRSI.AxisChange();
            zedGraphRSI.Invalidate();
        }

        private static void drawCommonPoints(GraphPane pane2, List<double> buy, List<double> sell)
        {
            drawBuyAndSellSignals(pane2, buy, Color.Indigo);
            drawBuyAndSellSignals(pane2, sell, Color.DarkOrange);
        }

        private static void drawBuyAndSellSignals(GraphPane pane2, List<double> n, Color color)
        {
            for (int i = 0; i < n.Count; i++)
            {
                var line = new LineObj(color, n[i], 0, n[i], 1);

                line.Location.CoordinateFrame = CoordType.XScaleYChartFraction;
                line.IsClippedToChartRect = true;

                line.Line.Width = 1f;

                pane2.GraphObjList.Add(line);
            }
        }

        private static void paneSettings(GraphPane pane)
        {
            pane.Chart.Fill = new Fill(Color.White, Color.Azure, 45.0f);
            pane.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45.0f);
            pane.XAxis.Type = AxisType.DateAsOrdinal;
            pane.Y2Axis.Type = AxisType.DateAsOrdinal;
            pane.X2Axis.IsVisible = true;
            pane.X2Axis.Scale.Min = 0;
            pane.X2Axis.Scale.Max = arrayForDataGrid.Count;
            pane.X2Axis.MajorGrid.IsVisible = true;
            pane.XAxis.Title.Text = "Дата";
            pane.YAxis.Title.Text = "Цена";
        }

        private static void drawSettings(ZedGraphControl zedGraphEMA)
        {
            zedGraphEMA.IsShowHScrollBar = true;
            zedGraphEMA.IsShowVScrollBar = true;
            zedGraphEMA.IsScrollY2 = true;
            zedGraphEMA.IsAutoScrollRange = true;
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
 