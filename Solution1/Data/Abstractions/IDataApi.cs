using System.Collections.Generic;

namespace TP.ConcurrentProgramming.Data.Abstractions
{
    public interface IDataApi
    {
        IReadOnlyList<IBall> GetAll();

        void Add(IBall ball);

        void Clear();

        IBall CreateBall(
            double diameter,
            double positionX,
            double positionY,
            double velocityX,
            double velocityY
        );
    }
}