using LiveCharts;
using LiveCharts.Wpf;
using SmartWords.ViewModels.Base;
using System.Windows.Media;

namespace SmartWords.ViewModels
{
    internal class PieChart : ViewModel
    {
        private SeriesCollection _pieChartSeries;
        public SeriesCollection PieChartSeries
        {
            get => _pieChartSeries;
            set => Set(ref _pieChartSeries, value);
        }

        public PieChart()
        {
            PieChartSeries = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Выученные",
                    Values = new ChartValues<int> { 10 },
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6e2b58")),
                    DataLabels = false,
                    LabelPoint = point => $"{10} "
                },
                new PieSeries
                {
                    Title = "Невыученные ",
                    Values = new ChartValues<int> { 5 },
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D7D2D")),
                    DataLabels = false,
                    LabelPoint = point => $"{5} "
                },
                new PieSeries
                {
                    Title = "Осталось",
                    Values = new ChartValues<int> { 3 },
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1b315f")),
                    DataLabels = false,
                    LabelPoint = point => $"{3} "
                }
            };
        }
    }
}
