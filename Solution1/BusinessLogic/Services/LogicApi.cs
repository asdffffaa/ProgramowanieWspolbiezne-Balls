using System;
using System.Collections.Generic;
using TP.ConcurrentProgramming.BusinessLogic.Abstractions;
using TP.ConcurrentProgramming.Data.Abstractions;

namespace TP.ConcurrentProgramming.BusinessLogic.Services
{
    public class LogicApi : ILogicApi
    {
        private readonly IDataApi _dataApi;
        private readonly Random _random = new Random();
        private readonly object _lock = new object();

        public event Action? BallsUpdated;

        public LogicApi(IDataApi dataApi)
        {
            _dataApi = dataApi;
        }

        public IReadOnlyList<IBall> GetBalls()
        {
            lock (_lock)
            {
                return _dataApi.GetAll();
            }
        }

        public void CreateBalls(int count, double areaWidth, double areaHeight)
        {
            lock (_lock)
            {
                _dataApi.Clear();

                for (int i = 0; i < count; i++)
                {
                    double diameter = _random.Next(20, 50);

                    double x = _random.NextDouble() * (areaWidth - diameter);
                    double y = _random.NextDouble() * (areaHeight - diameter);

                    double vx;
                    double vy;

                    do
                    {
                        vx = _random.NextDouble() * 6 - 3;
                        vy = _random.NextDouble() * 6 - 3;
                    }
                    while (vx == 0 && vy == 0);

                    IBall ball = _dataApi.CreateBall(
                        diameter,
                        x,
                        y,
                        vx,
                        vy
                    );

                    _dataApi.Add(ball);
                }
            }

            BallsUpdated?.Invoke();
        }

        public void UpdateBalls(double areaWidth, double areaHeight)
        {
            lock (_lock)
            {
                foreach (IBall ball in _dataApi.GetAll())
                {
                    double newX = ball.Position.X + ball.Velocity.X;
                    double newY = ball.Position.Y + ball.Velocity.Y;

                    if (newX < 0)
                    {
                        newX = 0;
                        ball.Velocity.X = -ball.Velocity.X;
                    }
                    else if (newX + ball.Diameter > areaWidth)
                    {
                        newX = areaWidth - ball.Diameter;
                        ball.Velocity.X = -ball.Velocity.X;
                    }

                    if (newY < 0)
                    {
                        newY = 0;
                        ball.Velocity.Y = -ball.Velocity.Y;
                    }
                    else if (newY + ball.Diameter > areaHeight)
                    {
                        newY = areaHeight - ball.Diameter;
                        ball.Velocity.Y = -ball.Velocity.Y;
                    }

                    ball.Position.X = newX;
                    ball.Position.Y = newY;
                }

                ResolveBallCollisions();
            }

            BallsUpdated?.Invoke();
        }

        private void ResolveBallCollisions()
        {
            IReadOnlyList<IBall> balls = _dataApi.GetAll();

            for (int i = 0; i < balls.Count; i++)
            {
                for (int j = i + 1; j < balls.Count; j++)
                {
                    IBall firstBall = balls[i];
                    IBall secondBall = balls[j];

                    ResolveCollisionBetweenTwoBalls(firstBall, secondBall);
                }
            }
        }

        private void ResolveCollisionBetweenTwoBalls(IBall firstBall, IBall secondBall)
        {
            double firstRadius = firstBall.Diameter / 2.0;
            double secondRadius = secondBall.Diameter / 2.0;

            double firstCenterX = firstBall.Position.X + firstRadius;
            double firstCenterY = firstBall.Position.Y + firstRadius;

            double secondCenterX = secondBall.Position.X + secondRadius;
            double secondCenterY = secondBall.Position.Y + secondRadius;

            double deltaX = secondCenterX - firstCenterX;
            double deltaY = secondCenterY - firstCenterY;

            double distanceSquared = deltaX * deltaX + deltaY * deltaY;
            double minimumDistance = firstRadius + secondRadius;
            double minimumDistanceSquared = minimumDistance * minimumDistance;

            if (distanceSquared > minimumDistanceSquared)
            {
                return;
            }

            double distance = Math.Sqrt(distanceSquared);

            if (distance == 0)
            {
                distance = 0.01;
                deltaX = 0.01;
                deltaY = 0;
            }

            double normalX = deltaX / distance;
            double normalY = deltaY / distance;

            double relativeVelocityX = firstBall.Velocity.X - secondBall.Velocity.X;
            double relativeVelocityY = firstBall.Velocity.Y - secondBall.Velocity.Y;

            double velocityAlongNormal =
                relativeVelocityX * normalX +
                relativeVelocityY * normalY;

            if (velocityAlongNormal <= 0)
            {
                SeparateBalls(
                    firstBall,
                    secondBall,
                    normalX,
                    normalY,
                    minimumDistance - distance
                );

                return;
            }

            double firstMass = firstBall.Mass;
            double secondMass = secondBall.Mass;

            double impulse = (2.0 * velocityAlongNormal) / (firstMass + secondMass);

            firstBall.Velocity.X -= impulse * secondMass * normalX;
            firstBall.Velocity.Y -= impulse * secondMass * normalY;

            secondBall.Velocity.X += impulse * firstMass * normalX;
            secondBall.Velocity.Y += impulse * firstMass * normalY;

            SeparateBalls(
                firstBall,
                secondBall,
                normalX,
                normalY,
                minimumDistance - distance
            );
        }

        private void SeparateBalls(
            IBall firstBall,
            IBall secondBall,
            double normalX,
            double normalY,
            double overlap
        )
        {
            if (overlap <= 0)
            {
                return;
            }

            double firstMass = firstBall.Mass;
            double secondMass = secondBall.Mass;
            double totalMass = firstMass + secondMass;

            double firstMoveRatio = secondMass / totalMass;
            double secondMoveRatio = firstMass / totalMass;

            firstBall.Position.X -= normalX * overlap * firstMoveRatio;
            firstBall.Position.Y -= normalY * overlap * firstMoveRatio;

            secondBall.Position.X += normalX * overlap * secondMoveRatio;
            secondBall.Position.Y += normalY * overlap * secondMoveRatio;
        }
    }
}