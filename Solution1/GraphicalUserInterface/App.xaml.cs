using System.Windows;
using TP.ConcurrentProgramming.BusinessLogic.Abstractions;
using TP.ConcurrentProgramming.BusinessLogic.Services;
using TP.ConcurrentProgramming.Data.Abstractions;
using TP.ConcurrentProgramming.Data.Models;
using TP.ConcurrentProgramming.PresentationViewModel.ViewModels;

namespace TP.ConcurrentProgramming.PresentationView
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IBallRepository repository = new BallRepository();
            ILogicApi logicApi = new LogicApi(repository);
            MainWindowViewModel viewModel = new MainWindowViewModel(logicApi);

            MainWindow mainWindow = new MainWindow(viewModel);
            mainWindow.Show();
        }
    }
}