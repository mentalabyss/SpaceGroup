using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace SpaceGroup
{
    public class AtomVisual : ModelVisual3D
    {
        bool isHidden = false;
        bool hasPolyhedra = false;
        double x;
        double y;
        double z;
        double atomVisualSize;
        string atomColor;
        
        private Atom atom;
        GeometryModel3D polyhedraModel;
        Model3DGroup MiscModel3DGroup = new Model3DGroup();

        public AtomVisual() { }

        public AtomVisual(Point3D atomPoint, double atomVisualSize, string atomColor, Atom atom) //для создания атомов размножений
        {
            this.atomVisualSize = atomVisualSize;
            this.atomColor = atomColor;
            this.atom = atom;

            MeshGeometry3D atomMesh = new MeshGeometry3D();

            atomMesh.AddSphere(atomPoint, atomVisualSize, 20, 30);

            SolidColorBrush brush1 = (SolidColorBrush)(new BrushConverter().ConvertFrom(this.atomColor));

            DiffuseMaterial material1 = new DiffuseMaterial(brush1);

            GeometryModel3D atomModel = new GeometryModel3D(atomMesh, material1);

            //Content = atomModel;
            MiscModel3DGroup.Children.Add(atomModel);
            Content = MiscModel3DGroup;
        }

        public AtomVisual(Atom atom, string atomColor, SpaceGroupCl selectedSpaceGroup, CrystalCell atomCell,
            List<Atom> multipliedAtoms, double xAxisL = 0, double yAxisL = 0, double zAxisL = 0)
        {
            if (selectedSpaceGroup == null)
                throw new NotImplementedException();

            this.atom = atom;

            Content = MiscModel3DGroup;

            double atomVisualSize = 0.4;
            if (atom.Element[0] == 'O' && (atom.Element[1] != 's'))
            {
                atomVisualSize = 0.2;
            }

            //Model3DGroup atomRepro = new Model3DGroup();
            List<Atom> atomReproList = new List<Atom>();

            for (int i = 0; i < selectedSpaceGroup.Expressions.Length; i += 3)
            {
                double X = SpaceGroupCl.Evaluate(selectedSpaceGroup.Expressions[i + 1], 0, atom.Y, 0); //сухие координаты
                double Y = SpaceGroupCl.Evaluate(selectedSpaceGroup.Expressions[i + 2], 0, 0, atom.Z);
                double Z = SpaceGroupCl.Evaluate(selectedSpaceGroup.Expressions[i], atom.X, 0, 0);


                if (X < 0)
                    X += 1;
                if (Y < 0)
                    Y += 1;
                if (Z < 0)
                    Z += 1;

                if (X > 1)
                    X -= 1;
                if (Y > 1)
                    Y -= 1;
                if (Z > 1)
                    Z -= 1;

                //
                double a = atomCell.YAxisL;
                double b = atomCell.ZAxisL;
                double c = atomCell.XAxisL;

                double xC = c * Math.Cos(ToRadians(atomCell.Gamma));
                double yC = (b * c * Math.Cos(ToRadians(atomCell.Beta)) - xC * b * Math.Cos(ToRadians(atomCell.Alpha))) /
                    b * Math.Sin(ToRadians(atomCell.Alpha));
                double zC = Math.Sqrt(c * c - xC * xC - yC * yC);

                Vector3D xVector = new Vector3D(a * X, 0, 0);
                Vector3D oyVector = new Vector3D(b * Math.Cos(ToRadians(atomCell.Alpha)) * Y, b * Math.Sin(ToRadians(atomCell.Alpha)) * Y, 0);
                Vector3D zVector = new Vector3D(xC * Z, yC * Z, zC * Z);

                Vector3D xyVector = Vector3D.Add(xVector, oyVector);
                Vector3D xyzVector = Vector3D.Add(xyVector, zVector);
                //

                var addedAtom = new Atom(atom.Element, Z.ToString(), X.ToString(), Y.ToString(), null);
                if (multipliedAtoms.Contains(addedAtom)) continue;
                multipliedAtoms.Add(addedAtom);
                AtomVisual multipliedAtomVisual = new AtomVisual((Point3D)xyzVector, atomVisualSize, atomColor, addedAtom);
                Children.Add(multipliedAtomVisual);
                atomReproList.Add(new Atom(atom.Element, Z.ToString(), X.ToString(), Y.ToString(), atom.Brush));
            }
        }

        public Atom Atom { get => atom; set => atom = value; }
        public string AtomColor { get => atomColor; set => atomColor = value; }

        public void AddPolyhedra(CrystalCell atomCell)
        {
            if (atom.hasPolyhedra)
            {
                MeshGeometry3D polyhedraMesh = new MeshGeometry3D();
                polyhedraModel = polyhedraMesh.DrawSinglePolyhedra(atom, atomCell, 4);
            }
        }

        public void showPolyhedra()
        {
            if (!MiscModel3DGroup.Children.Contains(polyhedraModel) && atom.hasPolyhedra)
            MiscModel3DGroup.Children.Add(polyhedraModel);
        }
        public void hidePolyhedra()
        {
            MiscModel3DGroup.Children.Remove(polyhedraModel);
        }

        public static double ToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }
    }
}
