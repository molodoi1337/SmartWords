using SmartWords.Models;
using SmartWords.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SmartWords.Views.Windows
{
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; } // Статическое свойство для доступа к экземпляру
        SearchWord searchWord = new();

        public MainWindow()
        {
            InitializeComponent();

            Instance = this;
            Application.Current.MainWindow = this; // Устанавливаем главное окно

            SearchDataGrid.SearchHelper.AllowFiltering = true;
            SearchDataGrid.SearchHelper.SearchBrush = Brushes.Gray;

            UnlearnedDataGrid.SearchHelper.SearchBrush = Brushes.Gray;
            UnlearnedDataGrid.SearchHelper.AllowFiltering = true;

            LoadTable();
        }

        public void LoadTable()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow)
                {
                    SearchDataGrid.ColumnSizer = Syncfusion.UI.Xaml.Grid.GridLengthUnitType.Star;
                    SearchDataGrid.ScrollMode = Syncfusion.UI.Xaml.Grid.ScrollMode.Async;
                    ((MainWindow)window).SearchDataGrid.ItemsSource = searchWord.GetWords().Select(item => new
                    {
                        Английский = item.En,
                        Русский = item.Ru,
                        Транскринция = item.Tr,
                    });

                    var unlearnedWords = searchWord.GetUnlearnedWords();

                    if (unlearnedWords?.Any() != true)
                    {
                        ((MainWindow)window).UnlearnedDataGrid.ItemsSource = new List<Word> {
                            new Word {En = "Пока нет не выученных слов" }
                        }
                        .Select(item => new { Ожидаем = "Пусто" });
                        UnlearnedDataGrid.Height = 100;
                    }
                    else
                    {
                        UnlearnedDataGrid.ColumnSizer = Syncfusion.UI.Xaml.Grid.GridLengthUnitType.Star;
                        UnlearnedDataGrid.ScrollMode = Syncfusion.UI.Xaml.Grid.ScrollMode.Async;
                        ((MainWindow)window).UnlearnedDataGrid.ItemsSource = searchWord.GetUnlearnedWords().Select(item => new
                        {
                            Английский = item.En,
                            Русский = item.Ru,
                            Транскринция = item.Tr,
                        });
                    }
                }
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchDataGrid.SearchHelper.Search(SearchBox.Text);
        }
    }
}