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
            double.TryParse(x, out this.x);
            double.TryParse(y, out this.y);
            double.TryParse(z, out this.z);

            //if (this.x < 0)
            //    this.x += 1;

            //if (this.y < 0)
            //    this.y += 1;

            //if (this.z < 0)
            //    this.z += 1;
        }
    }
}
