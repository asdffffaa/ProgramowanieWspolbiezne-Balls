using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.Data.Abstractions;

namespace TP.ConcurrentProgramming.Data.Models
{
    public class Position : IPosition
    {
        public double X { get; set; }
        public double Y { get; set; }


        public Position(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
