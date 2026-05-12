using Microsoft.VisualStudio.TestTools.UnitTesting;
using TP.ConcurrentProgramming.Data.Models;

namespace TP.ConcurrentProgramming.DataTest
{
    [TestClass]
    public class PositionTests
    {
        [TestMethod]
        public void Position_ShouldStoreConstructorValues()
        {
            Position position = new Position(10, 20);

            Assert.AreEqual(10, position.X);
            Assert.AreEqual(20, position.Y);
        }

        [TestMethod]
        public void Position_ShouldUpdateCoordinates()
        {
            Position position = new Position(10, 20);

            position.X = 30;
            position.Y = 40;

            Assert.AreEqual(30, position.X);
            Assert.AreEqual(40, position.Y);
        }
    }
}