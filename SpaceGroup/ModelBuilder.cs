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
        private static Vector3D ScaleVector(Vector3D vector, double length)
        {
            double scale = length / vector.Length;
            return new Vector3D(
                vector.X * scale,
                vector.Y * scale,
                vector.Z * scale);
        }

        public static void BuildDiscreteAxis(out GeometryModel3D discreteYAxisModel, out GeometryModel3D discreteXAxisModel, out GeometryModel3D discreteZAxisModel, CrystalCell crystalCell)
        {
            MeshGeometry3D xMesh = new MeshGeometry3D();
            MeshGeometry3D yMesh = new MeshGeometry3D();
            MeshGeometry3D zMesh = new MeshGeometry3D();
            Point3D origin = new Point3D(0, 0, 0);

            const double length = 5;

            double a = length;
            double b = length;
            double c = length;

            //Y (normal: X):

            Point3D xmax = new Point3D(5, 0, 0);
            yMesh.AddSegment(origin, xmax, new Vector3D(0, 1, 0), 0.1);

            Vector3D vX = xmax - origin;
            vX.Normalize();
            Vector3D perpX = Vector3D.CrossProduct(vX, new Vector3D(0, 1, 0));
            perpX.Normalize();
            Vector3D v1X = ScaleVector(-vX + perpX, 1);
            Vector3D v2X = ScaleVector(-vX - perpX, 1);
            yMesh.AddSegment(xmax, xmax + v1X, new Vector3D(0, 1, 0), 0.1);
            yMesh.AddSegment(xmax, xmax + v2X, new Vector3D(0, 1, 0), 0.1);

            Vector3D perpX1 = Vector3D.CrossProduct(vX, new Vector3D(0, 0, 1));
            perpX1.Normalize();
            Vector3D v1X1 = ScaleVector(-vX + perpX1, 1);
            Vector3D v2X1 = ScaleVector(-vX - perpX1, 1);
            yMesh.AddSegment(xmax, xmax + v1X1, new Vector3D(0, 0, 1), 0.1);
            yMesh.AddSegment(xmax, xmax + v2X1, new Vector3D(0, 0, 1), 0.1);

            SolidColorBrush axesBrush = Brushes.Green;
            DiffuseMaterial yAxisMaterial = new DiffuseMaterial(axesBrush);

            discreteYAxisModel = new GeometryModel3D(yMesh, yAxisMaterial);

            //Z (normal: Y):

            double x_c = c * Math.Cos(ToRadians(crystalCell.Alpha));

            double y_c = (b * c * Math.Cos(ToRadians(crystalCell.Beta)) - x_c * b * Math.Cos(ToRadians(crystalCell.Gamma)))
                /b * Math.Sin(ToRadians(crystalCell.Gamma));

            
            //double z_c = c * c - x_c * x_c - y_c * y_c;
            
            Point3D yMax = new Point3D(x_c, 5, y_c);
            zMesh.AddSegment(origin, yMax, new Vector3D(1, 0, 0), 0.1);

            Vector3D vY = yMax - origin;
            vY.Normalize();
            Vector3D perpY = Vector3D.CrossProduct(vY, new Vector3D(1, 0, 0));
            perpX.Normalize();
            Vector3D v1Y = ScaleVector(-vY + perpY, 1);
            Vector3D v2Y = ScaleVector(-vY - perpY, 1);
            zMesh.AddSegment(yMax, yMax + v1Y, new Vector3D(0, 1, 0), 0.1);
            zMesh.AddSegment(yMax, yMax + v2Y, new Vector3D(0, 1, 0), 0.1);

            Vector3D perpY1 = Vector3D.CrossProduct(vY, new Vector3D(0, 0, 1));
            perpX1.Normalize();
            Vector3D v1Y1 = ScaleVector(-vY + perpY1, 1);
            Vector3D v2Y1 = ScaleVector(-vY - perpY1, 1);
            zMesh.AddSegment(yMax, yMax + v1Y1, new Vector3D(0, 0, 1), 0.1);
            zMesh.AddSegment(yMax, yMax + v2Y1, new Vector3D(0, 0, 1), 0.1);

            axesBrush = Brushes.Blue;
            DiffuseMaterial zAxisMaterial = new DiffuseMaterial(axesBrush);

            discreteZAxisModel = new GeometryModel3D(zMesh, zAxisMaterial);


            //X (normal: Z):
            Point3D zMax = new Point3D(b * Math.Cos(ToRadians(crystalCell.Gamma)), 0, b * Math.Sin(crystalCell.Gamma));
            xMesh.AddSegment(origin, zMax, new Vector3D(1, 1, 0), 0.1);

            Vector3D vZ = zMax - origin;
            vZ.Normalize();
            Vector3D perpZ = Vector3D.CrossProduct(vZ, new Vector3D(0, 1, 0));
            perpX.Normalize();
            Vector3D v1Z = ScaleVector(-vZ + perpZ, 1);
            Vector3D v2Z = ScaleVector(-vZ - perpZ, 1);
            xMesh.AddSegment(zMax, zMax + v1Z, new Vector3D(0, 1, 0), 0.1);
            xMesh.AddSegment(zMax, zMax + v2Z, new Vector3D(0, 1, 0), 0.1);

            Vector3D perpZ1 = Vector3D.CrossProduct(vZ, new Vector3D(1, 0, 0));
            perpX1.Normalize();
            Vector3D v1Z1 = ScaleVector(-vZ + perpZ1, 1);
            Vector3D v2Z1 = ScaleVector(-vZ - perpZ1, 1);
            xMesh.AddSegment(zMax, zMax + v1Z1, new Vector3D(1, 0, 0), 0.1);
            xMesh.AddSegment(zMax, zMax + v2Z1, new Vector3D(1, 0, 0), 0.1);

            axesBrush = Brushes.Red;
            DiffuseMaterial xAxisMaterial = new DiffuseMaterial(axesBrush);

            discreteXAxisModel = new GeometryModel3D(xMesh, xAxisMaterial);
        }

        public static double ToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }
    }
}
