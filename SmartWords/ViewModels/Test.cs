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
        static int currentIndex = 0; //Properties.Settings.Default.LastWordIndex;

        private Visibility _isTestTabVisiable = Visibility.Collapsed;
        public Visibility IsTestTabVisiable
        {
            get => _isTestTabVisiable;
            set => Set(ref _isTestTabVisiable, value);
        }

        private string _textBlockText;
        public string TextBlockText
        {
            get => _textBlockText;
            set => Set(ref _textBlockText, value);
        }
        //
        private string _button1Text;
        private string _button2Text;
        private string _button3Text;
        private string _resultMessage;
        private string _transctiption;
        private string _correctAnswer;
        private int _correctAnswerIndex = currentIndex;

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
            get => _resultMessage;
            private set => Set(ref _resultMessage, value);
        }
        public string Transctiption
        {
            get => _transctiption;
            private set => Set(ref _transctiption, value);
        }

        private void InitializeButtons()
        {
            Random random = new Random();

            // Выбираем правильный ответ по порядку
            _correctAnswer = _words[_correctAnswerIndex].Ru;

            // Создаем список для текста кнопок
            List<string> buttonTexts = new List<string> { _correctAnswer }; // Добавляем правильный ответ

            // Добавляем еще два случайных слова (убедимся, что они не совпадают с правильным ответом)
            while (buttonTexts.Count < 3)
            {
                int temp = currentIndex - 1;
                string word = _words[random.Next(temp - 9, temp - 1)].Ru;
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

            if (button.Content?.ToString() == _correctAnswer)
            {
                button.Tag = "Correct";
            }
            else
            {
                button.Tag = "Incorrect";
            }

            _correctAnswerIndex++;

            Task.Delay(350).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    button.Tag = null;
                    _isAnimating = false; // Разблокируем кнопки
                    InitializeButtons(); // Обновляем вопросы/ответы
                });
            });
        }
        //
        public Test() { }
        private MainWindowViewModel mainWindow;
        public Test(MainWindowViewModel viewModel)
        {
            mainWindow = viewModel;
            mainWindow.CurrentIndexChanged += OnCurrentIndexChanged;
            _words = mainWindow.Words;

            IsTestTabVisiable = Visibility.Collapsed; 
            ButtonClickCommand = new LambdaCommand(OnButtonClick);
        }
        
        // Обработчик события изменения индекса
        private void OnCurrentIndexChanged(int newIndex)
        {
            newIndex--;
            currentIndex = newIndex;

            if (newIndex % 10 == 0 && newIndex != 0 || newIndex % 100 == 0 && newIndex != 0)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    IsTestTabVisiable = Visibility.Visible;
                });
                InitializeButtons();
            }
        }
    }
}
