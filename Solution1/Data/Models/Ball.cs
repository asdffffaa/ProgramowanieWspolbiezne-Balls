using System;
using System.Threading;
using TP.ConcurrentProgramming.Data.Abstractions;

namespace TP.ConcurrentProgramming.Data.Models
{
    public class Ball : IBall
    {
        private static int _lastId;
        private readonly object _syncRoot = new object();

        public Ball(double diameter, IPosition position, IVector velocity)
        {
            if (diameter <= 0)
            {
                throw new ArgumentException("Diameter must be >= 0", nameof(diameter));
            }

            Id = Interlocked.Increment(ref _lastId);
            Diameter = diameter;
            Position = position;
            Velocity = velocity;
            Mass = CalculateMass(diameter);
        }

        public int Id { get; }
        public object SyncRoot => _syncRoot;
        public double Diameter { get; }
        public double Mass { get; }
        public IPosition Position { get; }
        public IVector Velocity { get; }

        public BallSnapshot Snapshot()
        {
            lock (_syncRoot)
            {
                return new BallSnapshot(
                    Id,
                    Diameter,
                    Mass,
                    Position.X,
                    Position.Y,
                    Velocity.X,
                    Velocity.Y);
            }
        }

        public void Move(TimeSpan elapsedTime)
        {
            if (elapsedTime <= TimeSpan.Zero)
            {
                return;
            }

            lock (_syncRoot)
            {
                double seconds = elapsedTime.TotalSeconds;
                Position.X += Velocity.X * seconds;
                Position.Y += Velocity.Y * seconds;
            }
        }

        public void MoveBy(double deltaX, double deltaY)
        {
            lock (_syncRoot)
            {
                Position.X += deltaX;
                Position.Y += deltaY;
            }
        }

        public void SetPosition(double x, double y)
        {
            lock (_syncRoot)
            {
                Position.X = x;
                Position.Y = y;
            }
        }

        public void SetVelocity(double x, double y)
        {
            lock (_syncRoot)
            {
                Velocity.X = x;
                Velocity.Y = y;
            }
        }

        public void ChangeVelocity(double deltaX, double deltaY)
        {
            lock (_syncRoot)
            {
                Velocity.X += deltaX;
                Velocity.Y += deltaY;
            }
        }

        private static double CalculateMass(double diameter)
        {
            return diameter * diameter;
        }
    }
}
