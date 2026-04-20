using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.Data.Abstractions;
using TP.ConcurrentProgramming.BusinessLogic.Services;
using TP.ConcurrentProgramming.Data.Models;

namespace TP.ConcurrentProgramming.BusinessLogicTest
{
    [TestClass]
    public class LogicApiTests
    {

        [TestMethod]
        public void CreateBalls_ShouldCreateRequstedNumberOfBalls()
        {
            IBallRepository repository = new BallRepository();
            
            LogicApi logicApi = new LogicApi(repository);

            logicApi.CreateBalls(5, 800, 400);

            Assert.AreEqual(5, logicApi.GetBalls().Count);
        }

        [TestMethod]
        public void CreateBalls_ShouldPlaceBallsInsideBoard()
        {
            IBallRepository repository = new BallRepository();
            LogicApi logicApi = new LogicApi(repository);

            logicApi.CreateBalls(10, 800, 400);

            foreach (IBall ball in logicApi.GetBalls())
            {
                Assert.IsTrue(ball.Position.X >= 0);
                Assert.IsTrue(ball.Position.Y >= 0);
                Assert.IsTrue(ball.Position.X + ball.Diameter <= 800);
                Assert.IsTrue(ball.Position.Y + ball.Diameter <= 400);
            }
        }

        [TestMethod]
        public void CreateBalls_ShouldRaiseBallsUpdatedEvent()
        {
            IBallRepository repository = new BallRepository();
            LogicApi logicApi = new LogicApi(repository);
            bool raised = false;

            logicApi.BallsUpdated += () => raised = true;

            logicApi.CreateBalls(3, 800, 400);

            Assert.IsTrue(raised);
        }

        [TestMethod]
        public void UpdateBalls_ShouldMoveBallWhenNoCollision()
        {
            IBallRepository repository = new BallRepository();
            LogicApi logicApi = new LogicApi(repository);

            IBall ball = new Ball(20, new Position(100, 100), new Vector(2, 3));
            repository.Add(ball);

            logicApi.UpdateBalls(800, 400);

            Assert.AreEqual(102, ball.Position.X);
            Assert.AreEqual(103, ball.Position.Y);
        }

        [TestMethod]
        public void UpdateBalls_ShouldBounceFromLeftWall()
        {
            IBallRepository repository = new BallRepository();
            LogicApi logicApi = new LogicApi(repository);

            IBall ball = new Ball(20, new Position(1, 100), new Vector(-5, 0));
            repository.Add(ball);

            logicApi.UpdateBalls(800, 400);

            Assert.AreEqual(0, ball.Position.X);
            Assert.AreEqual(5, ball.Velocity.X);
        }

        [TestMethod]
        public void UpdateBalls_ShouldBounceFromRightWall()
        {
            IBallRepository repository = new BallRepository();
            LogicApi logicApi = new LogicApi(repository);

            IBall ball = new Ball(20, new Position(779, 100), new Vector(5, 0));
            repository.Add(ball);

            logicApi.UpdateBalls(800, 400);

            Assert.AreEqual(780, ball.Position.X);
            Assert.AreEqual(-5, ball.Velocity.X);
        }

        [TestMethod]
        public void UpdateBalls_ShouldBounceFromTopWall()
        {
            IBallRepository repository = new BallRepository();
            LogicApi logicApi = new LogicApi(repository);

            IBall ball = new Ball(20, new Position(100, 1), new Vector(0, -4));
            repository.Add(ball);

            logicApi.UpdateBalls(800, 400);

            Assert.AreEqual(0, ball.Position.Y);
            Assert.AreEqual(4, ball.Velocity.Y);
        }

        [TestMethod]
        public void UpdateBalls_ShouldBounceFromBottomWall()
        {
            IBallRepository repository = new BallRepository();
            LogicApi logicApi = new LogicApi(repository);

            IBall ball = new Ball(20, new Position(100, 379), new Vector(0, 5));
            repository.Add(ball);

            logicApi.UpdateBalls(800, 400);

            Assert.AreEqual(380, ball.Position.Y);
            Assert.AreEqual(-5, ball.Velocity.Y);
        }

        [TestMethod]
        public void UpdateBalls_ShouldRaiseBallsUpdatedEvent()
        {
            IBallRepository repository = new BallRepository();
            LogicApi logicApi = new LogicApi(repository);
            bool raised = false;

            logicApi.BallsUpdated += () => raised = true;

            repository.Add(new Ball(20, new Position(100, 100), new Vector(1, 1)));
            logicApi.UpdateBalls(800, 400);

            Assert.IsTrue(raised);
        }

    }
}
