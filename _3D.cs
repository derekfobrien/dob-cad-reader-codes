using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADReader
{
    // this is a class to define a 3D point, of three double-precision values to
    // denote the X, Y and Z ordinates

    public class _3D
    {
        public double x;
        public double y;
        public double z;

        public string Publish()
        {
            // this method simply writes a string in the form (X, Y, Z)
            string MyString;

            MyString = string.Concat("(", x.ToString(), ", ", y.ToString(), ", ", z.ToString(), ")");

            return MyString;
        }
    }
}
