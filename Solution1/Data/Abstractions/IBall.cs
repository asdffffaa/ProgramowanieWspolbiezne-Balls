using System;

namespace TP.ConcurrentProgramming.Data.Abstractions
{
    public interface IBall
    {
        int Id { get; }
        object SyncRoot { get; }
        double Diameter { get; }
        double Mass { get; }
        IPosition Position { get; }
        IVector Velocity { get; }

        BallSnapshot Snapshot();
        void Move(TimeSpan elapsedTime);
        void MoveBy(double deltaX, double deltaY);
        void SetPosition(double x, double y);
        void SetVelocity(double x, double y);
        void ChangeVelocity(double deltaX, double deltaY);
    }
}
