using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.BusinessLogic.Services;
using TP.ConcurrentProgramming.Data.Abstractions;
using TP.ConcurrentProgramming.Data.Models;

namespace TP.ConcurrentProgramming.BusinessLogicTest
{
    [TestClass]
    public class LogicApiTests
    {
        private const double Delta = 0.0001;

        private static LogicApi CreateLogicApi(out IBallRepository repository)
        {
            repository = new BallRepository();
            IDataApi dataApi = new DataApi(repository);

            return new LogicApi(dataApi);
        }

        [TestMethod]
        public void CreateBalls_ShouldCreateRequestedNumberOfBalls()
        {
            LogicApi logicApi = CreateLogicApi(out _);

            logicApi.CreateBalls(5, 800, 400);

            Assert.AreEqual(5, logicApi.GetBalls().Count);
        }

        [TestMethod]
        public void CreateBalls_ShouldPlaceBallsInsideBoard()
        {
            LogicApi logicApi = CreateLogicApi(out _);

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
            LogicApi logicApi = CreateLogicApi(out _);
            bool raised = false;

            logicApi.BallsUpdated += () => raised = true;

            logicApi.CreateBalls(3, 800, 400);

            Assert.IsTrue(raised);
        }

        [TestMethod]
        public void UpdateBalls_ShouldMoveBallWhenNoCollision()
        {
            LogicApi logicApi = CreateLogicApi(out IBallRepository repository);

            IBall ball = new Ball(20, new Position(100, 100), new Vector(2, 3));
            repository.Add(ball);

            logicApi.UpdateBalls(800, 400);

            Assert.AreEqual(102, ball.Position.X, Delta);
            Assert.AreEqual(103, ball.Position.Y, Delta);
        }

        [TestMethod]
        public void UpdateBalls_ShouldBounceFromLeftWall()
        {
            LogicApi logicApi = CreateLogicApi(out IBallRepository repository);

            IBall ball = new Ball(20, new Position(1, 100), new Vector(-5, 0));
            repository.Add(ball);

            logicApi.UpdateBalls(800, 400);

            Assert.AreEqual(0, ball.Position.X, Delta);
            Assert.AreEqual(5, ball.Velocity.X, Delta);
        }

        [TestMethod]
        public void UpdateBalls_ShouldBounceFromRightWall()
        {
            LogicApi logicApi = CreateLogicApi(out IBallRepository repository);

            IBall ball = new Ball(20, new Position(779, 100), new Vector(5, 0));
            repository.Add(ball);

            logicApi.UpdateBalls(800, 400);

            Assert.AreEqual(780, ball.Position.X, Delta);
            Assert.AreEqual(-5, ball.Velocity.X, Delta);
        }

        [TestMethod]
        public void UpdateBalls_ShouldBounceFromTopWall()
        {
            LogicApi logicApi = CreateLogicApi(out IBallRepository repository);

            IBall ball = new Ball(20, new Position(100, 1), new Vector(0, -4));
            repository.Add(ball);

            logicApi.UpdateBalls(800, 400);

            Assert.AreEqual(0, ball.Position.Y, Delta);
            Assert.AreEqual(4, ball.Velocity.Y, Delta);
        }

        [TestMethod]
        public void UpdateBalls_ShouldBounceFromBottomWall()
        {
            LogicApi logicApi = CreateLogicApi(out IBallRepository repository);

            IBall ball = new Ball(20, new Position(100, 379), new Vector(0, 5));
            repository.Add(ball);

            logicApi.UpdateBalls(800, 400);

            Assert.AreEqual(380, ball.Position.Y, Delta);
            Assert.AreEqual(-5, ball.Velocity.Y, Delta);
        }

        [TestMethod]
        public void UpdateBalls_ShouldRaiseBallsUpdatedEvent()
        {
            LogicApi logicApi = CreateLogicApi(out IBallRepository repository);
            bool raised = false;

            logicApi.BallsUpdated += () => raised = true;

            repository.Add(new Ball(20, new Position(100, 100), new Vector(1, 1)));

            logicApi.UpdateBalls(800, 400);

            Assert.IsTrue(raised);
        }

        [TestMethod]
        public void UpdateBalls_ShouldChangeVelocitiesAfterBallCollision()
        {
            LogicApi logicApi = CreateLogicApi(out IBallRepository repository);

            IBall firstBall = new Ball(20, new Position(100, 100), new Vector(2, 0));
            IBall secondBall = new Ball(20, new Position(118, 100), new Vector(-2, 0));

            repository.Add(firstBall);
            repository.Add(secondBall);

            logicApi.UpdateBalls(800, 400);

            Assert.IsTrue(firstBall.Velocity.X < 0);
            Assert.IsTrue(secondBall.Velocity.X > 0);
        }

        [TestMethod]
        public void UpdateBalls_ShouldSeparateBallsAfterCollision()
        {
            LogicApi logicApi = CreateLogicApi(out IBallRepository repository);

            IBall firstBall = new Ball(20, new Position(100, 100), new Vector(2, 0));
            IBall secondBall = new Ball(20, new Position(118, 100), new Vector(-2, 0));

            repository.Add(firstBall);
            repository.Add(secondBall);

            logicApi.UpdateBalls(800, 400);

            double firstRadius = firstBall.Diameter / 2.0;
            double secondRadius = secondBall.Diameter / 2.0;

            double firstCenterX = firstBall.Position.X + firstRadius;
            double firstCenterY = firstBall.Position.Y + firstRadius;

            double secondCenterX = secondBall.Position.X + secondRadius;
            double secondCenterY = secondBall.Position.Y + secondRadius;

            double deltaX = secondCenterX - firstCenterX;
            double deltaY = secondCenterY - firstCenterY;

            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            double minimumDistance = firstRadius + secondRadius;

            Assert.IsTrue(distance >= minimumDistance - Delta);
        }

        [TestMethod]
        public void UpdateBalls_ShouldUseMassDuringCollision()
        {
            LogicApi logicApi = CreateLogicApi(out IBallRepository repository);

            IBall smallBall = new Ball(10, new Position(100, 100), new Vector(5, 0));
            IBall bigBall = new Ball(30, new Position(114, 90), new Vector(-1, 0));

            repository.Add(smallBall);
            repository.Add(bigBall);

            double smallVelocityBefore = smallBall.Velocity.X;
            double bigVelocityBefore = bigBall.Velocity.X;

            logicApi.UpdateBalls(800, 400);

            Assert.AreNotEqual(smallVelocityBefore, smallBall.Velocity.X);
            Assert.AreNotEqual(bigVelocityBefore, bigBall.Velocity.X);
            Assert.IsTrue(bigBall.Mass > smallBall.Mass);
        }

        [TestMethod]
        public void GetBalls_ShouldReturnBallsFromDataApi()
        {
            LogicApi logicApi = CreateLogicApi(out IBallRepository repository);

            IBall ball = new Ball(20, new Position(50, 60), new Vector(1, 1));
            repository.Add(ball);

            IReadOnlyList<IBall> balls = logicApi.GetBalls();

            Assert.AreEqual(1, balls.Count);
            Assert.AreSame(ball, balls[0]);
        }

        [TestMethod]
        public void UpdateBalls_ShouldUseElapsedTime()
        {
            LogicApi logicApi = CreateLogicApi(out IBallRepository repository);

            IBall ball = new Ball(20, new Position(100, 100), new Vector(10, -20));
            repository.Add(ball);

            logicApi.UpdateBalls(800, 400, TimeSpan.FromSeconds(0.5));

            Assert.AreEqual(105, ball.Position.X, Delta);
            Assert.AreEqual(90, ball.Position.Y, Delta);
        }

        [TestMethod]
        public async Task StartSimulation_ShouldMoveBallsInBackground()
        {
            LogicApi logicApi = CreateLogicApi(out IBallRepository repository);

            IBall ball = new Ball(20, new Position(100, 100), new Vector(120, 0));
            repository.Add(ball);

            logicApi.StartSimulation(800, 400);
            await Task.Delay(80);
            logicApi.StopSimulation();

            Assert.IsTrue(ball.Position.X > 100);
            Assert.IsFalse(logicApi.IsRunning);
        }
    }
}
