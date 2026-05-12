using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TP.ConcurrentProgramming.BusinessLogic.Abstractions;
using TP.ConcurrentProgramming.PresentationModel.Models;
using TP.ConcurrentProgramming.PresentationViewModel.Commands;

namespace TP.ConcurrentProgramming.PresentationViewModel.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly ILogicApi _logicApi;
        private readonly BoardPresentationModel _boardPresentationModel;
        private readonly SynchronizationContext? _synchronizationContext;

        private CancellationTokenSource? _cancellationTokenSource;
        private bool _isRunning;
        private int _ballsCount = 5;

        private double _boardWidth = 800;
        private double _boardHeight = 400;

        private double _logicBoardWidth = 800;
        private double _logicBoardHeight = 400;

        private double _viewBoardWidth = 800;
        private double _viewBoardHeight = 400;

        public MainWindowViewModel(ILogicApi logicApi)
        {
            _logicApi = logicApi;
            _boardPresentationModel = new BoardPresentationModel();

            _synchronizationContext = SynchronizationContext.Current;

            _logicApi.BallsUpdated += OnBallsUpdated;

            CreateBallsCommand = new RelayCommand(CreateBalls, () => !_isRunning);
            StartCommand = new RelayCommand(Start, () => !_isRunning);
            StopCommand = new RelayCommand(Stop, () => _isRunning);
        }

        public BoardPresentationModel Board => _boardPresentationModel;

        public int BallsCount
        {
            get => _ballsCount;
            set
            {
                _ballsCount = value;
                OnPropertyChanged();
            }
        }

        public double BoardWidth
        {
            get => _boardWidth;
            set
            {
                _boardWidth = value;
                OnPropertyChanged();
            }
        }

        public double BoardHeight
        {
            get => _boardHeight;
            set
            {
                _boardHeight = value;
                OnPropertyChanged();
            }
        }

        public double LogicBoardWidth
        {
            get => _logicBoardWidth;
            set
            {
                _logicBoardWidth = value;
                OnPropertyChanged();
            }
        }

        public double LogicBoardHeight
        {
            get => _logicBoardHeight;
            set
            {
                _logicBoardHeight = value;
                OnPropertyChanged();
            }
        }

        public double ViewBoardWidth
        {
            get => _viewBoardWidth;
            set
            {
                _viewBoardWidth = value;
                OnPropertyChanged();
            }
        }

        public double ViewBoardHeight
        {
            get => _viewBoardHeight;
            set
            {
                _viewBoardHeight = value;
                OnPropertyChanged();
            }
        }

        public ICommand CreateBallsCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        private void CreateBalls()
        {
            _logicApi.CreateBalls(BallsCount, LogicBoardWidth, LogicBoardHeight);
        }

        private void Start()
        {
            if (_isRunning)
            {
                return;
            }

            _isRunning = true;
            RaiseCommands();

            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;

            Task.Run(async () =>
            {
                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        _logicApi.UpdateBalls(LogicBoardWidth, LogicBoardHeight);
                        await Task.Delay(16, token);
                    }
                }
                catch (TaskCanceledException)
                {
                }
            }, token);
        }

        private void Stop()
        {
            if (!_isRunning)
            {
                return;
            }

            _cancellationTokenSource?.Cancel();
            _isRunning = false;
            RaiseCommands();
        }

        private void OnBallsUpdated()
        {
            if (_synchronizationContext == null)
            {
                UpdatePresentationModel();
                return;
            }

            _synchronizationContext.Post(_ =>
            {
                UpdatePresentationModel();
            }, null);
        }

        private void UpdatePresentationModel()
        {
            _boardPresentationModel.UpdateFromLogic(
                _logicApi.GetBalls(),
                LogicBoardWidth,
                LogicBoardHeight,
                ViewBoardWidth,
                ViewBoardHeight
            );
        }

        private void RaiseCommands()
        {
            ((RelayCommand)CreateBallsCommand).RaiseCanExecuteChanged();
            ((RelayCommand)StartCommand).RaiseCanExecuteChanged();
            ((RelayCommand)StopCommand).RaiseCanExecuteChanged();
        }
    }
}