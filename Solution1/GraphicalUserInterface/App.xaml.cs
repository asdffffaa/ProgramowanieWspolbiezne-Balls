using System;
using System.IO;
using System.Windows;
using TP.ConcurrentProgramming.BusinessLogic.Abstractions;
using TP.ConcurrentProgramming.BusinessLogic.Services;
using TP.ConcurrentProgramming.Data.Abstractions;
using TP.ConcurrentProgramming.Data.Diagnostics;
using TP.ConcurrentProgramming.Data.Models;
using TP.ConcurrentProgramming.PresentationViewModel.ViewModels;

namespace TP.ConcurrentProgramming.PresentationView
{
    public partial class App : Application
    {
        private IDiagnosticLogger? _diagnosticLogger;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string diagnosticsPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "diagnostics",
                "balls.jsonl");

            _diagnosticLogger = new AsyncFileDiagnosticLogger(diagnosticsPath);

            IBallRepository repository = new BallRepository();
            IDataApi dataApi = new DataApi(repository, _diagnosticLogger);
            ILogicApi logicApi = new LogicApi(dataApi);
            MainWindowViewModel viewModel = new MainWindowViewModel(logicApi);

            MainWindow mainWindow = new MainWindow(viewModel);
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _diagnosticLogger?.Dispose();
            base.OnExit(e);
        }
    }
}
