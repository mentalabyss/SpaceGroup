using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGroup
{
    public class Atom
    {
        string element;
        double x, y, z;

        public string Element
        {
            get { return element; }
        }

        public double X { get { return x; } }
        public double Y { get { return y; } }
        public double Z { get { return z; } }


        public Atom(string element, string x, string y, string z)
        {
            this.element = element;
            this.x = Convert.ToDouble(x);
            this.y = Convert.ToDouble(y);
            this.z = Convert.ToDouble(z);
        }
    }
}
