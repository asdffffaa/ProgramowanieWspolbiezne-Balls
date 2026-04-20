using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.Data.Abstractions
{
    public interface IBallRepository
    {
        IReadOnlyList<IBall> GetAll();
        void Add(IBall ball);
        void Clear();
    }
}
