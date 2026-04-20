using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.PresentationViewModel.ViewModels;

namespace TP.ConcurrentProgramming.PresentationViewModelTest
{
    [TestClass]
    public class MainWindowViewModelTests
    {
        [TestMethod]
        public void Constructor_ShouldCreateBoardModel()
        {
            FakeLogicApi logicApi = new FakeLogicApi();
            MainWindowViewModel viewModel = new MainWindowViewModel(logicApi);

            Assert.IsNotNull(viewModel.Board);
        }

        [TestMethod]
        public void BallsCount_Set_ShouldUpdateValue()
        {
            FakeLogicApi logicApi = new FakeLogicApi();
            MainWindowViewModel viewModel = new MainWindowViewModel(logicApi);

            viewModel.BallsCount = 7;

            Assert.AreEqual(7, viewModel.BallsCount);
        }

        [TestMethod]
        public void CreateBallsCommand_ShouldCallLogicApi()
        {
            FakeLogicApi logicApi = new FakeLogicApi();
            MainWindowViewModel viewModel = new MainWindowViewModel(logicApi)
            {
                BallsCount = 3
            };

            viewModel.CreateBallsCommand.Execute(null);

            Assert.AreEqual(1, logicApi.CreateBallsCallCount);
            Assert.AreEqual(3, viewModel.Board.Balls.Count);
        }

        [TestMethod]
        public void LogicEvent_ShouldUpdateBoardBalls()
        {
            FakeLogicApi logicApi = new FakeLogicApi();
            MainWindowViewModel viewModel = new MainWindowViewModel(logicApi)
            {
                BallsCount = 2
            };

            logicApi.CreateBalls(2, 800, 400);

            Assert.AreEqual(2, viewModel.Board.Balls.Count);
        }

        [TestMethod]
        public void StartCommand_ShouldBecomeDisabled_AfterStart()
        {
            FakeLogicApi logicApi = new FakeLogicApi();
            MainWindowViewModel viewModel = new MainWindowViewModel(logicApi);

            viewModel.StartCommand.Execute(null);

            Assert.IsFalse(viewModel.StartCommand.CanExecute(null));
            Assert.IsFalse(viewModel.CreateBallsCommand.CanExecute(null));
            Assert.IsTrue(viewModel.StopCommand.CanExecute(null));
        }

        [TestMethod]
        public void StopCommand_ShouldEnableStartAgain()
        {
            FakeLogicApi logicApi = new FakeLogicApi();
            MainWindowViewModel viewModel = new MainWindowViewModel(logicApi);

            viewModel.StartCommand.Execute(null);
            viewModel.StopCommand.Execute(null);

            Assert.IsTrue(viewModel.StartCommand.CanExecute(null));
            Assert.IsTrue(viewModel.CreateBallsCommand.CanExecute(null));
            Assert.IsFalse(viewModel.StopCommand.CanExecute(null));
        }

    }
}
