using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.BusinessLogic.Abstractions;
using TP.ConcurrentProgramming.Data.Abstractions;
using TP.ConcurrentProgramming.Data.Models;

namespace TP.ConcurrentProgramming.BusinessLogic.Services
{
    public class LogicApi : ILogicApi
    {
        private readonly IBallRepository _ballRepository;
        private readonly Random _random = new Random();

        public event Action? BallsUpdated;

        public LogicApi(IBallRepository ballRepository)
        {
            _ballRepository = ballRepository;
        }

        public IReadOnlyList<IBall> GetBalls()
        {
            return _ballRepository.GetAll();
        }

        public void CreateBalls(int count, double areaWidth, double areaHeight)
        {
            _ballRepository.Clear();

            const double diameter = 20.0;

            for (int i = 0; i < count; i++)
            {
                double x = _random.NextDouble() * (areaWidth - diameter);
                double y = _random.NextDouble() * (areaHeight - diameter);


                double vx;
                double vy;

                do
                {
                    vx = _random.NextDouble() * 6 - 3;
                    vy = _random.NextDouble() * 6 - 3;
                } while (vx == 0 && vy == 0);

                IBall ball = new Ball(diameter, new Position(x, y), new Vector(vx, vy));

                _ballRepository.Add(ball);
            }

            BallsUpdated?.Invoke();

        }

        public void UpdateBalls(double areaWidth, double areaHeight)
        {
            foreach (IBall ball in _ballRepository.GetAll())
            {
                double newX = ball.Position.X + ball.Velocity.X;
                double newY = ball.Position.Y + ball.Velocity.Y;

                if (newX < 0)
                {
                    newX = 0;
                    // ball.Velocity.X = -ball.Velocity.X;
                }
                else if (newX + ball.Diameter > areaWidth)
                {
                    newX = areaWidth - ball.Diameter;
                    // ball.Velocity.X = -ball.Velocity.X;
                }

                if (newY < 0)
                {
                    newY = 0;
                    // ball.Velocity.Y = -ball.Velocity.Y;
                }

                else if(newY + ball.Diameter > areaHeight)
                {
                    newY = areaHeight - ball.Diameter;
                    // ball.Velocity.Y = -ball.Velocity.Y;
                }

                ball.Position.X = newX;
                ball.Position.Y = newY;
            }

            BallsUpdated?.Invoke();
        } 
        


    }
}
