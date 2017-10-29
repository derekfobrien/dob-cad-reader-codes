using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CADReader
{
    public partial class Form1 : Form
    {
        public DrawingSet theDrawingSet = new DrawingSet();
        public Viewing theViewing = new Viewing();

        string theFileName = "Untitled.dxf";
        int MouseMode = 0; // 0 = nothing, 1 = pan, 2 = zoom
        bool LMBPressed; // whether the left mouse button is pressed or not
        int InitEX, InitEY; // variables to hold values of the mouse coordinates at the time the mouse button was pressed
        double InitV1, InitV2; // variables to hold values at the time the mouse button was pressed, for use by the Viewing class


        public Form1()
        {
            InitializeComponent();
        }
    
        private void Form1_Load(object sender, EventArgs e)
        {
            ResizeControls();

            openFileDialog1.Filter = "DXF files|*.dxf";
            
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            ResizeControls();
        }



       

        private void MnuFileOpen_Click(object sender, EventArgs e)
        {
            //display the OpenFileDialog
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                theFileName = openFileDialog1.FileName;
                theDrawingSet.ReadFromDXF(theFileName, listBox1);
                theDrawingSet.SetExtents(theViewing, pictureBox1, slCentre, slCorners, slBoxSize);
                theDrawingSet.DrawSet(pictureBox1, theViewing);
            }
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            //Click
            LMBPressed = true;
            switch (MouseMode)
            {
                case 1:
                    //pan
                    InitV1 = theViewing.WorldCenX;
                    InitV2 = theViewing.WorldCenY;
                    break;
                case 2:
                    //zoom
                    InitV2 = theViewing.WorldHeight;
                    break;
            }

            InitEX = e.X;
            InitEY = e.Y;

        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            //Unclick
            LMBPressed = false;
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //Move
            if (LMBPressed)
            {
                switch (MouseMode)
                {
                    case 1://pan
                        theViewing.Pan(pictureBox1, slCentre, slCorners, slBoxSize, InitV1, InitV2, InitEX, InitEY, e.X, e.Y);
                        theDrawingSet.DrawSet(pictureBox1, theViewing);
                        break;
                    case 2://zoom
                        theViewing.Zoom(pictureBox1, slCentre, slCorners, slBoxSize, InitV2, InitEY, e.Y);
                        theDrawingSet.DrawSet(pictureBox1, theViewing);
                        break;
                }//end switch
            }//endif
        }

        private void PictureBox1_Resize(object sender, EventArgs e)
        {
            theViewing.AdjustPictureBoxResize(pictureBox1, slCentre, slCorners, slBoxSize);
            theDrawingSet.DrawSet(pictureBox1, theViewing);
        }

        private void CmdPan_Click(object sender, EventArgs e)
        {
            MouseMode = 1;
            cmdPan.BackColor = Color.FromArgb(127, 255, 255);
            cmdZoom.BackColor = BackColor;
        }

        private void CmdZoom_Click(object sender, EventArgs e)
        {
            MouseMode = 2;
            cmdZoom.BackColor = Color.FromArgb(127, 255, 255);
            cmdPan.BackColor = BackColor;
        }

        void ResizeControls()
        {
            int Buffer = 15;

            listBox1.Left = Buffer;
            listBox1.Top = Buffer + menuStrip1.Height;
            listBox1.Height = ClientSize.Height - (listBox1.Top + Buffer);

            pictureBox1.Left = (2 * Buffer) + listBox1.Width;
            pictureBox1.Top = listBox1.Top;
            pictureBox1.Height = listBox1.Height;
            pictureBox1.Width = ClientSize.Width - (pictureBox1.Left + cmdPan.Width + (2 * Buffer));

            cmdPan.Top = pictureBox1.Top;
            cmdPan.Left = pictureBox1.Left + pictureBox1.Width + Buffer;

            cmdZoom.Top = cmdPan.Top + cmdPan.Height + Buffer;
            cmdZoom.Left = cmdPan.Left;


        }

       
    }
 
}
