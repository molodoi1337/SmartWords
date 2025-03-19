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

        readonly string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SmartWords", "passedIndexes.json");
        
        private int _currentIndex = 0;
        public int CurrentIndex
        {
            get => _currentIndex;
            private set { if (Set(ref _currentIndex, value)) OnCurrentIndexChanged(); }
        }

        private List<int> _passedIndexes;
        public List<int> PassedIndexes => _passedIndexes;

        public LambdaCommand NextWordCommand { get; } // Команда для перехода к следующему слову

        private List<Word> _words; // Коллекция слов из JSON
        public List<Word> Words => _words;

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

                _words = JsonSerializer.Deserialize<List<Word>>(json, options);

                if (_words == null || _words.Count == 0)
                {
                    MessageBox.Show("Файл JSON пуст или не удалось загрузить данные.");
                    return;
                }


                if (CurrentIndex < 0 || CurrentIndex >= _words.Count)
                {
                    MessageBox.Show("Некорректный индекс текущего слова.");
                    return;
                }

                CurrentWord = _words[CurrentIndex];
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке JSON: {ex.Message}");
            }
        }

        private bool CanExecuteNextWord(object parameter)
        {
            return _words != null && CurrentIndex < _words.Count - 1;
        }

        private void OnNextWordCommandExecuted(object parameter)
        {
            if (_words == null || _words.Count == 0)
                return;

            _passedIndexes.Add(CurrentIndex);

            CurrentIndex++;
            if (CurrentIndex >= _words.Count)
                CurrentIndex = 0; // Вернуться к началу списка, если достигнут конец

            CurrentWord = _words[CurrentIndex];

            SaveCurrentIndex();
            SavePassedIndexes();
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


        private void SavePassedIndexes()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)); // Создаем папку, если её нет
            File.WriteAllText(filePath, JsonSerializer.Serialize(PassedIndexes));
        }

        private List<int> LoadPassedIndexes()
        {
            List<int> loadedIndexes = null;
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                loadedIndexes = JsonSerializer.Deserialize<List<int>>(json);
            }
            return loadedIndexes;
        }
        #endregion

        #region CurrentIndexChanged
        public event Action<int> CurrentIndexChanged;

        private void OnCurrentIndexChanged()
        {
            CurrentIndexChanged?.Invoke(_currentIndex);
        }
        #endregion

        public MainWindowViewModel()
        {
            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);
            NextWordCommand = new LambdaCommand(OnNextWordCommandExecuted, CanExecuteNextWord);
            CurrentIndex = LoadCurrentIndex();

            LoadWordsFromJson("C:\\Users\\nniki\\source\\repos\\SmartWords\\SmartWords\\Data\\words.json");
            _passedIndexes = LoadPassedIndexes() ?? new List<int>();
        }
    }
}