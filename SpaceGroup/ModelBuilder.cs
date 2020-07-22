using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace SpaceGroup
{
    public static class ModelBuilder
    {
        public static void buildCellBorders(out GeometryModel3D borders_model, CrystalCell atomCell)
        {
            MeshGeometry3D borders_mesh = new MeshGeometry3D();
            Point3D origin = new Point3D(0, 0, 0);
            Point3D xLine = new Point3D(atomCell.YAxisL, 0, 0);
            Point3D yLine = new Point3D(0, atomCell.ZAxisL, 0);
            Point3D zLine = new Point3D(0, 0, atomCell.XAxisL);

            borders_mesh.AddSegment(origin, xLine, new Vector3D(0, 1, 0), 0.04);
            borders_mesh.AddSegment(origin, yLine, new Vector3D(1, 0, 0), 0.04);
            borders_mesh.AddSegment(origin, zLine, new Vector3D(0, 1, 0), 0.04);

            borders_mesh.AddSegment(new Point3D(atomCell.YAxisL, 0, 0), new Point3D(atomCell.YAxisL, 0, atomCell.XAxisL), new Vector3D(0, 1, 0), 0.04);
            borders_mesh.AddSegment(new Point3D(atomCell.YAxisL, atomCell.ZAxisL, 0), new Point3D(atomCell.YAxisL, atomCell.ZAxisL, atomCell.XAxisL), new Vector3D(0, 1, 0), 0.04);
            borders_mesh.AddSegment(new Point3D(0, atomCell.ZAxisL, 0), new Point3D(0, atomCell.ZAxisL, atomCell.XAxisL), new Vector3D(0, 1, 0), 0.04);


            borders_mesh.AddSegment(new Point3D(0, 0, atomCell.XAxisL), new Point3D(atomCell.YAxisL, 0, atomCell.XAxisL), new Vector3D(0, 1, 0), 0.04);
            borders_mesh.AddSegment(new Point3D(0, atomCell.ZAxisL, atomCell.XAxisL), new Point3D(atomCell.YAxisL, atomCell.ZAxisL, atomCell.XAxisL), new Vector3D(0, 1, 0), 0.04);
            borders_mesh.AddSegment(new Point3D(0, atomCell.ZAxisL, 0), new Point3D(atomCell.YAxisL, atomCell.ZAxisL, 0), new Vector3D(0, 1, 0), 0.04);


            borders_mesh.AddSegment(new Point3D(atomCell.YAxisL, 0, 0), new Point3D(atomCell.YAxisL, atomCell.ZAxisL, 0), new Vector3D(1, 0, 0), 0.04);
            borders_mesh.AddSegment(new Point3D(0, 0, atomCell.XAxisL), new Point3D(0, atomCell.ZAxisL, atomCell.XAxisL), new Vector3D(1, 0, 0), 0.04);
            borders_mesh.AddSegment(new Point3D(atomCell.YAxisL, 0, atomCell.XAxisL), new Point3D(atomCell.YAxisL, atomCell.ZAxisL, atomCell.XAxisL), new Vector3D(1, 0, 0), 0.04);




            SolidColorBrush borders_brush = Brushes.Black;
            DiffuseMaterial borders_material = new DiffuseMaterial(borders_brush);
            borders_model = new GeometryModel3D(borders_mesh, borders_material);
        }

        public static void buildStaticAxes(out GeometryModel3D static_Y_axis_model, out GeometryModel3D static_X_axis_model, out GeometryModel3D static_Z_axis_model)
        {
            MeshGeometry3D x_mesh = new MeshGeometry3D();
            MeshGeometry3D y_mesh = new MeshGeometry3D();
            MeshGeometry3D z_mesh = new MeshGeometry3D();
            Point3D origin = new Point3D(0, 0, 0);


            //Y (normal: X):
            Point3D xmax = new Point3D(25, 0, 0);
            y_mesh.AddSegment(origin, xmax, new Vector3D(0, 1, 0), 0.1);

            Vector3D vX = xmax - origin;
            vX.Normalize();
            Vector3D perpX = Vector3D.CrossProduct(vX, new Vector3D(0, 1, 0));
            perpX.Normalize();
            Vector3D v1X = ScaleVector(-vX + perpX, 1);
            Vector3D v2X = ScaleVector(-vX - perpX, 1);
            y_mesh.AddSegment(xmax, xmax + v1X, new Vector3D(0, 1, 0), 0.1);
            y_mesh.AddSegment(xmax, xmax + v2X, new Vector3D(0, 1, 0), 0.1);

            Vector3D perpX1 = Vector3D.CrossProduct(vX, new Vector3D(0, 0, 1));
            perpX1.Normalize();
            Vector3D v1X1 = ScaleVector(-vX + perpX1, 1);
            Vector3D v2X1 = ScaleVector(-vX - perpX1, 1);
            y_mesh.AddSegment(xmax, xmax + v1X1, new Vector3D(0, 0, 1), 0.1);
            y_mesh.AddSegment(xmax, xmax + v2X1, new Vector3D(0, 0, 1), 0.1);

            SolidColorBrush axes_brush = Brushes.Green;
            DiffuseMaterial y_axis_material = new DiffuseMaterial(axes_brush);

            static_Y_axis_model = new GeometryModel3D(y_mesh, y_axis_material);

            //Z (normal: Y):
            Point3D ymax = new Point3D(0, 25, 0);
            z_mesh.AddSegment(origin, ymax, new Vector3D(1, 0, 0), 0.1);

            Vector3D vY = ymax - origin;
            vY.Normalize();
            Vector3D perpY = Vector3D.CrossProduct(vY, new Vector3D(1, 0, 0));
            perpX.Normalize();
            Vector3D v1Y = ScaleVector(-vY + perpY, 1);
            Vector3D v2Y = ScaleVector(-vY - perpY, 1);
            z_mesh.AddSegment(ymax, ymax + v1Y, new Vector3D(0, 1, 0), 0.1);
            z_mesh.AddSegment(ymax, ymax + v2Y, new Vector3D(0, 1, 0), 0.1);

            Vector3D perpY1 = Vector3D.CrossProduct(vY, new Vector3D(0, 0, 1));
            perpX1.Normalize();
            Vector3D v1Y1 = ScaleVector(-vY + perpY1, 1);
            Vector3D v2Y1 = ScaleVector(-vY - perpY1, 1);
            z_mesh.AddSegment(ymax, ymax + v1Y1, new Vector3D(0, 0, 1), 0.1);
            z_mesh.AddSegment(ymax, ymax + v2Y1, new Vector3D(0, 0, 1), 0.1);

            axes_brush = Brushes.Blue;
            DiffuseMaterial z_axis_material = new DiffuseMaterial(axes_brush);

            static_Z_axis_model = new GeometryModel3D(z_mesh, z_axis_material);


            //X (normal: Z):
            Point3D zmax = new Point3D(0, 0, 25);
            x_mesh.AddSegment(origin, zmax, new Vector3D(0, 1, 0), 0.1);

            Vector3D vZ = zmax - origin;
            vZ.Normalize();
            Vector3D perpZ = Vector3D.CrossProduct(vZ, new Vector3D(0, 1, 0));
            perpX.Normalize();
            Vector3D v1Z = ScaleVector(-vZ + perpZ, 1);
            Vector3D v2Z = ScaleVector(-vZ - perpZ, 1);
            x_mesh.AddSegment(zmax, zmax + v1Z, new Vector3D(0, 1, 0), 0.1);
            x_mesh.AddSegment(zmax, zmax + v2Z, new Vector3D(0, 1, 0), 0.1);

            Vector3D perpZ1 = Vector3D.CrossProduct(vZ, new Vector3D(1, 0, 0));
            perpX1.Normalize();
            Vector3D v1Z1 = ScaleVector(-vZ + perpZ1, 1);
            Vector3D v2Z1 = ScaleVector(-vZ - perpZ1, 1);
            x_mesh.AddSegment(zmax, zmax + v1Z1, new Vector3D(1, 0, 0), 0.1);
            x_mesh.AddSegment(zmax, zmax + v2Z1, new Vector3D(1, 0, 0), 0.1);

            axes_brush = Brushes.Red;
            DiffuseMaterial x_axis_material = new DiffuseMaterial(axes_brush);

            static_X_axis_model = new GeometryModel3D(x_mesh, x_axis_material);

            //static_axes_model = new GeometryModel3D(axes_mesh, axes_material);
        }

        private static Vector3D ScaleVector(Vector3D vector, double length)
        {
            double scale = length / vector.Length;
            return new Vector3D(
                vector.X * scale,
                vector.Y * scale,
                vector.Z * scale);
        }

        public static void DrawPolyhedra(Model3DGroup PolyhedraGroup, CrystalCell atomCell, List<Atom> multipliedAtoms, ref List<GeometryModel3D> SelectableModels, ref List<List<Atom>> atomsList)
        {
            Polyhedra.CalculatePolyhedra(multipliedAtoms, atomCell.YAxisL, atomCell.ZAxisL, atomCell.XAxisL);

            foreach (Atom atom in multipliedAtoms.Where(a => a.hasPolyhedra))
            {
                MeshGeometry3D polyhedraMesh = new MeshGeometry3D();
                var polyhedraModel = polyhedraMesh.DrawSinglePolyhedra(atom, atomCell, 4);
                //SelectableModels.Add(polyhedraModel);
            }
        }

        public static void Translate_Cell(string direction, Model3DGroup Upper_Cell_Model, Model3DGroup cells_and_atoms, CrystalCell atomCell, Model3DGroup TranslationsGroup)
        {
            Upper_Cell_Model = cells_and_atoms.Clone();
            var transform = new TranslateTransform3D();

            switch (direction)
            {
                case "up":
                    transform = new TranslateTransform3D(0, atomCell.ZAxisL, 0);
                    break;
                case "down":
                    transform = new TranslateTransform3D(0, -atomCell.ZAxisL, 0);
                    break;
                case "right":
                    transform = new TranslateTransform3D(atomCell.YAxisL, 0, 0);
                    break;
                case "left":
                    transform = new TranslateTransform3D(-atomCell.YAxisL, 0, 0);
                    break;
                case "front":
                    transform = new TranslateTransform3D(0, 0, -atomCell.XAxisL);
                    break;
                case "back":
                    transform = new TranslateTransform3D(0, 0, atomCell.XAxisL);
                    break;
                default:
                    break;
            }

            Upper_Cell_Model.Transform = transform;

            TranslationsGroup.Children.Add(Upper_Cell_Model);
        }

        public static void Translate_Polyhedra(Model3DGroup polyhedraGroup, CrystalCell atomCell)
        {
            //TODO
            int count = polyhedraGroup.Children.Count;
            for (int i = 0; i < count; i++)
            {
                var translatedPolyhedra = polyhedraGroup.Children[i].Clone();
                translatedPolyhedra.Transform = new TranslateTransform3D(0, atomCell.ZAxisL, 0);
                polyhedraGroup.Children.Add(translatedPolyhedra);

            }
        }


        public static void visualizeAtom(Model3DGroup model_group, Atom atom, string atomColor, SpaceGroupCl selectedSpaceGroup, CrystalCell atomCell,
            ref List<GeometryModel3D> SelectableModels, ref List<Atom> multipliedAtoms, ref List<List<Atom>> atomsList, ref List<Model3DGroup> atomReproductions)
        {
            if (selectedSpaceGroup == null)
            {
                throw new NotImplementedException();
            }


            //atom.StringToColor();
            //atomColor = atom.Brush;
            SolidColorBrush brush1 = (SolidColorBrush)(new BrushConverter().ConvertFrom(atomColor));

            double atomSize = 0.4;
            if (atom.Element[0] == 'O' && (atom.Element[1] != 's'))
            {
                atomSize = 0.2;
               // brush1.Color = Color.FromRgb(255, 0, 0); //oxygen to red
            }
            //else
            //{
            //    brush1 = atom.Brush;
            //    brush1 = new SolidColorBrush(Color.FromArgb(255, 255, 139, 0));
            //}

            Model3DGroup atomRepro = new Model3DGroup();
            List<Atom> atomReproList = new List<Atom>();

            for (int i = 0; i < selectedSpaceGroup.Expressions.Length; i += 3)
            {
                MeshGeometry3D mesh1 = new MeshGeometry3D();
                double X = SpaceGroupCl.Evaluate(selectedSpaceGroup.Expressions[i + 1], 0, atom.Y, 0);
                double x = X * atomCell.YAxisL; //Y
                double Y = SpaceGroupCl.Evaluate(selectedSpaceGroup.Expressions[i + 2], 0, 0, atom.Z);
                double y = Y * atomCell.ZAxisL; //Z
                double Z = SpaceGroupCl.Evaluate(selectedSpaceGroup.Expressions[i], atom.X, 0, 0);
                double z = Z * atomCell.XAxisL; //X

                if (x < 0)
                    x += atomCell.YAxisL;
                if (y < 0)
                    y += atomCell.ZAxisL;
                if (z < 0)
                    z += atomCell.XAxisL;

                if (X < 0)
                    X += 1;
                if (Y < 0)
                    Y += 1;
                if (Z < 0)
                    Z += 1;


                //???

                if (x > atomCell.YAxisL)
                    x -= atomCell.YAxisL;
                if (y > atomCell.ZAxisL)
                    y -= atomCell.ZAxisL;
                if (z > atomCell.XAxisL)
                    z -= atomCell.XAxisL;

                if (X > 1)
                    X -= 1;
                if (Y > 1)
                    Y -= 1;
                if (Z > 1)
                    Z -= 1;

                if (!multipliedAtoms.Contains(new Atom(atom.Element, Z.ToString(), X.ToString(), Y.ToString(), null)))
                {
                    mesh1.AddSphere(new Point3D(x, y, z), atomSize, 20, 30);
                    DiffuseMaterial material1 = new DiffuseMaterial(brush1);
                    GeometryModel3D model1 = new GeometryModel3D(mesh1, material1);
                    atomRepro.Children.Add(model1);
                    SelectableModels.Add(model1);
                    //multipliedAtoms.Add(new Atom(atom.Element, atom., x.ToString(), y.ToString(), atomColor)); //NULLPTREXCEPTION
                    multipliedAtoms.Add(new Atom(atom.Element, Z.ToString(), X.ToString(), Y.ToString(), null)); //NULLPTREXCEPTION
                    atomReproList.Add(new Atom(atom.Element, Z.ToString(), X.ToString(), Y.ToString(), atom.Brush));
                }

                else
                {
                    Console.WriteLine("Found duplicate");
                }
            }
            atomsList.Add(atomReproList);
            atomReproductions.Add(atomRepro);
            model_group.Children.Add(atomRepro);
        }

        public static void buildDiscreteAxis(out GeometryModel3D discrete_y_axis_model, out GeometryModel3D discrete_x_axis_model, out GeometryModel3D discrete_z_axis_model, CrystalCell crystalCell)
        {
            MeshGeometry3D x_mesh = new MeshGeometry3D();
            MeshGeometry3D y_mesh = new MeshGeometry3D();
            MeshGeometry3D z_mesh = new MeshGeometry3D();
            Point3D origin = new Point3D(0, 0, 0);

            const double length = 5;

            double a = length;
            double b = length;
            double c = length;

            //Y (normal: X):

            Point3D xmax = new Point3D(5, 0, 0);
            y_mesh.AddSegment(origin, xmax, new Vector3D(0, 1, 0), 0.1);

            Vector3D vX = xmax - origin;
            vX.Normalize();
            Vector3D perpX = Vector3D.CrossProduct(vX, new Vector3D(0, 1, 0));
            perpX.Normalize();
            Vector3D v1X = ScaleVector(-vX + perpX, 1);
            Vector3D v2X = ScaleVector(-vX - perpX, 1);
            y_mesh.AddSegment(xmax, xmax + v1X, new Vector3D(0, 1, 0), 0.1);
            y_mesh.AddSegment(xmax, xmax + v2X, new Vector3D(0, 1, 0), 0.1);

            Vector3D perpX1 = Vector3D.CrossProduct(vX, new Vector3D(0, 0, 1));
            perpX1.Normalize();
            Vector3D v1X1 = ScaleVector(-vX + perpX1, 1);
            Vector3D v2X1 = ScaleVector(-vX - perpX1, 1);
            y_mesh.AddSegment(xmax, xmax + v1X1, new Vector3D(0, 0, 1), 0.1);
            y_mesh.AddSegment(xmax, xmax + v2X1, new Vector3D(0, 0, 1), 0.1);

            SolidColorBrush axes_brush = Brushes.Green;
            DiffuseMaterial y_axis_material = new DiffuseMaterial(axes_brush);

            discrete_y_axis_model = new GeometryModel3D(y_mesh, y_axis_material);

            //Z (normal: Y):

            double x_c = c * Math.Cos(toRadians(crystalCell.Beta));

            double y_c = (b * c * Math.Cos(toRadians(crystalCell.Alpha)) - x_c * b * Math.Cos(toRadians(crystalCell.Gamma)))
                /b * Math.Sin(toRadians(crystalCell.Gamma));

            
            //double z_c = c * c - x_c * x_c - y_c * y_c;
            
            Point3D ymax = new Point3D(x_c, 5, y_c);
            z_mesh.AddSegment(origin, ymax, new Vector3D(1, 0, 0), 0.1);

            Vector3D vY = ymax - origin;
            vY.Normalize();
            Vector3D perpY = Vector3D.CrossProduct(vY, new Vector3D(1, 0, 0));
            perpX.Normalize();
            Vector3D v1Y = ScaleVector(-vY + perpY, 1);
            Vector3D v2Y = ScaleVector(-vY - perpY, 1);
            z_mesh.AddSegment(ymax, ymax + v1Y, new Vector3D(0, 1, 0), 0.1);
            z_mesh.AddSegment(ymax, ymax + v2Y, new Vector3D(0, 1, 0), 0.1);

            Vector3D perpY1 = Vector3D.CrossProduct(vY, new Vector3D(0, 0, 1));
            perpX1.Normalize();
            Vector3D v1Y1 = ScaleVector(-vY + perpY1, 1);
            Vector3D v2Y1 = ScaleVector(-vY - perpY1, 1);
            z_mesh.AddSegment(ymax, ymax + v1Y1, new Vector3D(0, 0, 1), 0.1);
            z_mesh.AddSegment(ymax, ymax + v2Y1, new Vector3D(0, 0, 1), 0.1);

            axes_brush = Brushes.Blue;
            DiffuseMaterial z_axis_material = new DiffuseMaterial(axes_brush);

            discrete_z_axis_model = new GeometryModel3D(z_mesh, z_axis_material);


            //X (normal: Z):
            Point3D zmax = new Point3D(b * Math.Cos(toRadians(crystalCell.Gamma)), 0, b * Math.Sin(crystalCell.Gamma));
            x_mesh.AddSegment(origin, zmax, new Vector3D(1, 1, 0), 0.1);

            Vector3D vZ = zmax - origin;
            vZ.Normalize();
            Vector3D perpZ = Vector3D.CrossProduct(vZ, new Vector3D(0, 1, 0));
            perpX.Normalize();
            Vector3D v1Z = ScaleVector(-vZ + perpZ, 1);
            Vector3D v2Z = ScaleVector(-vZ - perpZ, 1);
            x_mesh.AddSegment(zmax, zmax + v1Z, new Vector3D(0, 1, 0), 0.1);
            x_mesh.AddSegment(zmax, zmax + v2Z, new Vector3D(0, 1, 0), 0.1);

            Vector3D perpZ1 = Vector3D.CrossProduct(vZ, new Vector3D(1, 0, 0));
            perpX1.Normalize();
            Vector3D v1Z1 = ScaleVector(-vZ + perpZ1, 1);
            Vector3D v2Z1 = ScaleVector(-vZ - perpZ1, 1);
            x_mesh.AddSegment(zmax, zmax + v1Z1, new Vector3D(1, 0, 0), 0.1);
            x_mesh.AddSegment(zmax, zmax + v2Z1, new Vector3D(1, 0, 0), 0.1);

            axes_brush = Brushes.Red;
            DiffuseMaterial x_axis_material = new DiffuseMaterial(axes_brush);

            discrete_x_axis_model = new GeometryModel3D(x_mesh, x_axis_material);
        }

        public static double toRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }
    }
}
