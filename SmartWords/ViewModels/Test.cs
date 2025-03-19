using SmartWords.Infrastructure.Commands.Base;
using SmartWords.Models;
using SmartWords.ViewModels.Base;
using System.Windows;

namespace SmartWords.ViewModels
{
    class Test : ViewModel
    {
        public LambdaCommand TestCommand { get; }
        private bool CanTestCommandExecuted(object p) => true;

        public List<Word> words;
        public List<int> passedIndexes;
        
        private MainWindowViewModel mainWindow;

        public Test(MainWindowViewModel viewModel)
        {
            mainWindow = viewModel;
            mainWindow.CurrentIndexChanged += OnCurrentIndexChanged;
            words = mainWindow.Words;
            passedIndexes = mainWindow.PassedIndexes;
        }

        // Обработчик события изменения индекса
        private void OnCurrentIndexChanged(int newIndex)
        {
            if (newIndex % 10 == 0 && newIndex != 0)
            {
                ShowTest();
            }
        }

        private void ShowTest()
        {
            foreach(var i in passedIndexes)
            {
                MessageBox.Show($"индекс -> {i}");
            }
        }
    }
}
