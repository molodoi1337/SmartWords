using SmartWords.Models;
using System.IO;
using System.Text.Json;

namespace SmartWords.ViewModels
{
    class SearchWord
    {
        private readonly string unlearnedPath = "unlearned_words.json";
        private readonly string allWordsPath = "Data\\mainWords.json";

        public List<Word> Words { get; set; }
        public List<Word> UnlearnedWords { get; set; }

        public List<Word> GetWords()
        {
            var json = File.ReadAllText(allWordsPath);

            var options = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip, // Пропуск комментариев
                AllowTrailingCommas = true, // Разрешение trailing запятых
                PropertyNameCaseInsensitive = true // Игнорирование регистра свойств
            };

            Words = JsonSerializer.Deserialize<List<Word>>(json, options);

            return Words;
        }

        public List<Word> GetUnlearnedWords()
        {
            var json = File.ReadAllText(unlearnedPath);

            if (string.IsNullOrWhiteSpace(json))
                return UnlearnedWords = null;

            var UnlearnedWordsId = JsonSerializer.Deserialize<List<int>>(json);

            UnlearnedWords = Words.Where(word => UnlearnedWordsId.Contains(word.Id)).ToList();

            return UnlearnedWords;
        }
    }
}
