using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.BusinessLogic.Abstractions;
using TP.ConcurrentProgramming.Data.Abstractions;
using TP.ConcurrentProgramming.Data.Models;

namespace TP.ConcurrentProgramming.PresentationViewModelTest
{
    internal class FakeLogicApi : ILogicApi
    {
        private readonly List<IBall> _balls = new List<IBall>();

        public event Action? BallsUpdated;

        public int CreateBallsCallCount { get; private set; }
        public int UpdateBallsCallCount { get; private set; }

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

            foreach (var ball in _balls)
            {
                ball.Position.X += ball.Velocity.X;
                ball.Position.Y += ball.Velocity.Y;
            }

            BallsUpdated?.Invoke();
        }
    }

}
