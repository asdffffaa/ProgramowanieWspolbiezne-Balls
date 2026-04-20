using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.PresentationModel.Models;

namespace TP.ConcurrentProgramming.PresentationModelTest
{
    [TestClass]
    public class BallPresentationModelTests
    {

        [TestMethod]
        public void X_Set_ShouldUpdateValue()
        {
            BallPresentationModel model = new BallPresentationModel();

            model.X = 15;

            Assert.AreEqual(15, model.X);
        }

        [TestMethod]
        public void Y_Set_ShouldUpdateValue()
        {
            BallPresentationModel model = new BallPresentationModel();

            model.Y = 25;

            Assert.AreEqual(25, model.Y);
        }

        [TestMethod]
        public void Diameter_Set_ShouldUpdateValue()
        {
            BallPresentationModel model = new BallPresentationModel();

            model.Diameter = 30;

            Assert.AreEqual(30, model.Diameter);
        }

    }
}
