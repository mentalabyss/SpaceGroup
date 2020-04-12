using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace SpaceGroup
{
    public static class ModelBuilder
    {
        public static void buildDiscreteAxis(out GeometryModel3D discrete_y_axis_model, out GeometryModel3D discrete_x_axis_model, out GeometryModel3D discrete_z_axis_model)
        {
            MeshGeometry3D x_mesh = new MeshGeometry3D();
            MeshGeometry3D y_mesh = new MeshGeometry3D();
            MeshGeometry3D z_mesh = new MeshGeometry3D();
            Point3D origin = new Point3D(0, 0, 0);


            //Y (normal: X):
            Point3D xmax = new Point3D(5, 0, 0);
            AddSegment(y_mesh, origin, xmax, new Vector3D(0, 1, 0), 1);

            Vector3D vX = xmax - origin;
            vX.Normalize();
            Vector3D perpX = Vector3D.CrossProduct(vX, new Vector3D(0, 1, 0));
            perpX.Normalize();
            Vector3D v1X = ScaleVector(-vX + perpX, 1);
            Vector3D v2X = ScaleVector(-vX - perpX, 1);
            AddSegment(y_mesh, xmax, xmax + v1X, new Vector3D(0, 1, 0), 1);
            AddSegment(y_mesh, xmax, xmax + v2X, new Vector3D(0, 1, 0), 1);

            Vector3D perpX1 = Vector3D.CrossProduct(vX, new Vector3D(0, 0, 1));
            perpX1.Normalize();
            Vector3D v1X1 = ScaleVector(-vX + perpX1, 1);
            Vector3D v2X1 = ScaleVector(-vX - perpX1, 1);
            AddSegment(y_mesh, xmax, xmax + v1X1, new Vector3D(0, 0, 1), 1);
            AddSegment(y_mesh, xmax, xmax + v2X1, new Vector3D(0, 0, 1), 1);

            SolidColorBrush axes_brush = Brushes.Green;
            DiffuseMaterial y_axis_material = new DiffuseMaterial(axes_brush);

            discrete_y_axis_model = new GeometryModel3D(y_mesh, y_axis_material);

            //Z (normal: Y):
            Point3D ymax = new Point3D(0, 5, 0);
            AddSegment(z_mesh, origin, ymax, new Vector3D(1, 0, 0), 1);

            Vector3D vY = ymax - origin;
            vY.Normalize();
            Vector3D perpY = Vector3D.CrossProduct(vY, new Vector3D(1, 0, 0));
            perpX.Normalize();
            Vector3D v1Y = ScaleVector(-vY + perpY, 1);
            Vector3D v2Y = ScaleVector(-vY - perpY, 1);
            AddSegment(z_mesh, ymax, ymax + v1Y, new Vector3D(0, 1, 0), 1);
            AddSegment(z_mesh, ymax, ymax + v2Y, new Vector3D(0, 1, 0), 1);

            Vector3D perpY1 = Vector3D.CrossProduct(vY, new Vector3D(0, 0, 1));
            perpX1.Normalize();
            Vector3D v1Y1 = ScaleVector(-vY + perpY1, 1);
            Vector3D v2Y1 = ScaleVector(-vY - perpY1, 1);
            AddSegment(z_mesh, ymax, ymax + v1Y1, new Vector3D(0, 0, 1), 1);
            AddSegment(z_mesh, ymax, ymax + v2Y1, new Vector3D(0, 0, 1), 1);

            axes_brush = Brushes.Blue;
            DiffuseMaterial z_axis_material = new DiffuseMaterial(axes_brush);

            discrete_z_axis_model = new GeometryModel3D(z_mesh, z_axis_material);


            //X (normal: Z):
            Point3D zmax = new Point3D(0, 0, 5);
            AddSegment(x_mesh, origin, zmax, new Vector3D(0, 1, 0), 1);

            Vector3D vZ = zmax - origin;
            vZ.Normalize();
            Vector3D perpZ = Vector3D.CrossProduct(vZ, new Vector3D(0, 1, 0));
            perpX.Normalize();
            Vector3D v1Z = ScaleVector(-vZ + perpZ, 1);
            Vector3D v2Z = ScaleVector(-vZ - perpZ, 1);
            AddSegment(x_mesh, zmax, zmax + v1Z, new Vector3D(0, 1, 0), 1);
            AddSegment(x_mesh, zmax, zmax + v2Z, new Vector3D(0, 1, 0), 1);

            Vector3D perpZ1 = Vector3D.CrossProduct(vZ, new Vector3D(1, 0, 0));
            perpX1.Normalize();
            Vector3D v1Z1 = ScaleVector(-vZ + perpZ1, 1);
            Vector3D v2Z1 = ScaleVector(-vZ - perpZ1, 1);
            AddSegment(x_mesh, zmax, zmax + v1Z1, new Vector3D(1, 0, 0), 1);
            AddSegment(x_mesh, zmax, zmax + v2Z1, new Vector3D(1, 0, 0), 1);

            axes_brush = Brushes.Red;
            DiffuseMaterial x_axis_material = new DiffuseMaterial(axes_brush);

            discrete_x_axis_model = new GeometryModel3D(x_mesh, x_axis_material);
        }

        public static void buildCellBorders(out GeometryModel3D borders_model, CrystalCell atomCell)
        {
            MeshGeometry3D borders_mesh = new MeshGeometry3D();
            Point3D origin = new Point3D(0, 0, 0);
            Point3D xLine = new Point3D(atomCell.YAxisL, 0, 0);
            Point3D yLine = new Point3D(0, atomCell.ZAxisL, 0);
            Point3D zLine = new Point3D(0, 0, atomCell.XAxisL);

            AddSegment(borders_mesh, origin, xLine, new Vector3D(0, 1, 0), 0.04);
            AddSegment(borders_mesh, origin, yLine, new Vector3D(1, 0, 0), 0.04);
            AddSegment(borders_mesh, origin, zLine, new Vector3D(0, 1, 0), 0.04);

            AddSegment(borders_mesh, new Point3D(atomCell.YAxisL, 0, 0), new Point3D(atomCell.YAxisL, 0, atomCell.XAxisL), new Vector3D(0, 1, 0), 0.04);
            AddSegment(borders_mesh, new Point3D(atomCell.YAxisL, atomCell.ZAxisL, 0), new Point3D(atomCell.YAxisL, atomCell.ZAxisL, atomCell.XAxisL), new Vector3D(0, 1, 0), 0.04);
            AddSegment(borders_mesh, new Point3D(0, atomCell.ZAxisL, 0), new Point3D(0, atomCell.ZAxisL, atomCell.XAxisL), new Vector3D(0, 1, 0), 0.04);


            AddSegment(borders_mesh, new Point3D(0, 0, atomCell.XAxisL), new Point3D(atomCell.YAxisL, 0, atomCell.XAxisL), new Vector3D(0, 1, 0), 0.04);
            AddSegment(borders_mesh, new Point3D(0, atomCell.ZAxisL, atomCell.XAxisL), new Point3D(atomCell.YAxisL, atomCell.ZAxisL, atomCell.XAxisL), new Vector3D(0, 1, 0), 0.04);
            AddSegment(borders_mesh, new Point3D(0, atomCell.ZAxisL, 0), new Point3D(atomCell.YAxisL, atomCell.ZAxisL, 0), new Vector3D(0, 1, 0), 0.04);


            AddSegment(borders_mesh, new Point3D(atomCell.YAxisL, 0, 0), new Point3D(atomCell.YAxisL, atomCell.ZAxisL, 0), new Vector3D(1, 0, 0), 0.04);
            AddSegment(borders_mesh, new Point3D(0, 0, atomCell.XAxisL), new Point3D(0, atomCell.ZAxisL, atomCell.XAxisL), new Vector3D(1, 0, 0), 0.04);
            AddSegment(borders_mesh, new Point3D(atomCell.YAxisL, 0, atomCell.XAxisL), new Point3D(atomCell.YAxisL, atomCell.ZAxisL, atomCell.XAxisL), new Vector3D(1, 0, 0), 0.04);




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
            AddSegment(y_mesh, origin, xmax, new Vector3D(0, 1, 0), 0.1);

            Vector3D vX = xmax - origin;
            vX.Normalize();
            Vector3D perpX = Vector3D.CrossProduct(vX, new Vector3D(0, 1, 0));
            perpX.Normalize();
            Vector3D v1X = ScaleVector(-vX + perpX, 1);
            Vector3D v2X = ScaleVector(-vX - perpX, 1);
            AddSegment(y_mesh, xmax, xmax + v1X, new Vector3D(0, 1, 0), 0.1);
            AddSegment(y_mesh, xmax, xmax + v2X, new Vector3D(0, 1, 0), 0.1);

            Vector3D perpX1 = Vector3D.CrossProduct(vX, new Vector3D(0, 0, 1));
            perpX1.Normalize();
            Vector3D v1X1 = ScaleVector(-vX + perpX1, 1);
            Vector3D v2X1 = ScaleVector(-vX - perpX1, 1);
            AddSegment(y_mesh, xmax, xmax + v1X1, new Vector3D(0, 0, 1), 0.1);
            AddSegment(y_mesh, xmax, xmax + v2X1, new Vector3D(0, 0, 1), 0.1);

            SolidColorBrush axes_brush = Brushes.Green;
            DiffuseMaterial y_axis_material = new DiffuseMaterial(axes_brush);

            static_Y_axis_model = new GeometryModel3D(y_mesh, y_axis_material);

            //Z (normal: Y):
            Point3D ymax = new Point3D(0, 25, 0);
            AddSegment(z_mesh, origin, ymax, new Vector3D(1, 0, 0), 0.1);

            Vector3D vY = ymax - origin;
            vY.Normalize();
            Vector3D perpY = Vector3D.CrossProduct(vY, new Vector3D(1, 0, 0));
            perpX.Normalize();
            Vector3D v1Y = ScaleVector(-vY + perpY, 1);
            Vector3D v2Y = ScaleVector(-vY - perpY, 1);
            AddSegment(z_mesh, ymax, ymax + v1Y, new Vector3D(0, 1, 0), 0.1);
            AddSegment(z_mesh, ymax, ymax + v2Y, new Vector3D(0, 1, 0), 0.1);

            Vector3D perpY1 = Vector3D.CrossProduct(vY, new Vector3D(0, 0, 1));
            perpX1.Normalize();
            Vector3D v1Y1 = ScaleVector(-vY + perpY1, 1);
            Vector3D v2Y1 = ScaleVector(-vY - perpY1, 1);
            AddSegment(z_mesh, ymax, ymax + v1Y1, new Vector3D(0, 0, 1), 0.1);
            AddSegment(z_mesh, ymax, ymax + v2Y1, new Vector3D(0, 0, 1), 0.1);

            axes_brush = Brushes.Blue;
            DiffuseMaterial z_axis_material = new DiffuseMaterial(axes_brush);

            static_Z_axis_model = new GeometryModel3D(z_mesh, z_axis_material);


            //X (normal: Z):
            Point3D zmax = new Point3D(0, 0, 25);
            AddSegment(x_mesh, origin, zmax, new Vector3D(0, 1, 0), 0.1);

            Vector3D vZ = zmax - origin;
            vZ.Normalize();
            Vector3D perpZ = Vector3D.CrossProduct(vZ, new Vector3D(0, 1, 0));
            perpX.Normalize();
            Vector3D v1Z = ScaleVector(-vZ + perpZ, 1);
            Vector3D v2Z = ScaleVector(-vZ - perpZ, 1);
            AddSegment(x_mesh, zmax, zmax + v1Z, new Vector3D(0, 1, 0), 0.1);
            AddSegment(x_mesh, zmax, zmax + v2Z, new Vector3D(0, 1, 0), 0.1);

            Vector3D perpZ1 = Vector3D.CrossProduct(vZ, new Vector3D(1, 0, 0));
            perpX1.Normalize();
            Vector3D v1Z1 = ScaleVector(-vZ + perpZ1, 1);
            Vector3D v2Z1 = ScaleVector(-vZ - perpZ1, 1);
            AddSegment(x_mesh, zmax, zmax + v1Z1, new Vector3D(1, 0, 0), 0.1);
            AddSegment(x_mesh, zmax, zmax + v2Z1, new Vector3D(1, 0, 0), 0.1);

            axes_brush = Brushes.Red;
            DiffuseMaterial x_axis_material = new DiffuseMaterial(axes_brush);

            static_X_axis_model = new GeometryModel3D(x_mesh, x_axis_material);

            //static_axes_model = new GeometryModel3D(axes_mesh, axes_material);
        }

        public static void AddSegment(MeshGeometry3D mesh, Point3D point1, Point3D point2, Vector3D up, double thickness)
        {
            //const double thickness = 0.04;

            // Get the segment's vector.
            Vector3D v = point2 - point1;

            // Get the scaled up vector.
            Vector3D n1 = ScaleVector(up, thickness / 2.0);

            // Get another scaled perpendicular vector.
            Vector3D n2 = Vector3D.CrossProduct(v, n1);
            n2 = ScaleVector(n2, thickness / 2.0);

            // Make a skinny box.
            // p1pm means point1 PLUS n1 MINUS n2.
            Point3D p1pp = point1 + n1 + n2;
            Point3D p1mp = point1 - n1 + n2;
            Point3D p1pm = point1 + n1 - n2;
            Point3D p1mm = point1 - n1 - n2;
            Point3D p2pp = point2 + n1 + n2;
            Point3D p2mp = point2 - n1 + n2;
            Point3D p2pm = point2 + n1 - n2;
            Point3D p2mm = point2 - n1 - n2;

            // Sides.
            AddTriangle(mesh, p1pp, p1mp, p2mp);
            AddTriangle(mesh, p1pp, p2mp, p2pp);

            AddTriangle(mesh, p1pp, p2pp, p2pm);
            AddTriangle(mesh, p1pp, p2pm, p1pm);

            AddTriangle(mesh, p1pm, p2pm, p2mm);
            AddTriangle(mesh, p1pm, p2mm, p1mm);

            AddTriangle(mesh, p1mm, p2mm, p2mp);
            AddTriangle(mesh, p1mm, p2mp, p1mp);

            // Ends.
            AddTriangle(mesh, p1pp, p1pm, p1mm);
            AddTriangle(mesh, p1pp, p1mm, p1mp);

            AddTriangle(mesh, p2pp, p2mp, p2mm);
            AddTriangle(mesh, p2pp, p2mm, p2pm);
        }

        private static Vector3D ScaleVector(Vector3D vector, double length)
        {
            double scale = length / vector.Length;
            return new Vector3D(
                vector.X * scale,
                vector.Y * scale,
                vector.Z * scale);
        }

        private static void AddTriangle(MeshGeometry3D mesh, Point3D point1, Point3D point2, Point3D point3)
        {
            // Create the points.
            int index1 = mesh.Positions.Count;
            mesh.Positions.Add(point1);
            mesh.Positions.Add(point2);
            mesh.Positions.Add(point3);

            // Create the triangle.
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1);
        }

        public static void DrawPolyhedra(ref Model3DGroup MainModel3Dgroup, ref GeometryModel3D polyhedra_model, CrystalCell atomCell, List<Atom> multipliedAtoms)
        {
            List<Atom> oxygens = Polyhedra.CalculatePolyhedra(multipliedAtoms, atomCell.YAxisL, atomCell.ZAxisL, atomCell.XAxisL);
            MeshGeometry3D polyhedra_mesh = new MeshGeometry3D();
            for (int i = 0; i < oxygens.Count; i += 4)
            {
                Point3D point0 = new Point3D(oxygens[i].Y * atomCell.YAxisL, oxygens[i].Z * atomCell.ZAxisL, oxygens[i].X * atomCell.XAxisL);
                Point3D point1 = new Point3D(oxygens[i + 1].Y * atomCell.YAxisL, oxygens[i + 1].Z * atomCell.ZAxisL, oxygens[i + 1].X * atomCell.XAxisL);
                Point3D point2 = new Point3D(oxygens[i + 2].Y * atomCell.YAxisL, oxygens[i + 2].Z * atomCell.ZAxisL, oxygens[i + 2].X * atomCell.XAxisL);
                Point3D point3 = new Point3D(oxygens[i + 3].Y * atomCell.YAxisL, oxygens[i + 3].Z * atomCell.ZAxisL, oxygens[i + 3].X * atomCell.XAxisL);

                ModelBuilder.AddSegment(polyhedra_mesh, point0, point1, new Vector3D(0, 1, 0), 0.04);
                ModelBuilder.AddSegment(polyhedra_mesh, point0, point2, new Vector3D(0, 1, 0), 0.04);
                ModelBuilder.AddSegment(polyhedra_mesh, point0, point3, new Vector3D(0, 1, 0), 0.04);
                ModelBuilder.AddSegment(polyhedra_mesh, point1, point3, new Vector3D(0, 1, 0), 0.04);
                ModelBuilder.AddSegment(polyhedra_mesh, point1, point2, new Vector3D(0, 1, 0), 0.04);
                ModelBuilder.AddSegment(polyhedra_mesh, point2, point3, new Vector3D(0, 1, 0), 0.04);

                SolidColorBrush borders_brush = Brushes.Black;
                DiffuseMaterial borders_material = new DiffuseMaterial(borders_brush);
                polyhedra_model = new GeometryModel3D(polyhedra_mesh, borders_material);
                MainModel3Dgroup.Children.Add(polyhedra_model);
            }
        }

        public static void Translate_Cell(string direction, ref Model3DGroup Upper_Cell_Model, ref Model3DGroup cells_and_atoms, CrystalCell atomCell, ref Model3DGroup TranslationsGroup)
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

        private static void AddSphere(MeshGeometry3D mesh, Point3D center, double radius, int num_phi, int num_theta)
        {
            double phi0, theta0;
            double dphi = Math.PI / num_phi;
            double dtheta = 2 * Math.PI / num_theta;

            phi0 = 0;
            double y0 = radius * Math.Cos(phi0);
            double r0 = radius * Math.Sin(phi0);
            for (int i = 0; i < num_phi; i++)
            {
                double phi1 = phi0 + dphi;
                double y1 = radius * Math.Cos(phi1);
                double r1 = radius * Math.Sin(phi1);

                // Point ptAB has phi value A and theta value B.
                // For example, pt01 has phi = phi0 and theta = theta1.
                // Find the points with theta = theta0.
                theta0 = 0;
                Point3D pt00 = new Point3D(
                    center.X + r0 * Math.Cos(theta0),
                    center.Y + y0,
                    center.Z + r0 * Math.Sin(theta0));
                Point3D pt10 = new Point3D(
                    center.X + r1 * Math.Cos(theta0),
                    center.Y + y1,
                    center.Z + r1 * Math.Sin(theta0));
                for (int j = 0; j < num_theta; j++)
                {
                    // Find the points with theta = theta1.
                    double theta1 = theta0 + dtheta;
                    Point3D pt01 = new Point3D(
                        center.X + r0 * Math.Cos(theta1),
                        center.Y + y0,
                        center.Z + r0 * Math.Sin(theta1));
                    Point3D pt11 = new Point3D(
                        center.X + r1 * Math.Cos(theta1),
                        center.Y + y1,
                        center.Z + r1 * Math.Sin(theta1));

                    // Create the triangles.
                    AddTriangle(mesh, pt00, pt11, pt10);
                    AddTriangle(mesh, pt00, pt01, pt11);

                    // Move to the next value of theta.
                    theta0 = theta1;
                    pt00 = pt01;
                    pt10 = pt11;
                }

                // Move to the next value of phi.
                phi0 = phi1;
                y0 = y1;
                r0 = r1;
            }

        }

        public static void visualizeAtom(ref Model3DGroup model_group, Atom atom, SolidColorBrush atomColor, SpaceGroupCl selectedSpaceGroup, CrystalCell atomCell,
            ref List<GeometryModel3D> SelectableModels, ref List<Atom> multipliedAtoms, ref List<List<Atom>> atomsList, ref List<Model3DGroup> atomReproductions)
        {
            atom.StringToColor();
            atomColor = atom.Brush;
            SolidColorBrush brush1 = new SolidColorBrush();

            double atomSize = 0.7;
            if (atom.Element[0] == 'O' && (atom.Element[1] != 's'))
            {
                atomSize = 0.3;
                brush1.Color = Color.FromRgb(255, 0, 0); //oxygen to red
            }
            else
            {
                brush1 = atom.Brush;
            }

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

                AddSphere(mesh1, new Point3D(x, y, z), atomSize, 20, 30);
                //AddSphere(mesh1, new Point3D(-1, 0, 0), 0.25, 5, 10);
                DiffuseMaterial material1 = new DiffuseMaterial(brush1);
                GeometryModel3D model1 = new GeometryModel3D(mesh1, material1);
                atomRepro.Children.Add(model1);
                SelectableModels.Add(model1);
                //multipliedAtoms.Add(new Atom(atom.Element, atom., x.ToString(), y.ToString(), atomColor)); //NULLPTREXCEPTION
                multipliedAtoms.Add(new Atom(atom.Element, Z.ToString(), X.ToString(), Y.ToString(), atomColor)); //NULLPTREXCEPTION
                atomReproList.Add(new Atom(atom.Element, Z.ToString(), X.ToString(), Y.ToString(), atom.Brush));
            }
            atomsList.Add(atomReproList);
            atomReproductions.Add(atomRepro);
            model_group.Children.Add(atomRepro);
        }
    }
}
