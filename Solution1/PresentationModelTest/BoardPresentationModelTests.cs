using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.Data.Abstractions;
using TP.ConcurrentProgramming.Data.Models;
using TP.ConcurrentProgramming.PresentationModel.Models;

namespace TP.ConcurrentProgramming.PresentationModelTest
{
    [TestClass]
    public class BoardPresentationModelTests
    {
        [TestMethod]
        public void UpdateFromLogic_ShouldAddMissingPresentationBalls()
        {
            BoardPresentationModel board = new BoardPresentationModel();
            IReadOnlyList<IBall> logicBalls = new List<IBall>
            {
                new Ball(20, new Position(10, 20), new Vector(1, 1)),
                new Ball(20, new Position(30, 40), new Vector(1, 1))
            };

            board.UpdateFromLogic(logicBalls, 800, 400, 800, 400);

            Assert.AreEqual(2, board.Balls.Count);
        }

        [TestMethod]
        public void UpdateFromLogic_ShouldRemoveExtraPresentationBalls()
        {
            BoardPresentationModel board = new BoardPresentationModel();

            board.UpdateFromLogic(new List<IBall>
            {
                new Ball(20, new Position(10, 20), new Vector(1, 1)),
                new Ball(20, new Position(30, 40), new Vector(1, 1))
            }, 800, 400, 800, 400);

            board.UpdateFromLogic(new List<IBall>
            {
                new Ball(20, new Position(50, 60), new Vector(1, 1))
            }, 800, 400, 800, 400);

            Assert.AreEqual(1, board.Balls.Count);
        }

        [TestMethod]
        public void UpdateFromLogic_ShouldCopyCoordinatesWithoutScaling_WhenBoardsAreEqual()
        {
            BoardPresentationModel board = new BoardPresentationModel();
            IReadOnlyList<IBall> logicBalls = new List<IBall>
            {
                new Ball(20, new Position(100, 50), new Vector(1, 1))
            };

            board.UpdateFromLogic(logicBalls, 800, 400, 800, 400);

            Assert.AreEqual(100, board.Balls[0].X);
            Assert.AreEqual(50, board.Balls[0].Y);
            Assert.AreEqual(20, board.Balls[0].Diameter);
        }

        [TestMethod]
        public void UpdateFromLogic_ShouldScaleCoordinatesAndDiameter()
        {
            BoardPresentationModel board = new BoardPresentationModel();
            IReadOnlyList<IBall> logicBalls = new List<IBall>
            {
                new Ball(20, new Position(400, 200), new Vector(1, 1))
            };

            board.UpdateFromLogic(logicBalls, 800, 400, 400, 200);

            Assert.AreEqual(200, board.Balls[0].X);
            Assert.AreEqual(100, board.Balls[0].Y);
            Assert.AreEqual(10, board.Balls[0].Diameter);
        }

    }
}
