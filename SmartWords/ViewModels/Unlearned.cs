using SmartWords.Interface;
using SmartWords.Services;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace SmartWords.ViewModels
{
    class Unlearned : ISavable
    {
        private List<int> unlearnedIndexList = new();
        private readonly string filePath = "unlearned_words.json";
        private Test _test;

        public Unlearned(Test test)
        {
            _test = test;
            LoadUnlearnedWords();
            ServiceLocator.Register(this);
        }

        public void AddUnlernedIndex(int index)
        {
            if (!unlearnedIndexList.Contains(index))
                unlearnedIndexList.Add(index);
        }

        private void LoadUnlearnedWords()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    var wordsFromFile = JsonSerializer.Deserialize<List<int>>(json);
                    if (wordsFromFile != null)
                        unlearnedIndexList = wordsFromFile;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке списка невыученных слов: {ex.Message}");
            }
        }

        public void Save()
        {
            try
            {
                List<int> beforeGarbageCollector = unlearnedIndexList;
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    AllowTrailingCommas = true
                };

                string json = JsonSerializer.Serialize(beforeGarbageCollector, options);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении не выученных слов : {ex.Message}");
            }
        }
    }
}
