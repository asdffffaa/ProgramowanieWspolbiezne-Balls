using System;

namespace TP.ConcurrentProgramming.Data.Abstractions
{
    public readonly struct BallSnapshot
    {
        public BallSnapshot(
            int id,
            double diameter,
            double mass,
            double positionX,
            double positionY,
            double velocityX,
            double velocityY)
        {
            Id = id;
            Diameter = diameter;
            Mass = mass;
            PositionX = positionX;
            PositionY = positionY;
            VelocityX = velocityX;
            VelocityY = velocityY;
        }

        public int Id { get; }
        public double Diameter { get; }
        public double Mass { get; }
        public double PositionX { get; }
        public double PositionY { get; }
        public double VelocityX { get; }
        public double VelocityY { get; }

        public BallDiagnosticRecord ToDiagnosticRecord(DateTimeOffset timestamp)
        {
            return new BallDiagnosticRecord(
                timestamp,
                Id,
                PositionX,
                PositionY,
                VelocityX,
                VelocityY);
        }
    }
}
