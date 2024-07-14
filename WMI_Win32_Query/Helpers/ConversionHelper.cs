using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMI_Win32_Query.Helpers
{
    public static class ConversionHelper
    {
        #region Conversion Functions
        #region Conversion Variables
        //Constants for conveersions of different byt sizes
        private const float FLOAT_GIG_CONVERSION = 1073741824f; //Holds the float conversion number of GB per bit
        private const float FLOAT_TERA_CONVERSION = 0.0009765625F;//Holds the float conversion number for TB per bit
        #endregion

        /// <summary>
        /// Converts a provided Megabyte float to Gigabytes
        /// </summary>
        /// <param name="conversionNum"></param>
        /// <returns>float</returns>
        public static float ConversionToGig(float conversionNum)
        {
            //Pre: Needs conversionNum to be initialized
            //Post: Returns gigConversion number to the program
            //Purpose: To convert the bytes number that is incoming to gigabytes

            //Set the gigConversion to 0
            float gigConversion;
            //Grabs the conversionNum from the one passed into the function then 
            //divides by the Float_GIG_CONVERSION Constant
            gigConversion = conversionNum / FLOAT_GIG_CONVERSION;

            return gigConversion; //Returns the variable gigConversion
        }//End ConversionToGig

        /// <summary>
        /// Converts a provided Gigabyte float to Terabytes
        /// </summary>
        /// <param name="ConversionNum"></param>
        /// <returns>float</returns>
        public static float ConversionToTer(float ConversionNum)
        {
            //Pre: Needs conversionNum to be initialized
            //Post: Returns teraConversion number to the program
            //Purpose: To convert the bytes number that is incoming to terabytes

            //Set the teraConversion to 0
            float teraConversion;
            //Grabs the conversionNum from the one passed into the function then 
            //divides by the Float_TERA_CONVERSION Constant
            teraConversion = ConversionNum / FLOAT_TERA_CONVERSION;

            return teraConversion;
        }//End ConversionToTer
        #endregion
    }
}
