﻿using SmartWords.Infrastructure.Commands.Base;
using SmartWords.Interface;
using SmartWords.Models;
using SmartWords.Services;
using SmartWords.ViewModels.Base;
using System.Windows;
using System.Windows.Controls;

namespace SmartWords.ViewModels
{
    class Test : ViewModel, ISavable
    {
        public LambdaCommand ButtonClickCommand { get; }
        public LambdaCommand WindowClosingCommand { get; }

        private List<Word> _words;
        static int currentIndex;

        private int _selectedIndex = LoadControlIndex();
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
        static private int _correctAnswerIndex;
        private bool _isAnimating = false; // Флаг для блокировки кнопок
        private bool _isPassingTest = false;

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

        #region SaveAndLoadMethod
        private void SaveStudentStatusVisibility()
        {
            Properties.Settings.Default.StudyVisiable = StudyTabVisiable.ToString();
            Properties.Settings.Default.Save();
        }

        static private Visibility LoadStudentStatusVisibility()
        {
            var visibility = Properties.Settings.Default.StudyVisiable == "Collapsed" ? Visibility.Collapsed : Visibility.Visible;
            return visibility;
        }

        private void SaveTestStatusVisibility()
        {
            Properties.Settings.Default.TestVisiable = TestTabVisiable.ToString();
            Properties.Settings.Default.Save();
        }

        static private Visibility LoadTestStatusVisibility()
        {
            var visibility = Properties.Settings.Default.TestVisiable == "Collapsed" ? Visibility.Collapsed : Visibility.Visible;
            return visibility;
        }

        private void SaveControlIndex()
        {
            Properties.Settings.Default.ControlIndex = SelectedIndex;
        }

        static private int LoadControlIndex()
        {
            return Properties.Settings.Default.ControlIndex;
        }
        #endregion

        private void InitializeButtons()
        {
            if (_correctAnswerIndex >= _words.Count) return; // Защита от выхода за границы списка

            // Выбираем правильное слово по порядку
            _correctAnswer = _words[_correctAnswerIndex].Ru;

            // Создаем список ответов
            List<string> buttonTexts = new List<string> { _correctAnswer };

            Random random = new Random();

            int minIndex = Math.Max(0, _correctAnswerIndex - 9); // Если тест после 10 слов

            if (_correctAnswerIndex % 100 == 0)
                minIndex = Math.Max(0, _correctAnswerIndex - 99); // Если тест после 100 слов

            int maxIndex = _correctAnswerIndex; // Берем слова до текущего индекса

            // Добавляем еще два случайных слова из диапазона
            while (buttonTexts.Count < 3)
            {
                string word = _words[random.Next(minIndex, maxIndex + 1)].Ru;
                if (!buttonTexts.Contains(word))
                {
                    buttonTexts.Add(word);
                }
            }

            // Перемешиваем кнопки
            buttonTexts = Shuffle(buttonTexts);

            // Устанавливаем текст кнопок
            Button1Text = buttonTexts[0];
            Button2Text = buttonTexts[1];
            Button3Text = buttonTexts[2];

            // Устанавливаем слово вопроса (англ. слово и транскрипцию)
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
                        SelectedIndex = 0;
                        _isPassingTest = false;
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

            ServiceLocator.Register(this);
            WindowClosingCommand = new LambdaCommand((object _) => ServiceLocator.ExecuteAllSaves());

        }

        // Обработчик события изменения индекса
        private void OnCurrentIndexChanged(int newIndex)
        {
            if ((newIndex % 10 == 0 && newIndex != 0) || (newIndex % 100 == 0 && newIndex != 0))
            {
                currentIndex = newIndex - 1;

                // Если тест идет после 10 слов – берем последние 10 слов
                if (newIndex % 10 == 0 && newIndex % 100 != 0)
                    _correctAnswerIndex = Math.Max(0, currentIndex - 9); // Берем последние 10 слов

                // Если тест идет после 100 слов – берем последние 100 слов
                if (newIndex % 100 == 0)
                    _correctAnswerIndex = Math.Max(0, currentIndex - 99); // Берем последние 100 слов

                TestTabVisiable = Visibility.Visible;
                StudyTabVisiable = Visibility.Collapsed;
                SaveTestStatusVisibility();
                SaveStudentStatusVisibility();
                SelectedIndex = 1;
                _isPassingTest = true;
                InitializeButtons();
            }
        }


        public void Save()
        {
            if (_isPassingTest)
            {
                SelectedIndex = 0;
                TestTabVisiable = Visibility.Collapsed;
                StudyTabVisiable = Visibility.Visible;
            }
            SaveTestStatusVisibility();
            SaveStudentStatusVisibility();
            SaveControlIndex();
        }
    }
}
