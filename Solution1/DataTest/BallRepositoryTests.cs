using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.Data.Abstractions;
using TP.ConcurrentProgramming.Data.Models;

namespace TP.ConcurrentProgramming.DataTest
{
    [TestClass]
    public class BallRepositoryTests
    {
        [TestMethod]
        public void Add_ShouldStoreBall()
        {
            IBallRepository repository = new BallRepository();
            IBall ball = new Ball(20, new Position(10, 15), new Vector(1, 2));

            repository.Add(ball);

            IReadOnlyList<IBall> balls = repository.GetAll();

            Assert.AreEqual(1, balls.Count);
            Assert.AreSame(ball, balls[0]);
        }

        [TestMethod]
        public void Clear_ShouldRemoveAllBalls()
        {
            IBallRepository repository = new BallRepository();

            repository.Add(new Ball(20, new Position(1, 1), new Vector(1, 1)));
            repository.Add(new Ball(20, new Position(2, 2), new Vector(2, 2)));

            repository.Clear();

            Assert.AreEqual(0, repository.GetAll().Count);
        }

        [TestMethod]
        public void GetAll_ShouldReturnReadOnlyList()
        {
            IBallRepository repository = new BallRepository();

            repository.Add(new Ball(20, new Position(1, 1), new Vector(1, 1)));

            IReadOnlyList<IBall> balls = repository.GetAll();

            Assert.AreEqual(1, balls.Count);
            Assert.IsInstanceOfType(balls, typeof(IReadOnlyList<IBall>));
        }

        [TestMethod]
        public void GetAll_ShouldReturnCopyOfCollection()
        {
            IBallRepository repository = new BallRepository();

            IBall firstBall = new Ball(20, new Position(1, 1), new Vector(1, 1));
            IBall secondBall = new Ball(20, new Position(2, 2), new Vector(2, 2));

            repository.Add(firstBall);

            IReadOnlyList<IBall> firstSnapshot = repository.GetAll();

            repository.Add(secondBall);

            IReadOnlyList<IBall> secondSnapshot = repository.GetAll();

            Assert.AreEqual(1, firstSnapshot.Count);
            Assert.AreEqual(2, secondSnapshot.Count);
        }

        [TestMethod]
        public void Repository_ShouldAllowConcurrentAddingBalls()
        {
            IBallRepository repository = new BallRepository();

            Parallel.For(0, 100, i =>
            {
                repository.Add(new Ball(20, new Position(i, i), new Vector(1, 1)));
            });

            Assert.AreEqual(100, repository.GetAll().Count);
        }

        [TestMethod]
        public void Repository_ShouldAllowConcurrentReadingAndAddingBalls()
        {
            IBallRepository repository = new BallRepository();

            Parallel.For(0, 100, i =>
            {
                repository.Add(new Ball(20, new Position(i, i), new Vector(1, 1)));
                IReadOnlyList<IBall> balls = repository.GetAll();

                Assert.IsTrue(balls.Count >= 0);
            });

            Assert.AreEqual(100, repository.GetAll().Count);
        }
    }
}