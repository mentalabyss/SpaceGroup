using System;
using System.Collections.Generic;
using System.Globalization;
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
        bool hasPolyhedra;

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

                CultureInfo culture = new CultureInfo("en-US");
                if (x.Contains('.'))
                    this.x = Double.Parse(x, culture);
                else if (x.Contains(','))
                    this.x = Double.Parse(x, new CultureInfo("ru-RU"));
                else
                    this.x = Double.Parse(x);

                if (y.Contains('.'))
                    this.y = Double.Parse(y, culture);
                else if (y.Contains(','))
                    this.y = Double.Parse(y, new CultureInfo("ru-RU"));
                else
                    this.y = Double.Parse(y);
                if (z.Contains('.'))
                    this.z = Double.Parse(z, culture);
                else if (z.Contains(','))
                    this.z = Double.Parse(z, new CultureInfo("ru-RU"));
                else
                    this.z = Double.Parse(z);
                this.brush = brush;
                color = brush.ToString();
            }
            catch(NullReferenceException e)
            {
                Console.WriteLine(e.Message);
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

        private static double GetDouble(string value, double defaultValue)
        {
            double result;

            // Try parsing in the current culture
            if (
                // Then try in US english
                !double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result) &&
                !double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("ru-RU"), out result) &&
                // Then in neutral language
                !double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                result = defaultValue;
            }
            return result;
        }

        public bool PositionEquals(object obj)
        {
            if ((obj == null) || ! GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Atom atom = (Atom)obj;
                return (X == atom.X && Y == atom.Y && Z == atom.Z);
            }
        }


        public override string ToString()
        {
            return X + " " + Y + " " + Z;
        }

        public override bool Equals(object obj)
        {
            var item = obj as Atom;

            if(item == null)
            {
                return false;
            }

            return this.X.Equals(item.X) && this.Y.Equals(item.Y) && this.Z.Equals(item.Z);
        }
    }
}
