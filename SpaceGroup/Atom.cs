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
    }
}
