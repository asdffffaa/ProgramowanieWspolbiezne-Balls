using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TP.ConcurrentProgramming.Data.Models;

namespace TP.ConcurrentProgramming.DataTest
{
    [TestClass]
    public class BallTests
    {
        [TestMethod]
        public void Ball_ShouldStoreConstructorValues()
        {
            Position position = new Position(5, 7);
            Vector vector = new Vector(2, 3);

            Ball ball = new Ball(20, position, vector);

            Assert.AreEqual(20, ball.Diameter);
            Assert.AreSame(position, ball.Position);
            Assert.AreSame(vector, ball.Velocity);
        }

        [TestMethod]
        public void Ball_ShouldCalculateMassFromDiameter()
        {
            Position position = new Position(5, 7);
            Vector vector = new Vector(2, 3);

            Ball ball = new Ball(20, position, vector);

            Assert.AreEqual(400, ball.Mass);
        }

        [TestMethod]
        public void Ball_ShouldThrowExceptionWhenDiameterIsZero()
        {
            Position position = new Position(5, 7);
            Vector vector = new Vector(2, 3);

            Assert.ThrowsException<ArgumentException>(() =>
            {
                new Ball(0, position, vector);
            });
        }

        [TestMethod]
        public void Ball_ShouldThrowExceptionWhenDiameterIsNegative()
        {
            Position position = new Position(5, 7);
            Vector vector = new Vector(2, 3);

            Assert.ThrowsException<ArgumentException>(() =>
            {
                new Ball(-10, position, vector);
            });
        }

        [TestMethod]
        public void Move_ShouldUseElapsedTime()
        {
            Ball ball = new Ball(20, new Position(10, 15), new Vector(4, -2));

            ball.Move(TimeSpan.FromSeconds(0.5));

            Assert.AreEqual(12, ball.Position.X);
            Assert.AreEqual(14, ball.Position.Y);
        }

        [TestMethod]
        public void Snapshot_ShouldCopyPositionAndVelocityUnderBallLock()
        {
            Ball ball = new Ball(20, new Position(10, 15), new Vector(4, -2));

            TP.ConcurrentProgramming.Data.Abstractions.BallSnapshot snapshot = ball.Snapshot();

            Assert.AreEqual(ball.Id, snapshot.Id);
            Assert.AreEqual(10, snapshot.PositionX);
            Assert.AreEqual(15, snapshot.PositionY);
            Assert.AreEqual(4, snapshot.VelocityX);
            Assert.AreEqual(-2, snapshot.VelocityY);
        }
    }
}
