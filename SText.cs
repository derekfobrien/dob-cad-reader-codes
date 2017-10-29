using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CADReader
{
    public class SText
    {
        public _3D Pos = new _3D();
        public double Height;
        public string Content;

        public string ReadFromDXF(StreamReader str, ListBox lb)
        {

            string GroupCode, Value, Coords;

            GroupCode = "Unused";
            Value = "Unassigned";

            lb.Items.Add("TEXT");

            while (GroupCode != "0")
            {
                // get the next code/value pair
                GroupCode = str.ReadLine().Trim();
                Value = str.ReadLine().Trim();
                switch (GroupCode)
                {
                    case "10": // x-ordinate of the insertion point
                        Pos.x = Convert.ToDouble(Value);
                        break;
                    case "20": // y-ordinate of the insertion point
                        Pos.y = Convert.ToDouble(Value);
                        break;
                    case "30": // z-ordinate of the insertion point
                        Pos.z = Convert.ToDouble(Value);
                        break;
                    case "40": // text height
                        Height = Convert.ToDouble(Value);
                        break;
                    case "1": // content
                        Content = Value;
                        break;
                }

            }

            Coords = string.Concat("Location: ", Pos.Publish());
            lb.Items.Add(Coords);
            Coords = string.Concat("Height: ", Height.ToString());
            lb.Items.Add(Coords);
            lb.Items.Add(Content);

            // this is the name of the next object to follow
            // it is the value when we get a group code of 0, thus exiting the while loop
            return Value;
        }
    }
}
