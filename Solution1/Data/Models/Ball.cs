using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.Data.Abstractions;

namespace TP.ConcurrentProgramming.Data.Models
{
    public class Ball : IBall
    {
        public double Diameter { get;  }
        public double Mass { get; }
        public IPosition Position { get; }
        public IVector Velocity { get; }

        public Ball (double diameter,IPosition position, IVector velocity)
        {
            if (diameter <= 0)
            {
                throw new ArgumentException("Diameter must be >= 0", nameof(diameter));
            }

            Diameter = diameter;
            Position = position;
            Velocity = velocity;
            Mass = CalculateMass(diameter);
        }

        private static double CalculateMass(double diameter) 
        {
            return diameter * diameter;
        }
    }
}
