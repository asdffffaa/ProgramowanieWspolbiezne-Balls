using System.Collections.Generic;

namespace TP.ConcurrentProgramming.Data.Abstractions
{
    public interface IDataApi
    {
        long DroppedDiagnosticRecords { get; }

        IReadOnlyList<IBall> GetAll();

        IReadOnlyList<BallSnapshot> GetSnapshots();

        void Add(IBall ball);

        void Clear();

        void SaveDiagnosticSnapshots();

        IBall CreateBall(
            double diameter,
            double positionX,
            double positionY,
            double velocityX,
            double velocityY
        );
    }
}
