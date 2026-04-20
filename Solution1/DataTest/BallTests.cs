using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            Assert.Equals(20, ball.Diameter);
            Assert.Equals(position, ball.Position);
            Assert.Equals(vector, ball.Velocity);
        }
    }
}
