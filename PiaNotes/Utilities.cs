using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiaNotes
{
    static class Utilities
    {
        // Method for converting a double to byte.
        public static byte DoubleToByte(double doubleVal)
        {
            byte byteVal = 0;

            // Double to byte conversion can overflow.
            try
            {
                byteVal = Convert.ToByte(doubleVal);
                return byteVal;
            }
            catch (OverflowException)
            {
                //If it doesn't work, display the error message in the debug console
                System.Diagnostics.Debug.WriteLine("Overflow in double-to-byte conversion.");
            }

            // Byte to double conversion cannot overflow.
            doubleVal = System.Convert.ToDouble(byteVal);
            return byteVal;
        }
    }
}
