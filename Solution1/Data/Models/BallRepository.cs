using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.Data.Abstractions;

namespace TP.ConcurrentProgramming.Data.Models
{
    public class BallRepository : IBallRepository
    {
        private readonly List<IBall> _balls = new List<IBall>(); // можно использовать только внутри BallRepository
        // _balls = new List<IBall>();

        public IReadOnlyList<IBall> GetAll()
        {
            return _balls.AsReadOnly();
        }

        public void Add(IBall ball)
        {
            _balls.Add(ball);
        }

        public void Clear()
        {
            _balls.Clear();
        }

    }
}
