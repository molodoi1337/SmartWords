using SmartWords.Infrastructure.Commands.Base;
using SmartWords.Models;
using SmartWords.ViewModels.Base;
using System.Windows;
using System.Windows.Controls;

namespace SmartWords.ViewModels
{
    class Test : ViewModel
    {
        public LambdaCommand ButtonClickCommand { get; }

        private List<Word> _words;
        static int currentIndex;

        private int _selectedIndex = 0;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => Set(ref _selectedIndex, value);
        }


        private Visibility _testTabVisiable = LoadTestStatusVisibility();
        public Visibility TestTabVisiable
        {
            get => _testTabVisiable;
            set => Set(ref _testTabVisiable, value);
        }
        private Visibility _studyTabVisiable = LoadStudentStatusVisibility();
        public Visibility StudyTabVisiable
        {
            get => _studyTabVisiable;
            set => Set(ref _studyTabVisiable, value);
        }

        private string _textBlockText;
        public string TextBlockText
        {
            get => _textBlockText;
            set => Set(ref _textBlockText, value);
        }

        private string _button1Text;
        private string _button2Text;
        private string _button3Text;
        private string _russianWord;
        private string _transctiption;
        private string _correctAnswer;
        static private int _correctAnswerIndex = 0;

        public string Button1Text
        {
            get => _button1Text;
            set => Set(ref _button1Text, value);
        }

        public string Button2Text
        {
            get => _button2Text;
            set => Set(ref _button2Text, value);
        }

        public string Button3Text
        {
            get => _button3Text;
            set => Set(ref _button3Text, value);
        }

        public string RussianWord
        {
            get => _russianWord;
            private set => Set(ref _russianWord, value);
        }
        public string Transctiption
        {
            get => _transctiption;
            private set => Set(ref _transctiption, value);
        }
        private void SaveStudentStatusVisibility(string status = "Visible")
        {
            Properties.Settings.Default.StudyVisiable = status;
            Properties.Settings.Default.Save();
        }

        static private Visibility LoadStudentStatusVisibility()
        {
            var visibility = Properties.Settings.Default.StudyVisiable == "Collapsed" ? Visibility.Collapsed : Visibility.Visible;
            return visibility;
        }

        private void SaveTestStatusVisibility(string status = "Visible")
        {
            Properties.Settings.Default.TestVisiable = status;
            Properties.Settings.Default.Save();
        }

        static private Visibility LoadTestStatusVisibility()
        {
            var visibility = Properties.Settings.Default.TestVisiable == "Collapsed" ? Visibility.Collapsed : Visibility.Visible;
            return visibility;
        }
        private void SaveCorrectAnswerIndex()
        {
            Properties.Settings.Default.CorrectAnswerIndex = _correctAnswerIndex;
            Properties.Settings.Default.Save();
        }

        static private int LoadCorrectAnswerIndex()
        {
            return Properties.Settings.Default.CorrectAnswerIndex;
        }

        private void InitializeButtons()
        {
            Random random = new Random();

            // Выбираем правильный ответ по порядку
            _correctAnswer = _words[_correctAnswerIndex].Ru;

            // Создаем список для текста кнопок
            List<string> buttonTexts = new List<string> { _correctAnswer }; // Добавляем правильный ответ

            // Добавляем еще два случайных слова не совпадающих с ответом
            while (buttonTexts.Count < 3)
            {
                int temp = currentIndex - 1;
                string word = _words[random.Next(temp - 1)].Ru;
                if (!buttonTexts.Contains(word))
                {
                    buttonTexts.Add(word);
                }
            }

            // Перемешиваем слова, чтобы правильный ответ не всегда был на одной и той же кнопке
            buttonTexts = Shuffle(buttonTexts);

            // Присваиваем слова кнопкам
            Button1Text = buttonTexts[0];
            Button2Text = buttonTexts[1];
            Button3Text = buttonTexts[2];

            // Устанавливаем английское слово и транскрипцию
            RussianWord = _words[_correctAnswerIndex].En;
            Transctiption = _words[_correctAnswerIndex].Tr;
        }

        private List<string> Shuffle(List<string> list)
        {
            Random random = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                string value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        private bool _isAnimating = false; // Флаг для блокировки кнопок

        private void OnButtonClick(object? parameter)
        {
            if (_isAnimating || parameter is not Button button)
                return;

            _isAnimating = true; // Блокируем кнопки

            button.Tag = button.Content?.ToString() == _correctAnswer ? "Correct" : "Incorrect";

            _correctAnswerIndex++;

            Task.Delay(350).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    button.Tag = null;
                    _isAnimating = false; // Разблокируем кнопки
                    if ((_correctAnswerIndex % 10 == 0 && _correctAnswerIndex != 0) || (_correctAnswerIndex % 100 == 0 && _correctAnswerIndex != 0))
                    {
                        TestTabVisiable = Visibility.Collapsed;
                        StudyTabVisiable = Visibility.Visible;
                        SaveTestStatusVisibility("Collapsed");
                        SaveStudentStatusVisibility();
                        SelectedIndex = 0;
                        _correctAnswerIndex += 10;
                    }
                    else { InitializeButtons(); }
                });
            });
        }

        private MainWindowViewModel mainWindow;
        public Test(MainWindowViewModel viewModel)
        {
            mainWindow = viewModel;
            mainWindow.CurrentIndexChanged += OnCurrentIndexChanged;
            _words = mainWindow.Words;

            ButtonClickCommand = new LambdaCommand(OnButtonClick);
        }

        // Обработчик события изменения индекса
        private void OnCurrentIndexChanged(int newIndex)
        {
            if ((newIndex % 10 == 0 && newIndex != 0) || (newIndex % 100 == 0 && newIndex != 0))
            {
                currentIndex = newIndex - 1;
                TestTabVisiable = Visibility.Visible;
                StudyTabVisiable = Visibility.Collapsed;
                SaveTestStatusVisibility();
                SaveStudentStatusVisibility("Collapsed");
                SelectedIndex = 1;
                InitializeButtons();
            }
        }
    }
}
