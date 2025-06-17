using SmartWords.Infrastructure.Commands.Base;
using SmartWords.Models;
using SmartWords.Views.Windows;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace SmartWords.ViewModels
{
    class SearchWord
    {
        public List<Word> Words { get; set; }
        public string filePath = "Data\\mainWords.json";

        public List<Word> LoadJson()
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

            return Words;
        }
    }
}
