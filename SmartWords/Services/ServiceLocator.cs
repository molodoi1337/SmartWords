using SmartWords.Interface;

namespace SmartWords.Services
{
    internal class ServiceLocator
    {
        private static readonly List<ISavable> _savables = new List<ISavable>();

        public static void Register(ISavable savable) => _savables.Add(savable);
        public static void ExecuteAllSaves() => _savables.ForEach(s => s.Save());
    }
}
