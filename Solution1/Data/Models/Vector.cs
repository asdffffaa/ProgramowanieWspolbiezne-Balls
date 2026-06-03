using TP.ConcurrentProgramming.Data.Abstractions;

namespace TP.ConcurrentProgramming.Data.Models
{
    public class Vector : IVector
    {
        private readonly object _lock = new object();
        private double _x;
        private double _y;

        public Vector(double x, double y)
        {
            _x = x;
            _y = y;
        }

        public double X
        {
            get
            {
                lock (_lock)
                {
                    return _x;
                }
            }
            set
            {
                lock (_lock)
                {
                    _x = value;
                }
            }
        }

        public double Y
        {
            get
            {
                lock (_lock)
                {
                    return _y;
                }
            }
            set
            {
                lock (_lock)
                {
                    _y = value;
                }
            }
        }
    }
}
