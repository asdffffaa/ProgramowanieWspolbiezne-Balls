using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.BusinessLogic.Abstractions;
using TP.ConcurrentProgramming.Data.Abstractions;

namespace TP.ConcurrentProgramming.BusinessLogic.Services
{
    public class LogicApi : ILogicApi
    {
        private static readonly TimeSpan SimulationFrameTime = TimeSpan.FromMilliseconds(16);
        private static readonly TimeSpan OneLogicalStep = TimeSpan.FromSeconds(1);

        private readonly IDataApi _dataApi;
        private readonly Random _random = new Random();
        private readonly object _lock = new object();

        private System.Timers.Timer? _simulationTimer;
        private DateTime _previousSimulationTime;
        private double _simulationAreaWidth;
        private double _simulationAreaHeight;
        private int _timerCallbackInProgress;
        private bool _isRunning;

        public LogicApi(IDataApi dataApi)
        {
            _dataApi = dataApi;
        }

        public event Action? BallsUpdated;

        public bool IsRunning
        {
            get
            {
                lock (_lock)
                {
                    return _isRunning;
                }
            }
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
                        vx = _random.NextDouble() * 360 - 180;
                        vy = _random.NextDouble() * 360 - 180;
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

                _dataApi.SaveDiagnosticSnapshots();
            }

            BallsUpdated?.Invoke();
        }

        public void UpdateBalls(double areaWidth, double areaHeight)
        {
            UpdateBalls(areaWidth, areaHeight, OneLogicalStep);
        }

        public void UpdateBalls(double areaWidth, double areaHeight, TimeSpan elapsedTime)
        {
            if (elapsedTime <= TimeSpan.Zero)
            {
                return;
            }

            lock (_lock)
            {
                IReadOnlyList<IBall> balls = _dataApi.GetAll();

                Parallel.ForEach(balls, ball =>
                {
                    ball.Move(elapsedTime);
                });

                foreach (IBall ball in balls)
                {
                    KeepBallInsideArea(ball, areaWidth, areaHeight);
                }

                ResolveBallCollisions(balls);
                _dataApi.SaveDiagnosticSnapshots();
            }

            BallsUpdated?.Invoke();
        }

        public void StartSimulation(double areaWidth, double areaHeight)
        {
            lock (_lock)
            {
                if (_isRunning)
                {
                    return;
                }

                _isRunning = true;
                _simulationAreaWidth = areaWidth;
                _simulationAreaHeight = areaHeight;
                _previousSimulationTime = DateTime.UtcNow;
                _simulationTimer = new System.Timers.Timer(SimulationFrameTime.TotalMilliseconds);
                _simulationTimer.AutoReset = true;
                _simulationTimer.Elapsed += OnSimulationTimerElapsed;
                _simulationTimer.Start();
            }
        }

        public void StopSimulation()
        {
            System.Timers.Timer? timer;

            lock (_lock)
            {
                if (!_isRunning)
                {
                    return;
                }

                _isRunning = false;
                timer = _simulationTimer;
                _simulationTimer = null;
                _previousSimulationTime = default;
            }

            if (timer is not null)
            {
                timer.Stop();
                timer.Elapsed -= OnSimulationTimerElapsed;
                timer.Dispose();
            }
        }

        private void OnSimulationTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (Interlocked.Exchange(ref _timerCallbackInProgress, 1) == 1)
            {
                return;
            }

            try
            {
                TimeSpan elapsedTime;
                double areaWidth;
                double areaHeight;

                lock (_lock)
                {
                    if (!_isRunning)
                    {
                        return;
                    }

                    DateTime current = DateTime.UtcNow;
                    elapsedTime = current - _previousSimulationTime;
                    _previousSimulationTime = current;

                    areaWidth = _simulationAreaWidth;
                    areaHeight = _simulationAreaHeight;
                }

                UpdateBalls(areaWidth, areaHeight, elapsedTime);
            }
            finally
            {
                Interlocked.Exchange(ref _timerCallbackInProgress, 0);
            }
        }

        private static void KeepBallInsideArea(IBall ball, double areaWidth, double areaHeight)
        {
            lock (ball.SyncRoot)
            {
                BallSnapshot snapshot = ball.Snapshot();
                double newX = snapshot.PositionX;
                double newY = snapshot.PositionY;
                double newVelocityX = snapshot.VelocityX;
                double newVelocityY = snapshot.VelocityY;

                if (newX < 0)
                {
                    newX = 0;
                    newVelocityX = Math.Abs(newVelocityX);
                }
                else if (newX + snapshot.Diameter > areaWidth)
                {
                    newX = areaWidth - snapshot.Diameter;
                    newVelocityX = -Math.Abs(newVelocityX);
                }

                if (newY < 0)
                {
                    newY = 0;
                    newVelocityY = Math.Abs(newVelocityY);
                }
                else if (newY + snapshot.Diameter > areaHeight)
                {
                    newY = areaHeight - snapshot.Diameter;
                    newVelocityY = -Math.Abs(newVelocityY);
                }

                ball.SetPosition(newX, newY);
                ball.SetVelocity(newVelocityX, newVelocityY);
            }
        }

        private static void ResolveBallCollisions(IReadOnlyList<IBall> balls)
        {
            for (int i = 0; i < balls.Count; i++)
            {
                for (int j = i + 1; j < balls.Count; j++)
                {
                    ResolveCollisionBetweenTwoBalls(balls[i], balls[j]);
                }
            }
        }

        private static void ResolveCollisionBetweenTwoBalls(IBall firstBall, IBall secondBall)
        {
            IBall firstLock = firstBall.Id < secondBall.Id ? firstBall : secondBall;
            IBall secondLock = firstBall.Id < secondBall.Id ? secondBall : firstBall;

            lock (firstLock.SyncRoot)
            {
                lock (secondLock.SyncRoot)
                {
                    BallSnapshot firstSnapshot = firstBall.Snapshot();
                    BallSnapshot secondSnapshot = secondBall.Snapshot();

                    double firstRadius = firstSnapshot.Diameter / 2.0;
                    double secondRadius = secondSnapshot.Diameter / 2.0;

                    double firstCenterX = firstSnapshot.PositionX + firstRadius;
                    double firstCenterY = firstSnapshot.PositionY + firstRadius;

                    double secondCenterX = secondSnapshot.PositionX + secondRadius;
                    double secondCenterY = secondSnapshot.PositionY + secondRadius;

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

                    double relativeVelocityX = firstSnapshot.VelocityX - secondSnapshot.VelocityX;
                    double relativeVelocityY = firstSnapshot.VelocityY - secondSnapshot.VelocityY;

                    double velocityAlongNormal = relativeVelocityX * normalX + relativeVelocityY * normalY;
                    double overlap = minimumDistance - distance;

                    if (velocityAlongNormal > 0)
                    {
                        double impulse = (2.0 * velocityAlongNormal) / (firstSnapshot.Mass + secondSnapshot.Mass);

                        firstBall.ChangeVelocity(
                            -impulse * secondSnapshot.Mass * normalX,
                            -impulse * secondSnapshot.Mass * normalY);

                        secondBall.ChangeVelocity(
                            impulse * firstSnapshot.Mass * normalX,
                            impulse * firstSnapshot.Mass * normalY);
                    }

                    SeparateBalls(
                        firstBall,
                        secondBall,
                        normalX,
                        normalY,
                        overlap,
                        firstSnapshot.Mass,
                        secondSnapshot.Mass);
                }
            }
        }

        private static void SeparateBalls(
            IBall firstBall,
            IBall secondBall,
            double normalX,
            double normalY,
            double overlap,
            double firstMass,
            double secondMass)
        {
            if (overlap <= 0)
            {
                return;
            }

            double totalMass = firstMass + secondMass;
            double firstMoveRatio = secondMass / totalMass;
            double secondMoveRatio = firstMass / totalMass;

            firstBall.MoveBy(
                -normalX * overlap * firstMoveRatio,
                -normalY * overlap * firstMoveRatio);

            secondBall.MoveBy(
                normalX * overlap * secondMoveRatio,
                normalY * overlap * secondMoveRatio);
        }
    }
}
