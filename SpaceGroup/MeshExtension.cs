using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace SpaceGroup
{
    public static class MeshExtension
    {
        public static GeometryModel3D DrawSinglePolyhedra(this MeshGeometry3D polyhedraMesh, Atom atom, CrystalCell atomCell, int nOfVertices)
        {

            if (nOfVertices > 4)
            {

                // GENERAL CASE FOR N > 4 (CONVEX HULL)

                List<Point3D> polyhedraPoints = new List<Point3D>();

                foreach (Atom polyhedraAtom in atom.PolyhedraAtoms)
                {
                    Point3D point = new Point3D(polyhedraAtom.Y * atomCell.YAxisL, polyhedraAtom.Z * atomCell.ZAxisL, polyhedraAtom.X * atomCell.XAxisL);
                }

                return null;
            }
            
            // GENERAL CASE//

            int i = 0;
            Point3D point0 = new Point3D(atom.PolyhedraAtoms[i].Y * atomCell.YAxisL, atom.PolyhedraAtoms[i].Z * atomCell.ZAxisL, atom.PolyhedraAtoms[i].X * atomCell.XAxisL);
            Point3D point1 = new Point3D(atom.PolyhedraAtoms[i + 1].Y * atomCell.YAxisL, atom.PolyhedraAtoms[i + 1].Z * atomCell.ZAxisL, atom.PolyhedraAtoms[i + 1].X * atomCell.XAxisL);
            Point3D point2 = new Point3D(atom.PolyhedraAtoms[i + 2].Y * atomCell.YAxisL, atom.PolyhedraAtoms[i + 2].Z * atomCell.ZAxisL, atom.PolyhedraAtoms[i + 2].X * atomCell.XAxisL);
            Point3D point3 = new Point3D(atom.PolyhedraAtoms[i + 3].Y * atomCell.YAxisL, atom.PolyhedraAtoms[i + 3].Z * atomCell.ZAxisL, atom.PolyhedraAtoms[i + 3].X * atomCell.XAxisL);

            AddSegment(polyhedraMesh, point0, point1, new Vector3D(0, 1, 0), 0.04);
            AddSegment(polyhedraMesh, point0, point2, new Vector3D(0, 1, 0), 0.04);
            AddSegment(polyhedraMesh, point0, point3, new Vector3D(0, 1, 0), 0.04);
            AddSegment(polyhedraMesh, point1, point3, new Vector3D(0, 1, 0), 0.04);
            AddSegment(polyhedraMesh, point1, point2, new Vector3D(0, 1, 0), 0.04);
            AddSegment(polyhedraMesh, point2, point3, new Vector3D(0, 1, 0), 0.04);

            AddTriangle(polyhedraMesh, point0, point1, point2);
            AddTriangle(polyhedraMesh, point0, point1, point3);
            AddTriangle(polyhedraMesh, point1, point2, point3);
            AddTriangle(polyhedraMesh, point0, point2, point3);


            DiffuseMaterial qDiffTransYellow =
            new DiffuseMaterial(new SolidColorBrush(System.Windows.Media.Color.FromArgb(64, 255, 255, 0)));

            SpecularMaterial qSpecTransWhite =
           new SpecularMaterial(new SolidColorBrush(System.Windows.Media.Color.FromArgb(128, 255, 255, 255)), 30.0);
            MaterialGroup qOuterMaterial = new MaterialGroup();
            qOuterMaterial.Children.Add(qDiffTransYellow);
            qOuterMaterial.Children.Add(qSpecTransWhite);

            GeometryModel3D polyhedraModel = new GeometryModel3D(polyhedraMesh, qOuterMaterial);
            polyhedraModel.BackMaterial = qOuterMaterial;

            return polyhedraModel;
        }

        public static void AddSegment(this MeshGeometry3D mesh, Point3D point1, Point3D point2, Vector3D up, double thickness)
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

        public static void AddTriangle(this MeshGeometry3D mesh, Point3D point1, Point3D point2, Point3D point3)
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

        private static Vector3D ScaleVector(Vector3D vector, double length)
        {
            double scale = length / vector.Length;
            return new Vector3D(
                vector.X * scale,
                vector.Y * scale,
                vector.Z * scale);
        }

        public static void AddSphere(this MeshGeometry3D mesh, Point3D center, double radius, int num_phi, int num_theta)
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
                    mesh.AddTriangle(pt00, pt11, pt10);
                    mesh.AddTriangle(pt00, pt01, pt11);

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
    }
}
