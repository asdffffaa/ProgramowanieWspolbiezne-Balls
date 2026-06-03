using System;
using System.Collections.Generic;
using TP.ConcurrentProgramming.Data.Abstractions;

namespace TP.ConcurrentProgramming.BusinessLogic.Abstractions
{
    public interface ILogicApi
    {
        bool IsRunning { get; }

        IReadOnlyList<IBall> GetBalls();
        void CreateBalls(int count, double areaWidth, double areaHeight);
        void UpdateBalls(double areaWidth, double areaHeight);
        void StartSimulation(double areaWidth, double areaHeight);
        void StopSimulation();

        event Action? BallsUpdated;
    }
}
