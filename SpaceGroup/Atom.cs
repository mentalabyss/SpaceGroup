using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace SpaceGroup
{
    public class Atom
    {
        public Atom() { }
        string element;
        double x, y, z;
        //SolidColorBrush 

        public string Element
        {
            get { return element; }
            set { element = value; }
        }

        public double X { get { return x; } set { x = value; } }
        public double Y { get { return y; } set { y = value; } }
        public double Z { get { return z; } set { z = value; } }


        public Atom(string element, string x, string y, string z)
        {
            try
            {
                this.element = element;
                this.x = Convert.ToDouble(x);
                this.y = Convert.ToDouble(y);
                this.z = Convert.ToDouble(z);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
