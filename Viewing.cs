using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CADReader
{
    public class Viewing
    {
        public double WorldCenX;
        public double WorldCenY;
        public double WorldLeftX;
        public double WorldRightX;
        public double WorldTopY;
        public double WorldBottomY;
        public double WorldWidth;
        public double WorldHeight;
        public double WorldDiagonal;
        public double WorldtoScreen;
        public double ScreenWidth;
        public double ScreenHeight;
        public double ScreenDiagonal;

        public void AdjustPictureBoxResize(PictureBox pb, ToolStripStatusLabel slc, ToolStripStatusLabel slbox, ToolStripStatusLabel slsize)
        {
            /* this method adjust the properties of the Viewing class following
             * re-sizing of the picture box.
             * The centre coordinates remain the same, as does the diagonal dimension
             * The width and height will be adjusted
            */

            ScreenWidth = pb.Width;
            ScreenHeight = pb.Height;

            ScreenDiagonal = Math.Sqrt((ScreenWidth * ScreenWidth) + (ScreenHeight * ScreenHeight));

            WorldtoScreen = ScreenDiagonal / WorldDiagonal;

            WorldLeftX = WorldCenX - (WorldWidth / 2);
            WorldRightX = WorldCenX + (WorldWidth / 2);

            WorldTopY = WorldCenY - (WorldHeight / 2);
            WorldBottomY = WorldCenY + (WorldHeight / 2);

            SetStatusLabels(slc, slbox, slsize);

            SetDiagonal();

        }

        public void Pan(PictureBox pb, ToolStripStatusLabel slc, ToolStripStatusLabel slbox, ToolStripStatusLabel slsize, double VXinit, double VYinit, int XInit, int YInit, int Xe, int Ye)
        {
            /* this method adjust the centre coordinate properties
             * the width, height and diagonal properties remain unchanged
             * */
             
            WorldCenX = VXinit + ((XInit - Xe) * WorldWidth / ScreenWidth);
            WorldCenY = VYinit - ((YInit - Ye) * WorldHeight / ScreenHeight);

            WorldLeftX = WorldCenX - (WorldWidth / 2);
            WorldRightX = WorldCenX + (WorldWidth / 2);

            WorldTopY = WorldCenY - (WorldHeight / 2);
            WorldBottomY = WorldCenY + (WorldHeight / 2);

            SetStatusLabels(slc, slbox, slsize);
        }

        public void Zoom(PictureBox pb, ToolStripStatusLabel slc, ToolStripStatusLabel slbox, ToolStripStatusLabel slsize, double VYInit, int YInit, int Ye)
        {
            /* this method adjusts the zoom 
             * 
             */

            WorldHeight = VYInit * Math.Pow(0.9, ((double)(YInit - Ye)) / 10);
            WorldWidth = WorldHeight * ScreenWidth / ScreenHeight;
            WorldtoScreen = WorldtoScreen = 0.95 * ScreenWidth / WorldWidth;

            WorldLeftX = WorldCenX - (WorldWidth / 2);
            WorldRightX = WorldCenX + (WorldWidth / 2);

            WorldTopY = WorldCenY - (WorldHeight / 2);
            WorldBottomY = WorldCenY + (WorldHeight / 2);

            SetStatusLabels(slc, slbox, slsize);

        }

        public void SetExtents(double XL, double XR, double YT, double YB, PictureBox pb, ToolStripStatusLabel slc, ToolStripStatusLabel slbox, ToolStripStatusLabel slsize)
         {
             /* XL, XR, YT and YB are the values of the left, right, top and bottom extents respectively
              * as calculated in the DrawingSet class
              */

            ScreenWidth = pb.Width;
            ScreenHeight = pb.Height;
            ScreenDiagonal = Math.Sqrt((ScreenWidth * ScreenWidth) + (ScreenHeight * ScreenHeight));

            WorldCenX = (XL + XR) / 2;
            WorldCenY = (YT + YB) / 2;

            WorldWidth = XR - XL;
            WorldHeight = YT - YB;

            WorldLeftX = WorldCenX - (WorldWidth / 2);
            WorldRightX = WorldCenX + (WorldWidth / 2);

            WorldTopY = WorldCenY - (WorldHeight / 2);
            WorldBottomY = WorldCenY + (WorldHeight / 2);

            if (WorldHeight / WorldWidth > ScreenHeight / ScreenWidth)
            {
                /* portrait leaning, where the width-to-height ratio of the extents box
                 * is greater than that of the picture box
                 */
                WorldtoScreen = 0.95 * ScreenHeight / WorldHeight;
                WorldWidth = ScreenWidth / WorldtoScreen;

            }
            else
            {
                /* portrait leaning, where the width-to-height ratio of the extents box
                 * is greater than that of the picture box
                 */
                WorldtoScreen = 0.95 * ScreenWidth / WorldWidth;
                WorldHeight = ScreenHeight / WorldtoScreen;
            }

            SetStatusLabels(slc, slbox, slsize);

            SetDiagonal();
        }

        void SetDiagonal()
        {
            WorldDiagonal = Math.Sqrt((WorldWidth * WorldWidth) + (WorldHeight * WorldHeight));
        }

        void SetStatusLabels(ToolStripStatusLabel slCen, ToolStripStatusLabel slCor, ToolStripStatusLabel slBox)
        {
            slCen.Text = string.Concat("Centre: ", WorldCenX.ToString("F3"), ", ", WorldCenY.ToString("F3"));
            slCor.Text = string.Concat("Corners: (", WorldLeftX.ToString("F3"), ", ", WorldTopY.ToString("F3"), ") - (", WorldRightX.ToString("F3"), ", ", WorldBottomY.ToString("F3"), ")");
            slBox.Text = string.Concat("Size: ", WorldWidth.ToString("F3"), " x ", WorldHeight.ToString("F3"));

        }
    }
}
