﻿using System;
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
        private List<Tuple<DateTime, double, double, double, double, double>> arrayForDataGrid;
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
                stockPointList.Add((XDate)t.Item1, t.Item3, t.Item4, t.Item2, t.Item5, t.Item6);
            }

            ZedGraphControl zedGraph = (ZedGraphControl)Host.Child;
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
            ema = Indicators.EMA(close, 5);
            macd = Indicators.MACD(close);
            obv = Indicators.OBV(close, volume);
            rsi = Indicators.RSI(close);
            //double test = Indicators.NRMSE(ema, sma);
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
            for (int i = 0; i < arrayForDataGrid.Count; i++)
            {
                list.Add((XDate) arrayForDataGrid[i].Item1, ma[i]);
                list2.Add((XDate) arrayForDataGrid[i].Item1, arrayForDataGrid[i].Item3);
            }

            ZedGraphControl zedGraphSMA = (ZedGraphControl) HostMA.Child;
            zedGraphSMA.GraphPane = new GraphPane();
            drawSettings(zedGraphSMA);
            GraphPane pane2 = zedGraphSMA.GraphPane;
            LineItem stickItem = pane2.AddCurve("", list, Color.Crimson, SymbolType.None);
            LineItem stickitem2 = pane2.AddCurve("", list2, Color.ForestGreen, SymbolType.None);
            stickItem.Color = Color.Crimson;
            stickitem2.Color = Color.ForestGreen;
            paneSettings(pane2);
            zedGraphSMA.AxisChange();
            zedGraphSMA.Invalidate();
            zedGraphSMA.Refresh();
            /*int i = myPane.AddYAxis("");
            myPane.YAxisList[i].Color = Color.Orange;
            myPane.YAxisList[i].Scale.IsVisible = false;
            myPane.YAxisList[i].MajorTic.IsAllTics = false;
            myPane.YAxisList[i].MinorTic.IsAllTics = false;
            myPane.YAxisList[i].Cross = pointOnXAxisThatIWantToMark;*/
        }

        private void drawMACD()
        {
            StockPointList list = new StockPointList();
            StockPointList list2 = new StockPointList();
            for (int i = 0; i < arrayForDataGrid.Count; i++)
            {
                list.Add((XDate) arrayForDataGrid[i].Item1, macd[i]);
                list2.Add((XDate) arrayForDataGrid[i].Item1, arrayForDataGrid[i].Item3);
            }

            ZedGraphControl zedGraphMACD = (ZedGraphControl) Host4.Child;
            zedGraphMACD.GraphPane = new GraphPane();
            drawSettings(zedGraphMACD);
            GraphPane pane2 = zedGraphMACD.GraphPane;
            LineItem stickItem = pane2.AddCurve("", list, Color.Crimson, SymbolType.None);
            paneSettings(pane2);
            stickItem.Color = Color.Crimson;
            zedGraphMACD.AxisChange();
            zedGraphMACD.Invalidate();
        }

        private void drawOBV()
        {
            StockPointList list = new StockPointList();
            for (int i = 0; i < arrayForDataGrid.Count; i++)
            {
                list.Add((XDate) arrayForDataGrid[i].Item1, obv[i]);
            }

            ZedGraphControl zedGraphOBV = (ZedGraphControl) Host5.Child;
            zedGraphOBV.GraphPane = new GraphPane();
            drawSettings(zedGraphOBV);
            GraphPane pane2 = zedGraphOBV.GraphPane;
            StickItem stickItem = pane2.AddStick("", list, Color.Crimson);
            paneSettings(pane2);
            stickItem.Color = Color.Crimson;
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
            LineItem stickItem = pane2.AddCurve("", list, Color.Crimson, SymbolType.None);
            paneSettings(pane2);
            stickItem.Color = Color.Crimson;
            zedGraphRSI.AxisChange();
            zedGraphRSI.Invalidate();
        }

        private static void paneSettings(GraphPane pane)
        {
            pane.Chart.Fill = new Fill(Color.White, Color.Azure, 45.0f);
            pane.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45.0f);
            pane.XAxis.Type = AxisType.DateAsOrdinal;
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
 