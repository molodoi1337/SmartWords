using SmartWords.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SmartWords.Views.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            SearchWord searchWord = new();

            sfDataGrid.SearchHelper.AllowFiltering = true;
            sfDataGrid.SearchHelper.SearchBrush = Brushes.Gray;
           

            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow)
                {
                    ((MainWindow)window).sfDataGrid.ItemsSource = searchWord.LoadJson();
                }
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            sfDataGrid.SearchHelper.Search(SearchBox.Text);
        }
    }
}