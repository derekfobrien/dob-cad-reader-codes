# dobs-cad-reader-codes
Object-oriented approach to reading an AutoCAD DXF File in C#
This program, which was written in C#, reads a DXF file, which comprises tagged data representing all the information contained in an AutoCAD drawing file. In this format, each item of data is preceded by an integer known as a "group code", which indicates the type of data element that follows.

A DXF file containing even a small number of lines, circles, polylines etc., when opened in Notepad, Notepad++ etc., is upwards of 17,000 lines in length. This is because it contains not only information about the drawing elements, but also of the settings (of which there are a few hundred), information on layers, colours, linetypes and lineweights. Any blocks in the drawing file also have to be described.

Back to the program, I have set up several classes, each with their own file.

Form1.cs - this file contains any variables that will be used in all the classes, methods etc. It also contains the functions relating to the controls' events.

Form1.Designer.cs - this file contains information relating to the sizes, positions etc. of all the controls - picture box, buttons, menus etc.

_3D.ds - this class represents a set of coordinates in 3D, three double-precision numbers.

DrawingSet.cs - this class controls all the procedures of opening and reading the DXF file, and delegates the reading to each type of drawing object.

Line.cs - this class represents a line object - two 3D coordinates

Polyline.cs - this class represents a polyline, containing coordinates for each point, and whether or not the polyline is closed.

Circle.cs - this class represents a circle object - a 3D coordinate representing the centrepoint, and a double-precision number representing the radius.

Arc.cs - this class represents an arc object - a 3D coordinate representing the centrepoint, and double precision numbers representing the radius, start angle and end angle.

Ellipse.cs - this class represents an ellipse object - a 3D coordinate representing the centrepoint, and double precision numbers representing the direction vector of the major axis, the ratio of minor radius to major radius, and start and end angles.
