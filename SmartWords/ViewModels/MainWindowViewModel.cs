using SmartWords.Infrastructure.Commands.Base;
using SmartWords.ViewModels.Base;
using System.Windows;
using System.Windows.Input;

namespace SmartWords.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Команды

        #region CloseApplicationCommand
        public ICommand CloseApplicationCommand { get; }

        private bool CanCloseApplicationCommandExecute(object p) => true;

        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }


        #endregion

        #endregion

        public MainWindowViewModel()
        {
            #region Команды
            
            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);

            #endregion
        }
    }
}
