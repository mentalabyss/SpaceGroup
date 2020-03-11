using SpaceGroup;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace SpaceGroup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Atom> multipliedAtoms;
        CrystalCell atomCell;
        SpaceGroupCl selectedSpaceGroup;
        TranslateTransform3D axesTranslate;
        public MainWindow()
        {
            InitializeComponent();
        }
        // The main object model group.
        private Model3DGroup cells_and_atoms;
        private Model3DGroup MainModel3Dgroup = new Model3DGroup();
        private GeometryModel3D AxesModel;
        private List<Model3DGroup> atomReproductions = new List<Model3DGroup>();
        private GeometryModel3D BordersModel;
        private GeometryModel3D polyhedra_model;
        private GeometryModel3D x_Axis;
        private GeometryModel3D y_Axis;
        private GeometryModel3D z_Axis;


        private SolidColorBrush atomColor;
        // The camera.
        private OrthographicCamera TheCamera;

        public void selectGroup(SpaceGroupCl spaceGroup)
        {
            selectedSpaceGroup = spaceGroup;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            multipliedAtoms = new List<Atom>();
            atomCell = new CrystalCell();
            selectedSpaceGroup = new SpaceGroupCl();
            atomCell.setCellParams(20.06999, 19.92, 13.42, 90, 90, 90); //hard code - заменить
            DataGridAtoms.ItemsSource = atomCell.atomCollection;

            // Give the camera its initial position.
            TheCamera = new OrthographicCamera();
            //TheCamera = new PerspectiveCamera();
            TheCamera.Width = 60;
            //TheCamera.FieldOfView = 60;
            MainViewport.Camera = TheCamera;
            PositionCamera();

            // Define lights.
            DefineLights();

            //buildAxes(out AxesModel);
            buildStaticAxes(out x_Axis, out y_Axis, out z_Axis);
            buildCellBorders(out BordersModel);
            // Create the model.
            //DefineModel(MainModel3Dgroup);

            // Add the group of models to a ModelVisual3D.
            ModelVisual3D model_visual = new ModelVisual3D();
            model_visual.Content = MainModel3Dgroup;
            MainModel3Dgroup.Children.Add(x_Axis);
            MainModel3Dgroup.Children.Add(y_Axis);
            MainModel3Dgroup.Children.Add(z_Axis);
            MainModel3Dgroup.Children.Add(BordersModel);
            //axesTranslate = new TranslateTransform3D(TheCamera.Position.X -0.2, TheCamera.Position.Y - 0.1, TheCamera.Position.Z - 0.5);
            //AxesModel.Transform = axesTranslate;
            // Display the main visual to the viewport.
            MainViewport.Children.Add(model_visual);
            moveFromCenter();
        }

        private void Translate_Cell()
        {

        }

        private void moveFromCenter()
        {
            var transform = new TranslateTransform3D(-atomCell.YAxisL / 2, -atomCell.ZAxisL / 2, -atomCell.XAxisL / 2);
            MainModel3Dgroup.Transform = transform;
        }

        private void buildCellBorders(out GeometryModel3D borders_model)
        {
            MeshGeometry3D borders_mesh = new MeshGeometry3D();
            Point3D origin = new Point3D(0, 0, 0);
            Point3D xLine = new Point3D(atomCell.YAxisL, 0, 0);
            Point3D yLine = new Point3D(0, atomCell.ZAxisL, 0);
            Point3D zLine = new Point3D(0, 0, atomCell.XAxisL);

            AddSegment(borders_mesh, origin, xLine, new Vector3D(0, 1, 0));
            AddSegment(borders_mesh, origin, yLine, new Vector3D(1, 0, 0));
            AddSegment(borders_mesh, origin, zLine, new Vector3D(0, 1, 0));

            AddSegment(borders_mesh, new Point3D(atomCell.YAxisL, 0, 0), new Point3D(atomCell.YAxisL, 0, atomCell.XAxisL), new Vector3D(0, 1, 0));
            AddSegment(borders_mesh, new Point3D(atomCell.YAxisL, atomCell.ZAxisL, 0), new Point3D(atomCell.YAxisL, atomCell.ZAxisL, atomCell.XAxisL), new Vector3D(0, 1, 0));
            AddSegment(borders_mesh, new Point3D(0, atomCell.ZAxisL, 0), new Point3D(0, atomCell.ZAxisL, atomCell.XAxisL), new Vector3D(0, 1, 0));


            AddSegment(borders_mesh, new Point3D(0, 0, atomCell.XAxisL), new Point3D(atomCell.YAxisL, 0, atomCell.XAxisL), new Vector3D(0, 1, 0));
            AddSegment(borders_mesh, new Point3D(0, atomCell.ZAxisL, atomCell.XAxisL), new Point3D(atomCell.YAxisL, atomCell.ZAxisL, atomCell.XAxisL), new Vector3D(0, 1, 0));
            AddSegment(borders_mesh, new Point3D(0, atomCell.ZAxisL, 0), new Point3D(atomCell.YAxisL, atomCell.ZAxisL, 0), new Vector3D(0, 1, 0));


            AddSegment(borders_mesh, new Point3D(atomCell.YAxisL, 0, 0), new Point3D(atomCell.YAxisL, atomCell.ZAxisL, 0), new Vector3D(1, 0, 0));
            AddSegment(borders_mesh, new Point3D(0, 0, atomCell.XAxisL), new Point3D(0, atomCell.ZAxisL, atomCell.XAxisL), new Vector3D(1, 0, 0));
            AddSegment(borders_mesh, new Point3D(atomCell.YAxisL, 0, atomCell.XAxisL), new Point3D(atomCell.YAxisL, atomCell.ZAxisL, atomCell.XAxisL), new Vector3D(1, 0, 0));




            SolidColorBrush borders_brush = Brushes.Black;
            DiffuseMaterial borders_material = new DiffuseMaterial(borders_brush);
            borders_model = new GeometryModel3D(borders_mesh, borders_material);
        }

        private void buildAxes(out GeometryModel3D axes_model)
        {
            // Make the axes model.
            MeshGeometry3D axes_mesh = new MeshGeometry3D();
            Point3D origin = new Point3D(0, 0, 0);
            Point3D xmax = new Point3D(0.1, 0, 0);
            Point3D ymax = new Point3D(0, 0.1, 0);
            Point3D zmax = new Point3D(0, 0, 0.1);
            AddSegment(axes_mesh, origin, xmax, new Vector3D(0, 1, 0));
            AddSegment(axes_mesh, origin, zmax, new Vector3D(0, 1, 0));
            AddSegment(axes_mesh, origin, ymax, new Vector3D(1, 0, 0));

            SolidColorBrush axes_brush = Brushes.Red;
            DiffuseMaterial axes_material = new DiffuseMaterial(axes_brush);
            axes_model = new GeometryModel3D(axes_mesh, axes_material);
        }

        private void buildStaticAxes(out GeometryModel3D static_Y_axis_model, out GeometryModel3D static_X_axis_model, out GeometryModel3D static_Z_axis_model)
        {
            MeshGeometry3D x_mesh = new MeshGeometry3D();
            MeshGeometry3D y_mesh = new MeshGeometry3D();
            MeshGeometry3D z_mesh = new MeshGeometry3D();
            Point3D origin = new Point3D(0, 0, 0);


            //Y (normal: X):
            Point3D xmax = new Point3D(25, 0, 0);
            AddSegment(y_mesh, origin, xmax, new Vector3D(0, 1, 0));

            Vector3D vX = xmax - origin;
            vX.Normalize();
            Vector3D perpX = Vector3D.CrossProduct(vX, new Vector3D(0, 1, 0));
            perpX.Normalize();
            Vector3D v1X = ScaleVector(-vX + perpX, 1);
            Vector3D v2X = ScaleVector(-vX - perpX, 1);
            AddSegment(y_mesh, xmax, xmax + v1X, new Vector3D(0, 1, 0));
            AddSegment(y_mesh, xmax, xmax + v2X, new Vector3D(0, 1, 0));

            Vector3D perpX1 = Vector3D.CrossProduct(vX, new Vector3D(0, 0, 1));
            perpX1.Normalize();
            Vector3D v1X1 = ScaleVector(-vX + perpX1, 1);
            Vector3D v2X1 = ScaleVector(-vX - perpX1, 1);
            AddSegment(y_mesh, xmax, xmax + v1X1, new Vector3D(0, 0, 1));
            AddSegment(y_mesh, xmax, xmax + v2X1, new Vector3D(0, 0, 1));

            SolidColorBrush axes_brush = Brushes.Blue;
            DiffuseMaterial y_axis_material = new DiffuseMaterial(axes_brush);

            static_Y_axis_model = new GeometryModel3D(y_mesh, y_axis_material);

            //Z (normal: Y):
            Point3D ymax = new Point3D(0, 25, 0);
            AddSegment(z_mesh, origin, ymax, new Vector3D(1, 0, 0));

            Vector3D vY = ymax - origin;
            vY.Normalize();
            Vector3D perpY = Vector3D.CrossProduct(vY, new Vector3D(1, 0, 0));
            perpX.Normalize();
            Vector3D v1Y = ScaleVector(-vY + perpY, 1);
            Vector3D v2Y = ScaleVector(-vY - perpY, 1);
            AddSegment(z_mesh, ymax, ymax + v1Y, new Vector3D(0, 1, 0));
            AddSegment(z_mesh, ymax, ymax + v2Y, new Vector3D(0, 1, 0));

            Vector3D perpY1 = Vector3D.CrossProduct(vY, new Vector3D(0, 0, 1));
            perpX1.Normalize();
            Vector3D v1Y1 = ScaleVector(-vY + perpY1, 1);
            Vector3D v2Y1 = ScaleVector(-vY - perpY1, 1);
            AddSegment(z_mesh, ymax, ymax + v1Y1, new Vector3D(0, 0, 1));
            AddSegment(z_mesh, ymax, ymax + v2Y1, new Vector3D(0, 0, 1));

            axes_brush = Brushes.Green;
            DiffuseMaterial z_axis_material = new DiffuseMaterial(axes_brush);

            static_Z_axis_model = new GeometryModel3D(z_mesh, z_axis_material);


            //X (normal: Z):
            Point3D zmax = new Point3D(0, 0, 25);
            AddSegment(x_mesh, origin, zmax, new Vector3D(0, 1, 0));

            Vector3D vZ = zmax - origin;
            vZ.Normalize();
            Vector3D perpZ = Vector3D.CrossProduct(vZ, new Vector3D(0, 1, 0));
            perpX.Normalize();
            Vector3D v1Z = ScaleVector(-vZ + perpZ, 1);
            Vector3D v2Z = ScaleVector(-vZ - perpZ, 1);
            AddSegment(x_mesh, zmax, zmax + v1Z, new Vector3D(0, 1, 0));
            AddSegment(x_mesh, zmax, zmax + v2Z, new Vector3D(0, 1, 0));

            Vector3D perpZ1 = Vector3D.CrossProduct(vZ, new Vector3D(1, 0, 0));
            perpX1.Normalize();
            Vector3D v1Z1 = ScaleVector(-vZ + perpZ1, 1);
            Vector3D v2Z1 = ScaleVector(-vZ - perpZ1, 1);
            AddSegment(x_mesh, zmax, zmax + v1Z1, new Vector3D(1, 0, 0));
            AddSegment(x_mesh, zmax, zmax + v2Z1, new Vector3D(1, 0, 0));

            axes_brush = Brushes.Red;
            DiffuseMaterial x_axis_material = new DiffuseMaterial(axes_brush);

            static_X_axis_model = new GeometryModel3D(x_mesh, x_axis_material);

            //static_axes_model = new GeometryModel3D(axes_mesh, axes_material);
        }

        private void AddSegment(MeshGeometry3D mesh, Point3D point1, Point3D point2, Vector3D up)
        {
            const double thickness = 0.04;

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

        private Vector3D ScaleVector(Vector3D vector, double length)
        {
            double scale = length / vector.Length;
            return new Vector3D(
                vector.X * scale,
                vector.Y * scale,
                vector.Z * scale);
        }

        private void DefineLights()
        {
            AmbientLight ambient_light = new AmbientLight(Colors.Gray);
            DirectionalLight directional_light =
                new DirectionalLight(Colors.Gray, new Vector3D(-1.0, -3.0, -2.0));
            MainModel3Dgroup.Children.Add(ambient_light);
            MainModel3Dgroup.Children.Add(directional_light);
        }

        private void PositionCamera()
        {
            TheCamera.Position = new Point3D(0, atomCell.ZAxisL / 2, +40);

            // Look toward the origin.
            TheCamera.LookDirection = new Vector3D(0, 0, -1);

            // Set the Up direction.
            TheCamera.UpDirection = new Vector3D(0, 1, 0);

            // Console.WriteLine("Camera.Position: (" + x + ", " + y + ", " + z + ")");
        }

        private void addAtomButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string s = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(atomName.Text.ToLower());
                Atom atom = new Atom(s, xCoord.Text, yCoord.Text, zCoord.Text, atomColor);
                visualizeAtom(MainModel3Dgroup, atom);
                atomCell.atomCollection.Add(atom);

                DataGridAtoms.Items.Refresh();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Не выбрана группа!");
            }
        }

        private void deleteButtonClick(object sender, RoutedEventArgs e)
        {
            int index = DataGridAtoms.SelectedIndex;
            Model3DGroup groupToRemove = atomReproductions[index];
            MainModel3Dgroup.Children.Remove(groupToRemove);
            atomCell.atomCollection.Remove(DataGridAtoms.SelectedItem as Atom);
            //MessageBox.Show((DataGridAtoms.SelectedItem as Atom).Element);
            DataGridAtoms.Items.Refresh();
            
        }

        private void visualizeAtom(Model3DGroup model_group, Atom atom)
        {

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
            for (int i = 0; i < selectedSpaceGroup.Expressions.Length; i += 3)
            {
                MeshGeometry3D mesh1 = new MeshGeometry3D();
                double x = SpaceGroupCl.Evaluate(selectedSpaceGroup.Expressions[i + 1], 0, atom.Y, 0) * atomCell.YAxisL; //Y
                double y = SpaceGroupCl.Evaluate(selectedSpaceGroup.Expressions[i + 2], 0, 0, atom.Z) * atomCell.ZAxisL; //Z
                double z = SpaceGroupCl.Evaluate(selectedSpaceGroup.Expressions[i], atom.X, 0, 0) * atomCell.XAxisL; //X

                if (x < 0)
                    x += atomCell.YAxisL;
                if (y < 0)
                    y += atomCell.ZAxisL;
                if (z < 0)
                    z += atomCell.XAxisL;

                AddSphere(mesh1, new Point3D(x, y, z), atomSize, 20, 30);
            //AddSphere(mesh1, new Point3D(-1, 0, 0), 0.25, 5, 10);
                DiffuseMaterial material1 = new DiffuseMaterial(brush1);
                GeometryModel3D model1 = new GeometryModel3D(mesh1, material1);
                atomRepro.Children.Add(model1);

                multipliedAtoms.Add(new Atom(atom.Element, z.ToString(), x.ToString(), y.ToString(), atomColor));
            }
            atomReproductions.Add(atomRepro);
            model_group.Children.Add(atomRepro);
            //model_group.Children.Add();

            //MeshGeometry3D mesh1 = new MeshGeometry3D();
            //AddSphere(mesh1, new Point3D(atomCell.YAxisL * atom.X, atomCell.ZAxisL * atom.Z, atomCell.XAxisL * atom.X), 0.25, 20, 30);
            ////AddSphere(mesh1, new Point3D(-1, 0, 0), 0.25, 5, 10);
            //SolidColorBrush brush1 = Brushes.Red;
            //DiffuseMaterial material1 = new DiffuseMaterial(brush1);
            //GeometryModel3D model1 = new GeometryModel3D(mesh1, material1);
            //model_group.Children.Add(model1);

        }

        private void DrawPolyhedra()
        {
            List<Atom> oxygens = Polyhedra.CalculatePolyhedra(multipliedAtoms);
            MeshGeometry3D polyhedra_mesh = new MeshGeometry3D();
            for (int i = 0; i < oxygens.Count; i += 4)
            {
                Point3D point0 = new Point3D(oxygens[i].Y, oxygens[i].Z, oxygens[i].X);
                Point3D point1 = new Point3D(oxygens[i+1].Y, oxygens[i+1].Z, oxygens[i+1].X);
                Point3D point2 = new Point3D(oxygens[i+2].Y, oxygens[i+2].Z, oxygens[i+2].X);
                Point3D point3 = new Point3D(oxygens[i+3].Y, oxygens[i+3].Z, oxygens[i+3].X);

                AddSegment(polyhedra_mesh, point0, point1, new Vector3D(0, 1, 0));
                AddSegment(polyhedra_mesh, point0, point2, new Vector3D(0, 1, 0));
                AddSegment(polyhedra_mesh, point0, point3, new Vector3D(0, 1, 0));
                AddSegment(polyhedra_mesh, point1, point3, new Vector3D(0, 1, 0));
                AddSegment(polyhedra_mesh, point1, point2, new Vector3D(0, 1, 0));
                AddSegment(polyhedra_mesh, point2, point3, new Vector3D(0, 1, 0));

                SolidColorBrush borders_brush = Brushes.Black;
                DiffuseMaterial borders_material = new DiffuseMaterial(borders_brush);
                polyhedra_model = new GeometryModel3D(polyhedra_mesh, borders_material);
                MainModel3Dgroup.Children.Add(polyhedra_model);
            }
        }

        private void AddSphere(MeshGeometry3D mesh, Point3D center,
    double radius, int num_phi, int num_theta)
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

        private void AddTriangle(MeshGeometry3D mesh, Point3D point1, Point3D point2, Point3D point3)
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

        bool mouseDragged = false;

        public void OnViewportMouseDown(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(this);
            mouseX = position.X;
            mouseY = position.Y;
            if (!mouseDragged)
            {
                MouseOldX = position.X;
                MouseOldY = position.Y;
            }
        }

        double mouseX;
        double mouseY;
        double MouseOldX;
        double MouseOldY;
        double mouseDeltaX;
        double mouseDeltaY;

        public void OnViewportMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                mouseDragged = true;
                MouseOldX = mouseX;
                MouseOldY = mouseY;
                Point position = e.GetPosition(this);
                mouseX = position.X;
                mouseY = position.Y;
                mouseDeltaX = (mouseX - MouseOldX);
                mouseDeltaY = (mouseY - MouseOldY);

                double angleX = mouseDeltaX * 0.1;
                double angleY = mouseDeltaY * 0.1;

                RotateCamera(angleX, angleY);  
            }
            else
            {
                mouseDragged = false;
            }

            if(e.MiddleButton == MouseButtonState.Pressed)
            {
                MouseOldX = mouseX;
                MouseOldY = mouseY;
                Point position = e.GetPosition(this);
                mouseX = position.X;
                mouseY = position.Y;
                mouseDeltaX = (mouseX - MouseOldX);
                mouseDeltaY = (mouseY - MouseOldY);

                MoveRight(- mouseDeltaX * 0.5);
                MoveUp(mouseDeltaY * 0.5);
            }

            if(e.RightButton == MouseButtonState.Pressed)
            {
                MouseOldX = mouseX;
                MouseOldY = mouseY;
                Point position = e.GetPosition(this);
                mouseX = position.X;
                mouseY = position.Y;
                mouseDeltaX = (mouseX - MouseOldX);
                mouseDeltaY = (mouseY - MouseOldY);

                MoveForward(mouseDeltaY * 0.5);
            }

            //moveAxesWithCamera();
        }

        public void MoveRight(double d)
        {
            double u = 0.05;
            PerspectiveCamera camera = (PerspectiveCamera)MainViewport.Camera;
            Vector3D lookDirection = camera.LookDirection;
            Point3D position = camera.Position;
            lookDirection.Normalize();
            position = position + u * RightDirection * d;

            camera.Position = position;
        }



        public void MoveForward(double d)
        {
            double u = 0.05;
            OrthographicCamera camera = (OrthographicCamera)MainViewport.Camera;
            Vector3D lookDirection = camera.LookDirection;
            Point3D position = camera.Position;

            lookDirection.Normalize();
            position = position + u * lookDirection * d;

            camera.Position = position;
        }

        public void MoveUp(double d)
        {
            double u = 0.05;
            OrthographicCamera camera = (OrthographicCamera)MainViewport.Camera;
            Vector3D upDirection = camera.UpDirection;
            Point3D position = camera.Position;

            upDirection.Normalize();
            position = position + u * upDirection * d;

            camera.Position = position;
        }

        private void newGroup_Click(object sender, RoutedEventArgs e)
        {
            SpaceGroupSettings spaceGroupSettings = new SpaceGroupSettings();
            spaceGroupSettings.Owner = this;
            spaceGroupSettings.Show();
        }


        //CAMERA STUFF
        public Vector3D LeftDirection
        {
            get { return TheCamera.UpDirection.Cross(TheCamera.LookDirection); }
        }

        public Vector3D RightDirection
        {
            get { return TheCamera.LookDirection.Cross(TheCamera.UpDirection); }
        }

        private void pickColor(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                atomColor = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
            }
        }

        private void ScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //MessageBox.Show(atomResizer.);
            //foreach(Model3DGroup atomGroup in atomReproductions)
            //{
            //    atomGroup.Transform = new ScaleTransform3D(atomResizer.Value * 0.1, atomResizer.Value * 0.1, atomResizer.Value * 0.1);
            //    new ScaleTransform3D()
            //}
        }
        //private void 

        private void RotateCamera(double angleX, double angleY)
        {
            Rotate(new Vector3D(0, 1, 0), - angleX * 2);
            Rotate(RightDirection, 2 * angleY);
        }


        public void Rotate(Vector3D axis, double angle)
        {
            Quaternion q = Math3D.Rotation(axis, angle);
            TheCamera.Position = q.Transform(TheCamera.Position);
            TheCamera.UpDirection = q.Transform(TheCamera.UpDirection);
            TheCamera.LookDirection = q.Transform(TheCamera.LookDirection);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DrawPolyhedra();
        }

        private void saveTableButton_Click(object sender, RoutedEventArgs e)
        {
            SerializeTableList();
        }

        private void SerializeTableList()
        {
            List<Atom> atoms = new List<Atom>();
            foreach(Atom a in atomCell.atomCollection)
            {
                atoms.Add(a);
            }
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Atom>));
            TextWriter writer = new StreamWriter("table.xml");

            Console.WriteLine("B");
            foreach(Atom a in atoms)
            {
                Console.WriteLine(a.X);
            }
            Console.WriteLine("E");

            xmlSerializer.Serialize(writer, atoms);
            writer.Close();
        }

        private ObservableCollection<Atom> DeserializeTableList(string filename)
        {

            var mySerializer = new XmlSerializer(typeof(List<Atom>));
            var myFileStream = new FileStream(filename, FileMode.Open);
            List<Atom> myObject = (List<Atom>)mySerializer.Deserialize(myFileStream);
            ObservableCollection<Atom> ret = new ObservableCollection<Atom>();
            foreach(Atom a in myObject)
            {
                a.StringToColor();
                ret.Add(a);
            }
            return ret;
        }

        private void openBtnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.DefaultExt = ".xml";
            //openFileDialog.Filter = "XML files(*.xml)";

            if(openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filename = openFileDialog.FileName;

                atomCell.atomCollection = DeserializeTableList(filename);
                foreach(Atom a in atomCell.atomCollection)
                {
                    visualizeAtom(MainModel3Dgroup, a);
                }
                MessageBox.Show("loaded");
                //visualizeAtom(MainModel3Dgroup, atom);
                //atomCell.atomCollection.Add(atom);
                DataGridAtoms.ItemsSource = atomCell.atomCollection;


            }
        }
    }
}
