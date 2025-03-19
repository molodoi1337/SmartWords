using SmartWords.ViewModels;
using SmartWords.Views.Windows;
using System.Windows;

namespace SmartWords
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Создаем ViewModel
            var mainWindowViewModel = new MainWindowViewModel();

            // Создаем экземпляр Test и передаем ему ViewModel
            var test = new Test(mainWindowViewModel);

            // Создаем MainWindow и устанавливаем DataContext
            var mainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel
            };

            // Показываем MainWindow
            mainWindow.Show();
        }
    }
}
