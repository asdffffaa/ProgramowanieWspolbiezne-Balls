using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.Data.Abstractions
{
    public interface IPosition
    {
        double X { get; set; }
        double Y { get; set; }

    }
}
