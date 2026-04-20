using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.Data.Abstractions;

namespace TP.ConcurrentProgramming.BusinessLogic.Abstractions
{
    public interface ILogicApi
    {
        IReadOnlyList<IBall> GetBalls();
        void CreateBalls(int count, double areaWidth, double areaHeight);
        void UpdateBalls(double areaWidth, double areaHeight);

        event Action? BallsUpdated; 
    
    }
}
