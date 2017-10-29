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
    
    public class Line
    {
        public _3D P1 = new _3D();
        public _3D P2 = new _3D();
        public double LeftX;
        public double RightX;
        public double TopY;
        public double BottomY;

        public string ReadFromDXF(StreamReader str, ListBox lb)
        {
            string GroupCode, Value, Coords;

            GroupCode = "Unused";
            Value = "Unassigned";

            lb.Items.Add("LINE");

            while (GroupCode != "0")
            {
                // get the next code/value pair
                GroupCode = str.ReadLine().Trim();
                Value = str.ReadLine().Trim();
                switch (GroupCode)
                {
                    case "10": // x-ordinate of start point
                        P1.x = Convert.ToDouble(Value);
                        break;
                    case "20": // y-ordinate of start point
                        P1.y = Convert.ToDouble(Value);
                        break;
                    case "30": // z-ordinate of start point
                        P1.z = Convert.ToDouble(Value);
                        break;
                    case "11": // x-ordinate of end point
                        P2.x = Convert.ToDouble(Value);
                        break;
                    case "21": // y-ordinate of end point
                        P2.y = Convert.ToDouble(Value);
                        break;
                    case "31": // z-ordinate of end point
                        P2.z = Convert.ToDouble(Value);
                        break;
                }
            }

            // write the co-ordinates onto the listbox
            Coords = string.Concat(P1.Publish(), " - ", P2.Publish());
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

            if (P1.x < P2.x)
            {
                LeftX = P1.x;
                RightX = P2.x;
            }
            else
            {
                LeftX = P2.x;
                RightX = P1.x;
            }

            if (P1.y < P2.y)
            {
                BottomY = P1.y;
                TopY = P2.y;
            }
            else
            {
                BottomY = P2.y;
                TopY = P1.y;
            }
        }
        
        public void Draw(Graphics G, Viewing V, Pen P)
        {
            int XA, XB, YA, YB;

            XA = (int)(((P1.x - V.WorldCenX) * V.WorldtoScreen) + (V.ScreenWidth / 2));
            YA = (int)(((V.WorldCenY - P1.y) * V.WorldtoScreen) + (V.ScreenHeight / 2));

            XB = (int)(((P2.x - V.WorldCenX) * V.WorldtoScreen) + (V.ScreenWidth / 2));
            YB = (int)(((V.WorldCenY - P2.y) * V.WorldtoScreen) + (V.ScreenHeight / 2));

            G.DrawLine(P, XA, YA, XB, YB);
        }

    }




}
