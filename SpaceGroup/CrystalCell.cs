using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGroup
{
    public class CrystalCell
    {
        public System.Collections.ObjectModel.ObservableCollection<Atom> atomCollection { get; set; }


        private double xAxisL, yAxisL, zAxisL, alpha, beta, gamma, volume;

        public double XAxisL { get => xAxisL; set => xAxisL = value; }
        public double YAxisL { get => yAxisL; set => yAxisL = value; }
        public double ZAxisL { get => zAxisL; set => zAxisL = value; }
        public double Alpha { get => xAxisL; set => xAxisL = value; }
        public double Beta { get => xAxisL; set => xAxisL = value; }
        public double Gamma { get => xAxisL; set => xAxisL = value; }
        public double Volume { get => volume; set => volume = value; }

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
