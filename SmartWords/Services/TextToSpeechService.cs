using SmartWords.Infrastructure.Commands.Base;
using BogaNet.TTS;

namespace SmartWords.Services
{
    internal class TextToSpeechService
    {
        public LambdaCommand TTSCommand { get; }
        public string text = "Bus on the Hidden";

        public TextToSpeechService()
        {
            TTSCommand = new LambdaCommand(OnTTSComandExecuted);
        }

        public void OnTTSComandExecuted(object param)
        {
            Speaker.Instance.SpeakAsync(text);
        }
    }
}
