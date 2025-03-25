using SmartWords.Infrastructure.Commands.Base;
using SmartWords.Models;
using SmartWords.ViewModels.Base;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace SmartWords.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region CloseApplicationCommand
        public LambdaCommand CloseApplicationCommand { get; }

        private bool CanCloseApplicationCommandExecute(object p) => true;

        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }
        #endregion

        #region NextWordCommand
        private Word _currentWord;
        public Word CurrentWord
        {
            get => _currentWord;
            set => Set(ref _currentWord, value);
        }

        private int _currentIndex = 0;
        public int CurrentIndex
        {
            get => _currentIndex;
            private set
            {
                if (Set(ref _currentIndex, value))
                    OnCurrentIndexChanged();
            }
        }

        //private List<int> _passedIndexes;
        //public List<int> PassedIndexes => _passedIndexes;

        public LambdaCommand NextWordCommand { get; } // Команда для перехода к следующему слову

        private List<Word> _words; // Коллекция слов из JSON
        public List<Word> Words
        {
            get => _words ??= new List<Word>();
            set => Set(ref _words, value);
        }

        private void LoadWordsFromJson(string filePath)
        {
            try
            {
                var json = File.ReadAllText(filePath);

                // Настройки для десериализации
                var options = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip, // Пропуск комментариев
                    AllowTrailingCommas = true, // Разрешение trailing запятых
                    PropertyNameCaseInsensitive = true // Игнорирование регистра свойств
                };

                Words = JsonSerializer.Deserialize<List<Word>>(json, options);

                if (Words == null || Words.Count == 0)
                {
                    MessageBox.Show("Файл JSON пуст или не удалось загрузить данные.");
                    return;
                }


                if (CurrentIndex < 0 || CurrentIndex >= _words.Count)
                {
                    MessageBox.Show("Некорректный индекс текущего слова.");
                    return;
                }

                CurrentWord = Words[CurrentIndex];
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке JSON: {ex.Message}");
            }
        }

        private bool CanExecuteNextWord(object parameter)
        {
            return Words != null && CurrentIndex < Words.Count - 1;
        }

        private void OnNextWordCommandExecuted(object parameter)
        {
            if (Words == null || Words.Count == 0)
                return;

            CurrentIndex++;
            if (CurrentIndex >= Words.Count)
                CurrentIndex = 0;

            CurrentWord = Words[CurrentIndex];

            SaveCurrentIndex();
        }

        private void SaveCurrentIndex()
        {
            Properties.Settings.Default.LastWordIndex = CurrentIndex;
            Properties.Settings.Default.Save();
        }

        private int LoadCurrentIndex()
        {
            return Properties.Settings.Default.LastWordIndex;
        }

        #endregion

        #region CurrentIndexChanged
        public event Action<int> CurrentIndexChanged;

        private void OnCurrentIndexChanged()
        {
            CurrentIndexChanged?.Invoke(_currentIndex);
        }
        #endregion
        public Test TestViewModel { get; }
        public MainWindowViewModel()
        {
            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);
            NextWordCommand = new LambdaCommand(OnNextWordCommandExecuted, CanExecuteNextWord);
            CurrentIndex = 0;//LoadCurrentIndex();
            LoadWordsFromJson("C:\\Users\\nniki\\source\\repos\\SmartWords\\SmartWords\\Data\\words.json");
            TestViewModel = new Test(this);
        }
    }
}