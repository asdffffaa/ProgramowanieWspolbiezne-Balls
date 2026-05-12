using System.Collections.Generic;
using TP.ConcurrentProgramming.Data.Abstractions;

namespace TP.ConcurrentProgramming.Data.Models
{
    public class DataApi : IDataApi
    {
        private readonly IBallRepository _ballRepository;

        public DataApi(IBallRepository ballRepository)
        {
            _ballRepository = ballRepository;
        }

        public IReadOnlyList<IBall> GetAll()
        {
            return _ballRepository.GetAll();
        }

        public void Add(IBall ball)
        {
            _ballRepository.Add(ball);
        }

        public void Clear()
        {
            _ballRepository.Clear();
        }

        public IBall CreateBall(
            double diameter,
            double positionX,
            double positionY,
            double velocityX,
            double velocityY
        )
        {
            return new Ball(
                diameter,
                new Position(positionX, positionY),
                new Vector(velocityX, velocityY)
            );
        }
    }
}