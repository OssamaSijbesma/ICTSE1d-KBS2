using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiaNotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiaNotes.UnitTests
{
    [TestClass]
    class UtilitiesTests
    {
        [TestMethod]
        public void DoubleToByteReturnsValue()
        {
            //Arange
            int doubleVal = 30;

            //Act
            var result = Utilities.DoubleToByte(doubleVal);

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DoubleToByteReturnsDouble()
        {
            //Arange
            double doubleVal = 30;

            //Act
            double result = Utilities.DoubleToByte(doubleVal);

            //Assert

            Assert.IsNotNull(result);
        }
    }
}
