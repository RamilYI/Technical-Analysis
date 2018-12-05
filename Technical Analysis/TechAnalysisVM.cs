using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Technical_Analysis.Properties;
namespace Technical_Analysis
{
    public class TechAnalysisVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
        }

        public TechAnalysisVM()
        {
            //OxyStorage = OxyArray;
            datagridStorage = arrayForDataGrid;
            ButtonCommand = new RelayCommand(o => MainButtonClick());
        }

        public ICommand ButtonCommand { get; set; }
        //public List<DataPoint> OxyStorage { get; set; }
        public object datagridStorage { get; set; }

        //public object OxyStorage2 { get; set; }


        private List<Tuple<DateTime, double, double, double, double, double>> arrayForDataGrid =
            new List<Tuple<DateTime, double, double, double, double, double>>();

        //private List<DataPoint> OxyArray = new List<DataPoint>();
        //private List<BoxPlotItem> OxyArray2 = new List<BoxPlotItem>();
        //private List<BoxPlotItem> OxyArray3 = new List<BoxPlotItem>();
        private void MainButtonClick()
        {
            arrayForDataGrid.Clear();
            OpenCsv();
            //OnPropertyChanged("OxyStorage");
            OnPropertyChanged("datagridArr");
            //OnPropertyChanged("OxyStorage2");
        }

        internal void OpenCsv()
        {
            var csvFile = new OpenFileDialog
            {
                Filter = Resources.TestOxyPlot_openCsv_csv_файл___csv
            };
            if (csvFile.ShowDialog() == DialogResult.OK && !csvFile.FileName.Equals(""))
            {
                var csvArray = File.ReadAllLines(csvFile.FileName)
                    .Skip(1)
                    .ToArray();
                TransformToArray(csvArray);
            }
        }
        

        internal void TransformToArray(string[] pseudoArray)
        {
            foreach (var t in pseudoArray)
            {
                var dateTimeValue = t.Split(';').First();
                var bufArray = t.Split(';')
                    .Skip(1)
                    .Select(s => double.Parse(s,
                        CultureInfo.InvariantCulture))
                    .ToArray();

                arrayForDataGrid.Add(new Tuple<DateTime, double, double, double, double, double>(DateTime.Parse(dateTimeValue),
                    bufArray[0],
                    bufArray[1],
                    bufArray[2],
                    bufArray[3],
                    bufArray[4]));
                //OxyArray.Add(new DataPoint(bufArray[0],
                //    bufArray[5]));
            }

        }

    }
}