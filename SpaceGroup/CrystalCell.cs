using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGroupProject
{
    public class CrystalCell
    {
        public System.Collections.ObjectModel.ObservableCollection<Atom> atomCollection { get; set; }

        public double xAxisL, yAxisL, zAxisL, alpha, beta, gamma;

        public CrystalCell()
        {
            atomCollection = new System.Collections.ObjectModel.ObservableCollection<Atom>();
        }

        public void setCellParams(double xL, double yL, double zL, double a, double b, double g)
        {
            xAxisL = xL;
            yAxisL = yL;
            zAxisL = zL;
            alpha = a;
            beta = b;
            gamma = g;
        }


    }
}
