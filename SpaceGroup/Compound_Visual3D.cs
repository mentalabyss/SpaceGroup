using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace SpaceGroup
{
    public class Compound_Visual3D : ModelVisual3D
    {
        private Model3DGroup AxesAndCellModels;

        public Compound_Visual3D()
        {
            AxesAndCellModels = new Model3DGroup();
            this.Content = AxesAndCellModels;
        }

        public void addAtomVisual(Atom atom)
        {
        }


    }
}
