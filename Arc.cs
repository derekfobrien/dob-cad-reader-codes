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
    public class Arc
    {
        public _3D Centre = new _3D();
        public double Radius;
        public double StartAngle;
        public double EndAngle;
        public double LeftX;
        public double RightX;
        public double TopY;
        public double BottomY;

        public string ReadFromDXF(StreamReader str, ListBox lb)
        {

            string GroupCode, Value, MyStr;

            GroupCode = "Unused";
            Value = "Unassigned";

            lb.Items.Add("ARC");

            while (GroupCode != "0")
            {
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
                    case "50": // start angle in degrees
                        StartAngle = Convert.ToDouble(Value);
                        break;
                    case "51": // end angle in degrees
                        EndAngle = Convert.ToDouble(Value);
                        break;
                }

            }

            // write the arc's properties onto the list box
            MyStr = string.Concat("Location: ", Centre.Publish());
            lb.Items.Add(MyStr);
            MyStr = string.Concat("Radius: ", Radius.ToString());
            lb.Items.Add(MyStr);
            MyStr = string.Concat("Angle sweep: ", StartAngle.ToString(), " to ", EndAngle.ToString());
            lb.Items.Add(MyStr);

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

            double StartAngleRadians, EndAngleRadians, TempNumber;

            // convert angle attributes to radians
            StartAngleRadians = StartAngle * Math.PI / 180;
            EndAngleRadians = EndAngle * Math.PI / 180;

            // calculate the extent points based only
            // on the start and end points
            LeftX = Centre.x + (Radius * Math.Cos(StartAngleRadians));
            RightX = LeftX;
            TopY = Centre.y + (Radius * Math.Sin(StartAngleRadians));
            BottomY = TopY;

            // end point, left and right extents
            TempNumber = Centre.x + (Radius * Math.Cos(EndAngleRadians));

            if (TempNumber < LeftX)
                LeftX = TempNumber;
            if (TempNumber > RightX)
                RightX = TempNumber;

            // end point, top and bottom extents
            TempNumber = Centre.y + (Radius * Math.Sin(EndAngleRadians));

            if (TempNumber > TopY)
                TopY = TempNumber;
            if (TempNumber < BottomY)
                BottomY = TempNumber;

            // right extent is the right quadrant point
            // start angle > end angle
            if (StartAngle > EndAngle)
            {
                StartAngle = StartAngle - 360;
                RightX = Centre.x + Radius;
            }

            // top extent is the top quadrant point
            // start < 90 and end > 90
            if (StartAngle < 90 && EndAngle > 90)
                TopY = Centre.y + Radius;

            // left extent is the left quadrant point
            // start < 180 and end > 180
            if (StartAngle < 180 && EndAngle > 180)
                LeftX = Centre.x - Radius;

            // bottom extent is the bottom quadrant point
            // start < 270 and end > 270
            if (StartAngle < 270 && EndAngle > 270)
                BottomY = Centre.y - Radius;

        }

        public void Draw(Graphics G, Viewing V, Pen P)
        {
            int TopLeftX, TopLeftY, Dia, Theta1, Theta2; 

            TopLeftX = (int)(((Centre.x - Radius - V.WorldCenX) * V.WorldtoScreen) + (V.ScreenWidth / 2));
            TopLeftY = (int)(((V.WorldCenY - (Centre.y + Radius)) * V.WorldtoScreen) + (V.ScreenHeight / 2));

            Dia = (int)(2 * Radius * V.WorldtoScreen);

            /* you will notice that I had to put minuses in front of EndAngle and StartAngle because
             * the DrawArc method calls for the angle dimensions as measured clockwise from the positive
             * side of the x-axis, whereas AutoCAD uses the notation as anticlockwise from the positive
             * side of the x-axis
            */
            Theta1 = (int)(- EndAngle);
            Theta2 = (int)(- StartAngle);
            G.DrawArc(P, TopLeftX, TopLeftY, Dia, Dia, Theta1, Theta2 - Theta1);

            // the rest of the code was used for proving the method
            /* I wrote the rest of the lines in this method to test that arc object was working the way I want it
             * Once I was satisfied that it was, I commented them out
            // Draw extents box
            int XA, YA, XB, YB;

            XA = (int)(((LeftX - V.WorldCenX) * V.WorldtoScreen) + (V.ScreenWidth / 2));
            YA = (int)(((V.WorldCenY - TopY) * V.WorldtoScreen) + (V.ScreenHeight / 2));
            XB = (int)(((RightX - V.WorldCenX) * V.WorldtoScreen) + (V.ScreenWidth / 2));
            YB = (int)(((V.WorldCenY - BottomY) * V.WorldtoScreen) + (V.ScreenHeight / 2));

            G.DrawLine(P, XA, YA, XB, YA);
            G.DrawLine(P, XA, YB, XB, YB);
            G.DrawLine(P, XA, YA, XA, YB);
            G.DrawLine(P, XB, YA, XB, YB);

            // draw lines from centrepoint to start and end points
            double StartAngleRadians, EndAngleRadians;

            StartAngleRadians = StartAngle * Math.PI / 180;
            EndAngleRadians = EndAngle * Math.PI / 180;

            XA = (int)(((Centre.x - V.WorldCenX) * V.WorldtoScreen) + (V.ScreenWidth / 2));
            YA = (int)(((V.WorldCenY - Centre.y) * V.WorldtoScreen) + (V.ScreenHeight / 2));
            XB = (int)((((Centre.x + (Radius * Math.Cos(StartAngleRadians))) - V.WorldCenX) * V.WorldtoScreen) + (V.ScreenWidth / 2));
            YB = (int)(((V.WorldCenY - (Centre.y + (Radius * Math.Sin(StartAngleRadians)))) * V.WorldtoScreen) + (V.ScreenHeight / 2));

            G.DrawLine(P, XA, YA, XB, YB);

            XB = (int)((((Centre.x + (Radius * Math.Cos(EndAngleRadians))) - V.WorldCenX) * V.WorldtoScreen) + (V.ScreenWidth / 2));
            YB = (int)(((V.WorldCenY - (Centre.y + (Radius * Math.Sin(EndAngleRadians)))) * V.WorldtoScreen) + (V.ScreenHeight / 2));

            G.DrawLine(P, XA, YA, XB, YB);
            */
        }
    }
}
