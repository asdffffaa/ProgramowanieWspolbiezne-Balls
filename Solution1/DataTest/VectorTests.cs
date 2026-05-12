using Microsoft.VisualStudio.TestTools.UnitTesting;
using TP.ConcurrentProgramming.Data.Models;

namespace TP.ConcurrentProgramming.DataTest
{
    [TestClass]
    public class VectorTests
    {
        [TestMethod]
        public void Vector_ShouldStoreConstructorValues()
        {
            Vector vector = new Vector(1, 2);

            Assert.AreEqual(1, vector.X);
            Assert.AreEqual(2, vector.Y);
        }

        [TestMethod]
        public void Vector_ShouldUpdateCoordinates()
        {
            Vector vector = new Vector(1, 2);

            vector.X = 3;
            vector.Y = 4;

            Assert.AreEqual(3, vector.X);
            Assert.AreEqual(4, vector.Y);
        }
    }
}