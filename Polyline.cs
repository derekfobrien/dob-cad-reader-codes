using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CADReader
{
    /*struct _3D
    {
        public double x;
        public double y;
        public double z;
    };*/



    public class Polyline
    {
        public ArrayList Pts = new ArrayList();
        public Boolean IsClosed;
        public double LeftX;
        public double RightX;
        public double TopY;
        public double BottomY;

        public Polyline()
        {
            IsClosed = false; // default: open
        }

        public string ReadFromDXF(StreamReader str, ListBox lb)
        {
            string GroupCode, Value;

            GroupCode = "Unused";
            Value = "Unassigned";
 
            lb.Items.Add("POLYLINE");

            while(GroupCode != "0")
            {
                // get the next code/value pair
                GroupCode = str.ReadLine().Trim();
                Value = str.ReadLine().Trim();
                if (GroupCode == "70") // Closed or not
                {
                    if (Value == "0")
                        IsClosed = false;
                    else if (Value == "1")
                        IsClosed = true;
                }
                if (GroupCode == "10")
                {
                    // create a new 3D point, and then the x-ordinate of it
                    _3D Pt = new _3D();
                    Pt.x = Convert.ToDouble(Value);

                    // the next group code is 20, which is followed by the y-ordinate of the point
                    GroupCode = str.ReadLine().Trim();
                    Value = str.ReadLine().Trim();

                    Pt.y = Convert.ToDouble(Value);

                    lb.Items.Add(Pt.Publish());

                    Pts.Add(Pt);
                }
            }

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

            for (int i = 0; i < Pts.Count; i++)
            {
                _3D Pt = (_3D)Pts[i];
                if (i == 0)
                {
                    LeftX = Pt.x;
                    RightX = Pt.x;
                    TopY = Pt.y;
                    BottomY = Pt.y;
                }
                else
                {
                    if (Pt.x < LeftX)
                        LeftX = Pt.x;
                    if (Pt.x > RightX)
                        RightX = Pt.x;
                    if (Pt.y > TopY)
                        TopY = Pt.y;
                    if (Pt.y < BottomY)
                        BottomY = Pt.y;
                }
            }
        }

        public void Draw(Graphics G, Viewing V, Pen P)
        {
            int XA, XB, YA, YB;

            for (int i = 0; i < Pts.Count - 1; i++)
            {
                _3D Pt1 = (_3D)Pts[i];
                _3D Pt2 = (_3D)Pts[i + 1];

                XA = (int)(((Pt1.x - V.WorldCenX) * V.WorldtoScreen) + (V.ScreenWidth / 2));
                YA = (int)(((V.WorldCenY - Pt1.y) * V.WorldtoScreen) + (V.ScreenHeight / 2));

                XB = (int)(((Pt2.x - V.WorldCenX) * V.WorldtoScreen) + (V.ScreenWidth / 2));
                YB = (int)(((V.WorldCenY - Pt2.y) * V.WorldtoScreen) + (V.ScreenHeight / 2));

                G.DrawLine(P, XA, YA, XB, YB);
            }

            // draw this segment if the polyline is closed
            if (IsClosed == true)
            {
                _3D Pt1 = (_3D)Pts[0];
                _3D Pt2 = (_3D)Pts[Pts.Count - 1];

                XA = (int)(((Pt1.x - V.WorldCenX) * V.WorldtoScreen) + (V.ScreenWidth / 2));
                YA = (int)(((V.WorldCenY - Pt1.y) * V.WorldtoScreen) + (V.ScreenHeight / 2));

                XB = (int)(((Pt2.x - V.WorldCenX) * V.WorldtoScreen) + (V.ScreenWidth / 2));
                YB = (int)(((V.WorldCenY - Pt2.y) * V.WorldtoScreen) + (V.ScreenHeight / 2));

                G.DrawLine(P, XA, YA, XB, YB);
            }
        }
    }
}
