using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.Data.Abstractions;

namespace TP.ConcurrentProgramming.Data.Models
{
    public class Ball : IBall
    {
        public double Diameter { get;  }
        public IPosition Position { get; }
        public IVector Velocity { get; }

        public Ball (double diameter, IPosition position, IVector velocity)
        {
            Diameter = diameter;
            Position = position;
            Velocity = velocity;
        }
    }
}
