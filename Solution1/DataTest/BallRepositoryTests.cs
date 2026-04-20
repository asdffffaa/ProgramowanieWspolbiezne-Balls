using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            Assert.AreEqual(1, repository.GetAll().Count);
            Assert.AreSame(ball, repository.GetAll()[0]);
            
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
            var balls = repository.GetAll();

            Assert.AreEqual(1, balls.Count);
        }
    }
}
