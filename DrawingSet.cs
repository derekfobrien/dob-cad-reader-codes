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
    public class DrawingSet
    {
        public ArrayList Lines = new ArrayList();
        public ArrayList Polylines = new ArrayList();
        public ArrayList STexts = new ArrayList();
        public ArrayList Circles = new ArrayList();
        public ArrayList Arcs = new ArrayList();
        public ArrayList Ellipses = new ArrayList();

        public double LeftX;
        public double RightX;
        public double TopY;
        public double BottomY;

        public void ReadFromDXF(string TheFileName, ListBox theListBox)
        {
            FileStream fs = new FileStream(TheFileName, FileMode.Open);
            StreamReader str = new StreamReader(fs);
            int RecordNo = 0;

            string GroupCode, Value, LastObject;

            LastObject = "Nothing Yet"; // an initial value

            // get the first code/value pair
            GroupCode = str.ReadLine().Trim();
            RecordNo++;
            Value = str.ReadLine().Trim();
            RecordNo++;

            // loop through the whole file until the "EOF" line
            while (Value != "EOF")
            {
                // if the group code is 0 and the value is SECTION
                if (GroupCode == "0" && Value == "SECTION")
                {
                    // this must be a new section, so get the next code/value pair
                    GroupCode = str.ReadLine().Trim();
                    RecordNo++;
                    Value = str.ReadLine().Trim();
                    RecordNo++;

                    if (Value == "ENTITIES")
                    {
                        // get the next code/value pair and ...
                        GroupCode = str.ReadLine().Trim();
                        RecordNo++;
                        Value = str.ReadLine().Trim();
                        RecordNo++;

                        //  loop through this section until the 'ENDSEC' 
                        while (Value != "ENDSEC")
                        {
                            if (GroupCode == "0")
                                LastObject = Value;

                            // While in a section, all '0' codes indicate an object. If you find a '0' store the object name for future use
                            if (GroupCode == "0")
                            {
                                switch (Value)
                                {
                                    // if it is a line
                                    case "LINE":
                                        Line theLine = new Line();
                                        Value = theLine.ReadFromDXF(str, theListBox);
                                        Lines.Add(theLine);
                                        break;
                                    case "LWPOLYLINE":
                                        Polyline thePolyline = new Polyline();
                                        Value = thePolyline.ReadFromDXF(str, theListBox);
                                        Polylines.Add(thePolyline);
                                        break;
                                    case "TEXT":
                                        SText theText = new SText();
                                        Value = theText.ReadFromDXF(str, theListBox);
                                        STexts.Add(theText);
                                        break;
                                    case "CIRCLE":
                                        Circle theCircle = new Circle();
                                        Value = theCircle.ReadFromDXF(str, theListBox);
                                        Circles.Add(theCircle);
                                        break;
                                    case "ARC":
                                        Arc theArc = new Arc();
                                        Value = theArc.ReadFromDXF(str, theListBox);
                                        Arcs.Add(theArc);
                                        break;
                                    case "ELLIPSE":
                                        Ellipse theEllipse = new Ellipse();
                                        Value = theEllipse.ReadFromDXF(str, theListBox);
                                        Ellipses.Add(theEllipse);
                                        break;
                                    default:
                                        // get the next code/value pair
                                        GroupCode = str.ReadLine().Trim();
                                        RecordNo++;
                                        Value = str.ReadLine().Trim();
                                        RecordNo++;
                                        break;
                                }
                            }
                            else
                            {
                                // get the next code/value pair
                                GroupCode = str.ReadLine().Trim();
                                RecordNo++;
                                Value = str.ReadLine().Trim();
                                RecordNo++;
                            }
                        }
                    }
                   
                }
                else
                {
                    // get the next code/value pair
                    GroupCode = str.ReadLine().Trim();
                    RecordNo++;
                    Value = str.ReadLine().Trim();
                    RecordNo++;
                }
            }
        }

        public void SetExtents(Viewing V, PictureBox p, ToolStripStatusLabel slc, ToolStripStatusLabel slbox, ToolStripStatusLabel slsize)
        {
            // get the extents using all the lines
            for (int i = 0; i < Lines.Count; i++)
            {
                Line theLine = (Line)Lines[i];
                if (i == 0)
                {
                    LeftX = theLine.LeftX;
                    RightX = theLine.RightX;
                    TopY = theLine.TopY;
                    BottomY = theLine.BottomY;
                }
                else
                {
                    if (theLine.LeftX < LeftX)
                        LeftX = theLine.LeftX;
                    if (theLine.RightX > RightX)
                        RightX = theLine.RightX;
                    if (theLine.TopY > TopY)
                        TopY = theLine.TopY;
                    if (theLine.BottomY < BottomY)
                        BottomY = theLine.BottomY;
                }
            }

            // get the extents using all the polylines
            for (int i = 0; i < Polylines.Count; i++)
            {
                Polyline thePolyline = (Polyline)Polylines[i];

                if (thePolyline.LeftX < LeftX)
                    LeftX = thePolyline.LeftX;
                if (thePolyline.RightX > RightX)
                    RightX = thePolyline.RightX;
                if (thePolyline.TopY > TopY)
                    TopY = thePolyline.TopY;
                if (thePolyline.BottomY < BottomY)
                    BottomY = thePolyline.BottomY;
            }

            // now the circles
            for (int i = 0; i < Circles.Count; i++)
            {
                Circle theCirle = (Circle)Circles[i];
                if (theCirle.LeftX < LeftX)
                    LeftX = theCirle.LeftX;
                if (theCirle.RightX > RightX)
                    RightX = theCirle.RightX;
                if (theCirle.TopY > TopY)
                    TopY = theCirle.TopY;
                if (theCirle.BottomY < BottomY)
                    BottomY = theCirle.BottomY;
            }

            // the arcs
            for (int i = 0; i < Arcs.Count; i++)
            {
                Arc theArc = (Arc)Arcs[i];
                if (theArc.LeftX < LeftX)
                    LeftX = theArc.LeftX;
                if (theArc.RightX > RightX)
                    RightX = theArc.RightX;
                if (theArc.TopY > TopY)
                    TopY = theArc.TopY;
                if (theArc.BottomY < BottomY)
                    BottomY = theArc.BottomY;
            }

            // the ellipses
            for (int i = 0; i < Ellipses.Count; i++)
            {
                Ellipse theEllipse = (Ellipse)Ellipses[i];
                if (theEllipse.LeftX < LeftX)
                    LeftX = theEllipse.LeftX;
                if (theEllipse.RightX > RightX)
                    RightX = theEllipse.RightX;
                if (theEllipse.TopY > TopY)
                    TopY = theEllipse.TopY;
                if (theEllipse.BottomY < BottomY)
                    BottomY = theEllipse.BottomY;
            }

            /* now that we have LeftX, RightX, TopY, BottomY, we need to calculate
             * the viewing coordinates
            */

            V.SetExtents(LeftX, RightX, TopY, BottomY, p, slc, slbox, slsize);
        }

        public void DrawSet(PictureBox thePictureBox, Viewing V)
        {
            // Draw the objects
            //setup the Graphics object and the pens with different colours
            Graphics Gr = thePictureBox.CreateGraphics();
            Pen penGrey = new Pen(Color.FromArgb(127, 127, 127));
            Pen penRed = new Pen(Color.Red);
            Pen penBlue = new Pen(Color.Blue);
            Pen penGreen = new Pen(Color.Green);
            Pen penTurquoise = new Pen(Color.FromArgb(80, 160, 255));

            Gr.Clear(Color.Black);

            // draw the lines
            for (int i = 0; i < Lines.Count; i++)
            {
                Line theLine = (Line)Lines[i];
                theLine.Draw(Gr, V, penGreen);
            }

            // draw the polylines
            for (int i = 0; i < Polylines.Count; i++)
            {
                Polyline thePolyline = (Polyline)Polylines[i];
                thePolyline.Draw(Gr, V, penBlue);
            }

            //draw the circles
            for (int i = 0; i < Circles.Count; i++)
            {
                Circle theCircle = (Circle)Circles[i];
                theCircle.Draw(Gr, V, penRed);
            }

            //draw the arcs
            for (int i = 0; i < Arcs.Count; i++)
            {
                Arc theArc = (Arc)Arcs[i];
                theArc.Draw(Gr, V, penGrey);
            }

            //draw the ellipses
            for (int i = 0; i < Ellipses.Count; i++)
            {
                Ellipse theEllipse = (Ellipse)Ellipses[i];
                theEllipse.Draw(Gr, V, penTurquoise);
            }

        }
    }
}
