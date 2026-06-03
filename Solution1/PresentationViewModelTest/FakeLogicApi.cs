using System;
using System.Collections.Generic;
using TP.ConcurrentProgramming.BusinessLogic.Abstractions;
using TP.ConcurrentProgramming.Data.Abstractions;
using TP.ConcurrentProgramming.Data.Models;

namespace TP.ConcurrentProgramming.PresentationViewModelTest
{
    internal class FakeLogicApi : ILogicApi
    {
        private readonly List<IBall> _balls = new List<IBall>();

        public event Action? BallsUpdated;

        public bool IsRunning { get; private set; }
        public int CreateBallsCallCount { get; private set; }
        public int UpdateBallsCallCount { get; private set; }
        public int StartSimulationCallCount { get; private set; }
        public int StopSimulationCallCount { get; private set; }

        public IReadOnlyList<IBall> GetBalls()
        {
            return _balls.AsReadOnly();
        }

        public void CreateBalls(int count, double areaWidth, double areaHeight)
        {
            CreateBallsCallCount++;
            _balls.Clear();

            for (int i = 0; i < count; i++)
            {
                _balls.Add(new Ball(20, new Position(10 * i, 10 * i), new Vector(1, 1)));
            }

            BallsUpdated?.Invoke();
        }

        public void UpdateBalls(double areaWidth, double areaHeight)
        {
            UpdateBallsCallCount++;

            foreach (IBall ball in _balls)
            {
                ball.Move(TimeSpan.FromSeconds(1));
            }

            BallsUpdated?.Invoke();
        }

        public void StartSimulation(double areaWidth, double areaHeight)
        {
            StartSimulationCallCount++;
            IsRunning = true;
        }

        public void StopSimulation()
        {
            StopSimulationCallCount++;
            IsRunning = false;
        }
    }
}
