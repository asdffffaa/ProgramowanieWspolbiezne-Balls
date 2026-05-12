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
        private readonly object _lock = new object();

        public IReadOnlyList<IBall> GetAll()
        {
            lock (_lock)
            {
                return _balls.ToList().AsReadOnly();
            }
        }

        public void Add(IBall ball)
        {
            lock(_lock)
            {
                _balls.Add(ball);
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _balls.Clear();
            }
        }


    }
}
