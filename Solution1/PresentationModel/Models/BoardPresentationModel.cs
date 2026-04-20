using System.Collections.Generic;
using System.Collections.ObjectModel;
using TP.ConcurrentProgramming.Data.Abstractions;

namespace TP.ConcurrentProgramming.PresentationModel.Models
{
    public class BoardPresentationModel
    {
        public ObservableCollection<BallPresentationModel> Balls { get; } = new ObservableCollection<BallPresentationModel>();

        public void UpdateFromLogic(
                    IReadOnlyList<IBall> logicBalls,
                    double logicBoardWidth,
                    double logicBoardHeight,
                    double viewBoardWidth,
                    double viewBoardHeight)
        {
            double scaleX = viewBoardWidth / logicBoardWidth;
            double scaleY = viewBoardHeight / logicBoardHeight;
            double diameterScale = scaleX < scaleY ? scaleX : scaleY;

            while (Balls.Count < logicBalls.Count)
            {
                Balls.Add(new BallPresentationModel());
            }

            while (Balls.Count > logicBalls.Count)
            {
                Balls.RemoveAt(Balls.Count - 1);
            }

            for (int i = 0; i < logicBalls.Count; i++)
            {
                Balls[i].X = logicBalls[i].Position.X * scaleX;
                Balls[i].Y = logicBalls[i].Position.Y * scaleY;
                Balls[i].Diameter = logicBalls[i].Diameter * diameterScale;
            }
        }
    }
}