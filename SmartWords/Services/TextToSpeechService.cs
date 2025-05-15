using BogaNet.TTS;

namespace SmartWords.Services
{
    public class TextToSpeechService
    {
        public void Speak(string text)
        {
            Speaker.Instance.SpeakAsync(text);
        }
    }
}
