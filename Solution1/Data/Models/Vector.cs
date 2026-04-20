using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.Data.Abstractions;

namespace TP.ConcurrentProgramming.Data.Models
{
    // double a = vector.X; - get
    // vector.x = 5; - set 
    public class Vector : IVector
    {
        public double X { get; set; } 
        public double Y { get; set; } 

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
