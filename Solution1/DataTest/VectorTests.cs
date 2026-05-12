using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.Data.Models;


namespace TP.ConcurrentProgramming.DataTest
{
    [TestClass]
    public class VectorTests
    {
        [TestMethod]
        public void Vector_ShouldStoreAndUpdateCoordinates()
        {
            Vector vector = new Vector(1, 2);

            Assert.AreEqual(1, vector.X);
            Assert.AreEqual(2, vector.Y);

            vector.X = 3;
            vector.Y = 4;

            Assert.AreEqual(3, vector.X);
            Assert.AreEqual(4, vector.Y);






        }
    }
}
