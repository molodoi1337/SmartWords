using LiveCharts;
using LiveCharts.Definitions.Charts;
using LiveCharts.Wpf;
using SmartWords.ViewModels.Base;
using System.IO;
using System.Text.Json;
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
        private readonly MainWindowViewModel _mainVM;

        public PieChart(MainWindowViewModel mainVM)
        {
            _mainVM = mainVM;

            UpdateChart();
        }

        static public int LoadUnlearnedWords()
        {
            if (File.Exists("unlearned_words.json"))
            {
                string json = File.ReadAllText("unlearned_words.json");
                var wordsFromFile = JsonSerializer.Deserialize<List<int>>(json);
                return wordsFromFile.Count;
            }
            return 0;
        }

        public void UpdateChart()
        {
            int unlearnedWordsCount = LoadUnlearnedWords();
            int learnedWordsCount = _mainVM.CurrentIndex - unlearnedWordsCount;
            int remainsLeaenedWordsCount = _mainVM.Words.Count - learnedWordsCount - unlearnedWordsCount;

            PieChartSeries = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Выученные",
                    Values = new ChartValues<int> { learnedWordsCount },
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6e2b58")),
                    DataLabels = false,
                    LabelPoint = point => $"{learnedWordsCount} "
                },
                new PieSeries
                {
                    Title = "Невыученные ",
                    Values = new ChartValues<int> { unlearnedWordsCount },
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D7D2D")),
                    DataLabels = false,
                    LabelPoint = point => $"{unlearnedWordsCount} "
                },
                new PieSeries
                {
                    Title = "Осталось",
                    Values = new ChartValues<int> { remainsLeaenedWordsCount },
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1b315f")),
                    DataLabels = false,
                    LabelPoint = point => $"{remainsLeaenedWordsCount} "
                }
            };
        }
    }
}
