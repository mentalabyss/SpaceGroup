using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;

namespace SpaceGroup
{
    public class Atom
    {
        public Atom() { }
        string element;
        double x, y, z;
        System.Windows.Media.SolidColorBrush brush;
        string color;

        public string Element
        {
            get { return element; }
            set { element = value; }
        }

        public double X { get { return x; } set { x = value; } }
        public double Y { get { return y; } set { y = value; } }
        public double Z { get { return z; } set { z = value; } }

        [XmlIgnoreAttribute]
        public SolidColorBrush Brush { get => brush; set => brush = value; }

        public Atom(string element, string x, string y, string z, System.Windows.Media.SolidColorBrush brush)
        {
            try
            {
                this.element = element;
                this.x = Convert.ToDouble(x);
                this.y = Convert.ToDouble(y);
                this.z = Convert.ToDouble(z);
                this.brush = brush;
                color = brush.ToString();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public string Color
        {
            get { return color; }
            set { color = value; }
        }

        public void StringToColor()
        {
            brush = new SolidColorBrush();
            brush.Color = (Color)ColorConverter.ConvertFromString(color);
        }

        public static double ThreeAtomsAngle(Atom atomA, Atom atomB, Atom atomC)
        {
            double ABx = atomB.X - atomA.X;
            double ABy = atomB.Y - atomA.Y;
            double ABz = atomB.Z - atomA.Z;

            double BCx = atomC.X - atomB.X;
            double BCy = atomC.Y - atomB.Y;
            double BCz = atomC.Z - atomB.Z;

            double ABBC = ABx * BCx + ABy * BCy + ABz * BCz;

            double AB_length = Math.Sqrt(ABx * ABx + ABy * ABy + ABz * ABz);
            double BC_length = Math.Sqrt(BCx * BCx + BCy * BCy + BCz * BCz);

            return 180 - Math.Acos(ABBC / (AB_length * BC_length)) * (180 / Math.PI);
        }

        public static double DistanceTwoAtoms(Atom atom1, Atom atom2, CrystalCell atomCell)
        {
            return Math.Sqrt((atom1.X - atom2.X) * (atom1.X - atom2.X) * atomCell.XAxisL * atomCell.XAxisL
                + (atom1.Y - atom2.Y) * (atom1.Y - atom2.Y) * atomCell.YAxisL * atomCell.YAxisL
                + (atom1.Z - atom2.Z) * (atom1.Z - atom2.Z) * atomCell.ZAxisL * atomCell.ZAxisL);
        }
    }
}
