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
    public class Ellipse
    {
        public _3D Centre = new _3D();
        public _3D MajorAxisEndpt = new _3D();
        public double MajorRadius;
        public double MinorRadius;
        public double MinorMajorRatio;
        public double Angle;
        private double AngleDeg;
        public double StartParameter;
        public double EndParameter;
        public double LeftX;
        public double RightX;
        public double TopY;
        public double BottomY;

        private void SetRadiiAndAngle()
        {
            double DX, DY, DZ;

            DX = MajorAxisEndpt.x * MajorAxisEndpt.x;
            DY = MajorAxisEndpt.y * MajorAxisEndpt.y;
            DZ = MajorAxisEndpt.z * MajorAxisEndpt.z;

            //get the major and minor radii
            MajorRadius = Math.Sqrt(DX + DY + DZ);

            MinorRadius = MajorRadius * MinorMajorRatio;

            // get the angle
            if (DY >= 0) // above the x-axis
                Angle = Math.Atan2(DY, DX);
            else // below the x-axis
                Angle = (2 * Math.PI) + Math.Atan2(DY, DX);

            AngleDeg = Angle * 180 / Math.PI;
        }

        public string ReadFromDXF(StreamReader str, ListBox lb)
        {

            string GroupCode, Value, MyStr;

            GroupCode = "Unused";
            Value = "Unassigned";

            lb.Items.Add("ELLIPSE");

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
                    case "11": // z-ordinate of the major axis endpoint
                        MajorAxisEndpt.x = Convert.ToDouble(Value);
                        break;
                    case "21": // z-ordinate of the major axis endpoint
                        MajorAxisEndpt.y = Convert.ToDouble(Value);
                        break;
                    case "31": // z-ordinate of the major axis endpoint
                        MajorAxisEndpt.z = Convert.ToDouble(Value);
                        break;
                    case "40": // ratio of minor axis to major axis
                        MinorMajorRatio = Convert.ToDouble(Value);
                        break;
                    case "41": // start angle in degrees
                        StartParameter = Convert.ToDouble(Value);
                        break;
                    case "42": // end angle in degrees
                        EndParameter = Convert.ToDouble(Value);
                        break;
                }
            }

            SetRadiiAndAngle();
            SetExtents();

            // write the ellipse's properties onto the listbox
            MyStr = string.Concat("Location: ", Centre.Publish());
            lb.Items.Add(MyStr);
            MyStr = string.Concat("Major Radius: ", MajorRadius.ToString());
            lb.Items.Add(MyStr);
            MyStr = string.Concat("Minor Radius: ", MinorRadius.ToString());
            lb.Items.Add(MyStr);
            MyStr = string.Concat("Angle: ", AngleDeg.ToString("F3"), "degrees");
            lb.Items.Add(MyStr);

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
            double LeftXFull, RightXFull, TopYFull, BottomYFull; // extents of the full ellipse
            double SinX, CosX; // sin and cosine of the angle
            double A, B; // Major and Minor radii
            double Prog1, Prog2, T1, T2; // a variable placeholder to enable step by step calculation

            SinX = Math.Sin(Angle);
            CosX = Math.Cos(Angle);

            A = MajorRadius;
            B = MinorRadius;

            // Get the left and right extents

            Prog1 = (A * A * CosX * CosX) / ((B * B * SinX * SinX) + (A * A * CosX * CosX));
            Prog2 = Math.Sqrt(Prog1); // square root

            T1 = Math.Acos(Prog2); // cos inverse - angle where one of the extents lies
            T2 = Math.Cos(T1);
            if (B * SinX != - A * CosX * Math.Sqrt(1 - (T2 * T2))/Math.Cos(T1))
            {
                Prog2 = - Math.Sqrt(Prog1);
                T1 = Math.Acos(Prog2);
            }

            T2 = Math.PI + T1; // the second angle is the first angle plus 180 degrees

            LeftXFull = Centre.x + (A * CosX * Math.Cos(T1)) - (B * SinX * Math.Sin(T1));
            RightXFull = Centre.x + (A * CosX * Math.Cos(T2)) - (B * SinX * Math.Sin(T2));

            if (LeftXFull > RightXFull)
            {
                Prog1 = LeftXFull;
                LeftXFull = RightXFull;
                RightXFull = Prog1;
            }

            // Get the top and bottom extents
            Prog1 = (B * B * CosX * CosX) / ((A * A * SinX * SinX) + (B * B * CosX * CosX));
            Prog2 = Math.Sqrt(Prog1); // square root
            T1 = Math.Acos(Prog2); // cos inverse
            T2 = Math.PI + T1;
            //T2 = Math.Acos(-Prog2);

            TopYFull = Centre.y + (B * CosX * Math.Cos(T1)) + (A * SinX * Math.Sin(T1));
            BottomYFull = Centre.y + (B * CosX * Math.Cos(T2)) + (A * SinX * Math.Sin(T2));

            if (TopYFull > BottomYFull)
            {
                Prog1 = TopYFull;
                TopYFull = BottomYFull;
                BottomYFull = Prog1;
            }

            LeftX = LeftXFull;
            RightX = RightXFull;
            TopY = TopYFull;
            BottomY = BottomYFull;

        }

        public void Draw(Graphics G, Viewing V, Pen P)
        {
            int XA, XB, YA, YB;
            double PX, PY, Pi;
            double SinX, CosX; // sin and cosine of the angle - to reduce the amount of times we have to invoke the relevant
            // functions in the Math class
    
            SinX = Math.Sin(Angle);
            CosX = Math.Cos(Angle);
            Pi = Math.PI;

            for (int i = 0; i < 180; i++)
            {
                PX = Centre.x + (MajorRadius * CosX * Math.Cos(i * Pi / 90)) - (MinorRadius * SinX * Math.Sin(i * Pi / 90));
                PY = Centre.y + (MajorRadius * SinX * Math.Cos(i * Pi / 90)) + (MinorRadius * CosX * Math.Sin(i * Pi / 90));
                XA = (int)(((PX - V.WorldCenX) * V.WorldtoScreen) + (V.ScreenWidth / 2));
                YA = (int)(((V.WorldCenY - PY) * V.WorldtoScreen) + (V.ScreenHeight / 2));

                PX = Centre.x + (MajorRadius * CosX * Math.Cos((i + 1) * Pi / 90)) - (MinorRadius * SinX * Math.Sin((i + 1) * Pi / 90));
                PY = Centre.y + (MajorRadius * SinX * Math.Cos((i + 1) * Pi / 90)) + (MinorRadius * CosX * Math.Sin((i + 1) * Pi / 90));
                XB = (int)(((PX - V.WorldCenX) * V.WorldtoScreen) + (V.ScreenWidth / 2));
                YB = (int)(((V.WorldCenY - PY) * V.WorldtoScreen) + (V.ScreenHeight / 2));

                G.DrawLine(P, XA, YA, XB, YB);
            }

              /* I wrote the rest of the lines in this method to test that arc object was working the way I want it
             * Once I was satisfied that it was, I commented them out */
            /* // Draw extents box

            XA = (int)(((LeftX - V.WorldCenX) * V.WorldtoScreen) + (V.ScreenWidth / 2));
            YA = (int)(((V.WorldCenY - TopY) * V.WorldtoScreen) + (V.ScreenHeight / 2));
            XB = (int)(((RightX - V.WorldCenX) * V.WorldtoScreen) + (V.ScreenWidth / 2));
            YB = (int)(((V.WorldCenY - BottomY) * V.WorldtoScreen) + (V.ScreenHeight / 2));

            G.DrawLine(P, XA, YA, XB, YA);
            G.DrawLine(P, XA, YB, XB, YB);
            G.DrawLine(P, XA, YA, XA, YB);
            G.DrawLine(P, XB, YA, XB, YB);
            */
        }

    }
}
