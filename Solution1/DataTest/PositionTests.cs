using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.Data.Models;

namespace TP.ConcurrentProgramming.DataTest
{
    [TestClass]
    public class PositionTests
    {
        [TestMethod]
        public void Position_ShouldStoreAndUpdateCoordinates()
        {
            Position position = new Position(10, 20);

            Assert.AreEqual(10, position.X);
            Assert.AreEqual(20, position.Y);

            position.X = 30;
            position.Y = 40;

            Assert.AreEqual(30, position.X);
            Assert.AreEqual(40, position.Y);
        
        }
    }
}
