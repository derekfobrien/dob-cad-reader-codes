using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CADReader
{
    public class Circle
    {
        public _3D Centre = new _3D();
        public double Radius;
        public double LeftX;
        public double RightX;
        public double TopY;
        public double BottomY;

        public string ReadFromDXF(StreamReader str, ListBox lb)
        {

            string GroupCode, Value, Coords;

            GroupCode = "Unused";
            Value = "Unassigned";

            lb.Items.Add("CIRCLE");

            while (GroupCode != "0")
            {
                // get the next code/value pair
                GroupCode = str.ReadLine().Trim();
                Value = str.ReadLine().Trim();
                switch (GroupCode)
                {
                    case "10": // x-ordinate of the centre point
                        Centre.x = Convert.ToDouble(Value);
                        break;
                    case "20": // y-ordinate of the centre point
                        Centre.y = Convert.ToDouble(Value);
                        break;
                    case "30": // z-ordinate of the centre point
                        Centre.z = Convert.ToDouble(Value);
                        break;
                    case "40": // radius
                        Radius = Convert.ToDouble(Value);
                        break;
                }

            }

            // write the circle's properties onto the list box
            Coords = string.Concat("Location: ", Centre.Publish());
            lb.Items.Add(Coords);
            Coords = string.Concat("Radius: ", Radius.ToString());
            lb.Items.Add(Coords);

            SetExtents();

            // this is the name of the next object to follow
            // it is the value when we get a group code of 0, thus exiting the while loop
            return Value;
        }

        public void SetExtents()
        {
            /* this method calculates the size of the 'box' to enclose the ellipse
             * it will then be used by the DrawingSet class to calculate the extents of all the
             * drawing objects as a unit, when arriving at values in the Viewing class, when displaying
             * the objects after they have been loaded from the file
             */

            LeftX = Centre.x - Radius;
            RightX = Centre.x + Radius;
            TopY = Centre.y + Radius;
            BottomY = Centre.y - Radius;
        
        }

        public void Draw(Graphics G, Viewing V, Pen P)
        {
            float XA, YA, Dia;

            XA = (float)(((LeftX - V.WorldCenX) * V.WorldtoScreen) + (V.ScreenWidth / 2));
            YA = (float)(((V.WorldCenY - TopY) * V.WorldtoScreen) + (V.ScreenHeight / 2));

            Dia = (float)(2 * Radius * V.WorldtoScreen);

            G.DrawEllipse(P, XA, YA, Dia, Dia);
        }

    }
}
