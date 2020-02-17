using SpaceGroup;
using System;
using System.Collections.Generic;
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

namespace SpaceGroup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CrystalCell atomCell;
        SpaceGroupCl selectedSpaceGroup;
        TranslateTransform3D axesTranslate;
        public MainWindow()
        {
            InitializeComponent();
        }
        // The main object model group.
        private Model3DGroup MainModel3Dgroup = new Model3DGroup();
        private GeometryModel3D AxesModel;
        private List<Model3DGroup> atomReproductions = new List<Model3DGroup>();

        private SolidColorBrush atomColor;
        // The camera.
        private PerspectiveCamera TheCamera;

        // The camera's current location.
        private double CameraPhi = Math.PI / 6.0;       // 30 degrees
        private double CameraTheta = Math.PI / 6.0;     // 30 degrees
        private double CameraR = 3.0;

        // The change in CameraPhi when you press the up and down arrows.
        private const double CameraDPhi = 0.1;

        // The change in CameraTheta when you press the left and right arrows.
        private const double CameraDTheta = 0.1;

        // The change in CameraR when you press + or -.
        private const double CameraDR = 0.1;

        public void selectGroup(SpaceGroupCl spaceGroup)
        {
            selectedSpaceGroup = spaceGroup;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            atomCell = new CrystalCell();
            selectedSpaceGroup = new SpaceGroupCl();
            atomCell.setCellParams(20.06999, 19.92, 13.42, 90, 90, 90); //hard code - заменить
            DataGridAtoms.ItemsSource = atomCell.atomCollection;

            // Give the camera its initial position.
            TheCamera = new PerspectiveCamera();
            TheCamera.FieldOfView = 60;
            MainViewport.Camera = TheCamera;
            PositionCamera();

            // Define lights.
            DefineLights();

            buildAxes(out AxesModel);
            // Create the model.
            //DefineModel(MainModel3Dgroup);

            // Add the group of models to a ModelVisual3D.
            ModelVisual3D model_visual = new ModelVisual3D();
            model_visual.Content = MainModel3Dgroup;
            MainModel3Dgroup.Children.Add(AxesModel);
            //axesTranslate = new TranslateTransform3D(TheCamera.Position.X -0.2, TheCamera.Position.Y - 0.1, TheCamera.Position.Z - 0.5);
            //AxesModel.Transform = axesTranslate;
            // Display the main visual to the viewport.
            MainViewport.Children.Add(model_visual);
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

 

        private void AddSegment(MeshGeometry3D mesh, Point3D point1, Point3D point2, Vector3D up)
        {
            const double thickness = 0.01;

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

        private void DefineModel(Model3DGroup model_group)
        {
#if ONE_BIG_SPHERE
            MeshGeometry3D mesh1 = new MeshGeometry3D();
            AddSphere(mesh1, new Point3D(0, 0, 0), 1, 5, 10);
            SolidColorBrush brush1 = Brushes.Red;
            DiffuseMaterial material1 = new DiffuseMaterial(brush1);
            GeometryModel3D model1 = new GeometryModel3D(mesh1, material1);
            model_group.Children.Add(model1);
#else
            // Make spheres centered at (+/-1, 0, 0).
            MeshGeometry3D mesh1 = new MeshGeometry3D();
            AddSphere(mesh1, new Point3D(1, 0, 0), 0.25, 5, 10);
            AddSphere(mesh1, new Point3D(-1, 0, 0), 0.25, 5, 10);
            SolidColorBrush brush1 = Brushes.Red;
            DiffuseMaterial material1 = new DiffuseMaterial(brush1);
            GeometryModel3D model1 = new GeometryModel3D(mesh1, material1);
            model_group.Children.Add(model1);

            // Make spheres centered at (0, +/-1, 0).
            MeshGeometry3D mesh2 = new MeshGeometry3D();
            AddSphere(mesh2, new Point3D(0, 1, 0), 0.25, 5, 10);
            AddSphere(mesh2, new Point3D(0, -1, 0), 0.25, 5, 10);
            SolidColorBrush brush2 = Brushes.Green;
            DiffuseMaterial material2 = new DiffuseMaterial(brush2);
            GeometryModel3D model2 = new GeometryModel3D(mesh2, material2);
            model_group.Children.Add(model2);

            // Make spheres centered at (0, 0, +/-1).
            MeshGeometry3D mesh3 = new MeshGeometry3D();
            AddSphere(mesh3, new Point3D(0, 0, 1), 0.25, 5, 10);
            AddSphere(mesh3, new Point3D(0, 0, -1), 0.25, 5, 10);
            SolidColorBrush brush3 = Brushes.Blue;
            DiffuseMaterial material3 = new DiffuseMaterial(brush3);
            GeometryModel3D model3 = new GeometryModel3D(mesh3, material3);
            model_group.Children.Add(model3);

#endif

            Console.WriteLine(
                mesh1.Positions.Count +
                mesh2.Positions.Count +
                mesh3.Positions.Count +
                " points");
            Console.WriteLine(
                (mesh1.TriangleIndices.Count +
                 mesh2.TriangleIndices.Count +
                 mesh3.TriangleIndices.Count +
                 " triangles"));
            Console.WriteLine();
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
            // Calculate the camera's position in Cartesian coordinates.
            //double y = CameraR * Math.Sin(CameraPhi);
            //double hyp = CameraR * Math.Cos(CameraPhi);
            //double x = hyp * Math.Cos(CameraTheta);
            //double z = hyp * Math.Sin(CameraTheta);
            TheCamera.Position = new Point3D(0, 0, +10);

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
                Atom atom = new Atom(s, xCoord.Text, yCoord.Text, zCoord.Text);
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
            Model3DGroup atomRepro = new Model3DGroup();
            for (int i = 0; i < selectedSpaceGroup.Expressions.Length; i += 3)
            {
                Console.WriteLine(i);
                MeshGeometry3D mesh1 = new MeshGeometry3D();
                AddSphere(mesh1, new Point3D
                   (
                     atomCell.YAxisL * SpaceGroupCl.Evaluate(selectedSpaceGroup.Expressions[i + 1], 0, atom.Y, 0),
                     atomCell.ZAxisL * SpaceGroupCl.Evaluate(selectedSpaceGroup.Expressions[i + 2], 0, 0, atom.Z),
                     atomCell.XAxisL * SpaceGroupCl.Evaluate(selectedSpaceGroup.Expressions[i], atom.X, 0, 0) 
                   ),
                     3, 20, 30);
                //AddSphere(mesh1, new Point3D(-1, 0, 0), 0.25, 5, 10);
                SolidColorBrush brush1 = atomColor;
                 DiffuseMaterial material1 = new DiffuseMaterial(brush1);
                 GeometryModel3D model1 = new GeometryModel3D(mesh1, material1);
                atomRepro.Children.Add(model1);
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

                changeHeading(angleX);
                ChangePitch(-angleY);

                //Rotate(Math3D.UnitY, 2 * angleX);
                //Rotate(RightDirection, 2 * angleY);


                //changeHeading(angleX);
                //ChangePitch(angleY);

                //CameraPhi += mouseDeltaY * 0.01;
                //if (CameraPhi > Math.PI / 2.0) CameraPhi = Math.PI / 2.0;

                //CameraTheta += mouseDeltaX * 0.01;

                //TheCamera.LookDirection = new Vector3D(TheCamera.LookDirection.X + mouseDeltaX * 0.01, TheCamera.LookDirection.Y + mouseDeltaY * 0.01, 0);


                //PositionCamera();
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

            moveAxesWithCamera();
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

        public void Rotate(Vector3D axis, double angle)
        {
            Quaternion q = Math3D.Rotation(axis, angle);
            TheCamera.Position = q.Transform(TheCamera.Position);
            TheCamera.UpDirection = q.Transform(TheCamera.UpDirection);
            TheCamera.LookDirection = q.Transform(TheCamera.LookDirection);
        }

        public void MoveForward(double d)
        {
            double u = 0.05;
            PerspectiveCamera camera = (PerspectiveCamera)MainViewport.Camera;
            Vector3D lookDirection = camera.LookDirection;
            Point3D position = camera.Position;

            lookDirection.Normalize();
            position = position + u * lookDirection * d;

            camera.Position = position;
        }

        public void MoveUp(double d)
        {
            double u = 0.05;
            PerspectiveCamera camera = (PerspectiveCamera)MainViewport.Camera;
            Vector3D upDirection = camera.UpDirection;
            Point3D position = camera.Position;

            upDirection.Normalize();
            position = position + u * upDirection * d;

            camera.Position = position;
        }

        //private void Window_KeyDown(object sender, KeyEventArgs e)
        //{
        //    switch (e.Key)
        //    {
        //        case Key.Up:
        //            CameraPhi += CameraDPhi;
        //            if (CameraPhi > Math.PI / 2.0) CameraPhi = Math.PI / 2.0;
        //            break;
        //        case Key.Down:
        //            CameraPhi -= CameraDPhi;
        //            if (CameraPhi < -Math.PI / 2.0) CameraPhi = -Math.PI / 2.0;
        //            break;
        //        case Key.Left:
        //            CameraTheta += CameraDTheta;
        //            break;
        //        case Key.Right:
        //            CameraTheta -= CameraDTheta;
        //            break;
        //        case Key.Add:
        //        case Key.OemPlus:
        //            CameraR -= CameraDR;
        //            if (CameraR < CameraDR) CameraR = CameraDR;
        //            break;
        //        case Key.Subtract:
        //        case Key.OemMinus:
        //            CameraR += CameraDR;
        //            break;
        //    }

        //    // Update the camera's position.
        //    PositionCamera();
        //}

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

        public void ChangeRoll(double angle)
        {
            TheCamera.UpDirection = TheCamera.UpDirection.Rotate(TheCamera.LookDirection, angle);
        }

        public void ChangePitch(double angle)
        {
            Quaternion q = Math3D.Rotation(LeftDirection, angle);
            TheCamera.UpDirection = q.Transform(TheCamera.UpDirection);
            TheCamera.LookDirection = q.Transform(TheCamera.LookDirection);
        }

        public void changeHeading(double angle)
        {
            Quaternion q = Math3D.RotationY(angle);
            TheCamera.UpDirection = q.Transform(TheCamera.UpDirection);
            TheCamera.LookDirection = q.Transform(TheCamera.LookDirection);
        }

        private void moveAxesWithCamera()
        {
            axesTranslate = new TranslateTransform3D(TheCamera.Position.X - 0.2, TheCamera.Position.Y - 0.1, TheCamera.Position.Z - 0.5);
            AxesModel.Transform = axesTranslate;
            //TranslateTransform3D axesTranslate2 = new TranslateTransform3D(TheCamera.LookDirection.X, TheCamera.LookDirection.Y, TheCamera.LookDirection.Z);
            //AxesModel.Transform = axesTranslate2;
        }

        private void pickColor(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                atomColor = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
            }
        }
        //private void 
    }
}
