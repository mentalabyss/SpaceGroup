﻿using SpaceGroup;
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
        private Model3DGroup cells_and_atoms = new Model3DGroup();
        private Model3DGroup atoms = new Model3DGroup();
        private Model3DGroup MainModel3Dgroup = new Model3DGroup();
        private Model3DGroup Upper_Cell_Model;
        private Model3DGroup DiscreteAxisGroup = new Model3DGroup();
        private Model3DGroup MainAndDA = new Model3DGroup();
        private Model3DGroup TranslationsGroup = new Model3DGroup();


        private List<Model3DGroup> atomReproductions = new List<Model3DGroup>();
        private List<GeometryModel3D> SelectableModels = new List<GeometryModel3D>();
        private List<List<Atom>> atomsList = new List<List<Atom>>(); 

        private GeometryModel3D BordersModel;
        private GeometryModel3D polyhedra_model;
        private GeometryModel3D x_Axis;
        private GeometryModel3D y_Axis;
        private GeometryModel3D z_Axis;
        private GeometryModel3D discrete_x_axis;
        private GeometryModel3D discrete_y_axis;
        private GeometryModel3D discrete_z_axis;
        private GeometryModel3D SelectedModel = null;


        private Material SelectedMaterial = new DiffuseMaterial(Brushes.Black);
        private Material NormalMaterial = new DiffuseMaterial();


        Label xLabel = new Label();
        Label yLabel = new Label();
        Label zLabel = new Label();



        bool polySwitch = false;
        bool translationsSwitch = false;


        private SolidColorBrush atomColor;
        // The camera.
        private OrthographicCamera TheCamera;

        public void selectGroup(SpaceGroupCl spaceGroup)
        {
            selectedSpaceGroup = spaceGroup;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            xLabel.Content = "Y";
            yLabel.Content = "Z";
            zLabel.Content = "X";
            xLabel.FontSize = 25;
            yLabel.FontSize = 25;
            zLabel.FontSize = 25;
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

            DefineLights();

            buildStaticAxes(out x_Axis, out y_Axis, out z_Axis);
            buildDiscreteAxis(out discrete_y_axis, out discrete_x_axis, out discrete_z_axis);
            DiscreteAxisGroup.Children.Add(discrete_x_axis);
            DiscreteAxisGroup.Children.Add(discrete_y_axis);
            DiscreteAxisGroup.Children.Add(discrete_z_axis);
            buildCellBorders(out BordersModel);

            ModelVisual3D model_visual = new ModelVisual3D();
            model_visual.Content = MainAndDA;
            MainModel3Dgroup.Children.Add(x_Axis);
            MainModel3Dgroup.Children.Add(y_Axis);
            MainModel3Dgroup.Children.Add(z_Axis);

            MainModel3Dgroup.Children.Add(cells_and_atoms);

          
            MainAndDA.Children.Add(MainModel3Dgroup);

            cells_and_atoms.Children.Add(BordersModel);

            MainViewport.Children.Add(model_visual);
            moveFromCenter();
            generateLabels();

        }

        private void Translate_Cell(string direction)
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


        private void generateLabels()
        {
            //-atomCell.YAxisL / 2, -atomCell.ZAxisL / 2, -atomCell.XAxisL / 2
            canvasOn3D.Children.Clear();
            //X
            Point LabelXPoint = (Point)Point3DToScreen2D(new Point3D(25 - atomCell.YAxisL / 2, -atomCell.ZAxisL / 2, -atomCell.XAxisL / 2), MainViewport);
            Canvas.SetLeft(xLabel, LabelXPoint.X);
            Canvas.SetTop(xLabel, LabelXPoint.Y);
            canvasOn3D.Children.Add(xLabel);

            Point LabelYPoint = (Point)Point3DToScreen2D(new Point3D(-atomCell.YAxisL / 2, 25 - atomCell.ZAxisL / 2, -atomCell.XAxisL / 2), MainViewport);
            Canvas.SetLeft(yLabel, LabelYPoint.X);
            Canvas.SetTop(yLabel, LabelYPoint.Y);
            canvasOn3D.Children.Add(yLabel);

            Point LabelZPoint = (Point)Point3DToScreen2D(new Point3D(-atomCell.YAxisL / 2, - atomCell.ZAxisL / 2, 25 - atomCell.XAxisL / 2), MainViewport);
            Canvas.SetLeft(zLabel, LabelZPoint.X);
            Canvas.SetTop(zLabel, LabelZPoint.Y);
            canvasOn3D.Children.Add(zLabel);
        }

        private void buildDiscreteAxis(out GeometryModel3D discrete_y_axis_model, out GeometryModel3D discrete_x_axis_model, out GeometryModel3D discrete_z_axis_model)
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

        private void moveFromCenter()
        {
            //var transform = new TranslateTransform3D(-atomCell.XAxisL / 2, -atomCell.YAxisL / 2, -atomCell.ZAxisL / 2);
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

        //private void buildAxes(out GeometryModel3D axes_model)
        //{

        //}

        private void buildStaticAxes(out GeometryModel3D static_Y_axis_model, out GeometryModel3D static_X_axis_model, out GeometryModel3D static_Z_axis_model)
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

        private void AddSegment(MeshGeometry3D mesh, Point3D point1, Point3D point2, Vector3D up, double thickness)
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
            TheCamera.Position = new Point3D(0, atomCell.ZAxisL / 2, +50);

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
                Atom atom = new Atom(s, xCoord.Text.Replace(',', '.'), yCoord.Text.Replace(',', '.'), zCoord.Text.Replace(',', '.'), atomColor);
                visualizeAtom(cells_and_atoms, atom);
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
            cells_and_atoms.Children.Remove(groupToRemove);
            atomCell.atomCollection.Remove(DataGridAtoms.SelectedItem as Atom);
            //MessageBox.Show((DataGridAtoms.SelectedItem as Atom).Element);
            DataGridAtoms.Items.Refresh();
            
        }

        private void visualizeAtom(Model3DGroup model_group, Atom atom)
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

        private void DrawPolyhedra()
        {
            List<Atom> oxygens = Polyhedra.CalculatePolyhedra(multipliedAtoms, atomCell.YAxisL, atomCell.ZAxisL, atomCell.XAxisL);
            MeshGeometry3D polyhedra_mesh = new MeshGeometry3D();
            for (int i = 0; i < oxygens.Count; i += 4)
            {
                Point3D point0 = new Point3D(oxygens[i].Y * atomCell.YAxisL, oxygens[i].Z * atomCell.ZAxisL, oxygens[i].X * atomCell.XAxisL);
                Point3D point1 = new Point3D(oxygens[i+1].Y * atomCell.YAxisL, oxygens[i+1].Z * atomCell.ZAxisL, oxygens[i+1].X * atomCell.XAxisL);
                Point3D point2 = new Point3D(oxygens[i+2].Y * atomCell.YAxisL, oxygens[i+2].Z * atomCell.ZAxisL, oxygens[i+2].X * atomCell.XAxisL);
                Point3D point3 = new Point3D(oxygens[i+3].Y * atomCell.YAxisL, oxygens[i+3].Z * atomCell.ZAxisL, oxygens[i+3].X * atomCell.XAxisL);

                AddSegment(polyhedra_mesh, point0, point1, new Vector3D(0, 1, 0), 0.04);
                AddSegment(polyhedra_mesh, point0, point2, new Vector3D(0, 1, 0), 0.04);
                AddSegment(polyhedra_mesh, point0, point3, new Vector3D(0, 1, 0), 0.04);
                AddSegment(polyhedra_mesh, point1, point3, new Vector3D(0, 1, 0), 0.04);
                AddSegment(polyhedra_mesh, point1, point2, new Vector3D(0, 1, 0), 0.04);
                AddSegment(polyhedra_mesh, point2, point3, new Vector3D(0, 1, 0), 0.04);

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





        public Point? Point3DToScreen2D(Point3D point3D, Viewport3D viewPort)
        {
            bool bOK = false;

            // We need a Viewport3DVisual but we only have a Viewport3D.
            
            for(int i = 0; i < viewPort.Children.Count; i++)
            {
                Console.WriteLine(viewPort.Children[i].ToString());
            }

            Viewport3DVisual vpv = VisualTreeHelper.GetParent(viewPort.Children[0]) as Viewport3DVisual;

            // Get the world to viewport transform matrix
            Matrix3D m = MUtils.TryWorldToViewportTransform(vpv, out bOK);

            if (bOK)
            {
                // Transform the 3D point to 2D
                Point3D transformedPoint = m.Transform(point3D);

                Point screen2DPoint = new Point(transformedPoint.X, transformedPoint.Y);

                return new Nullable<Point>(screen2DPoint);
            }
            else
            {
                return null;
            }
        }

        bool mouseDragged = false;

        List<GeometryModel3D> SelectedModels = new List<GeometryModel3D>();
        List<Material> SavedMaterials = new List<Material>();

        public void OnViewportMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                if (!contextMenu1.IsOpen)
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
            }


            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                // Get the mouse's position relative to the viewport.
                Point mouse_pos = e.GetPosition(MainViewport);

                // Perform the hit test.
                HitTestResult result = VisualTreeHelper.HitTest(MainViewport, mouse_pos);

                // See if we hit a model.
                RayMeshGeometry3DHitTestResult mesh_result = result as RayMeshGeometry3DHitTestResult;



                // Deselect the prevously selected model.
                //if (SelectedModel != null)
                //{
                //    SelectedModel.Material = NormalMaterial;
                //    SelectedModel = null;
                //}

                if (mesh_result != null)
                {
                    GeometryModel3D model = (GeometryModel3D)mesh_result.ModelHit;

                    if (SelectableModels.Contains(model) && !SelectedModels.Contains(model))
                    {
                        SelectedModel = model;
                        NormalMaterial = SelectedModel.Material;
                        SelectedModel.Material = SelectedMaterial;

                        SelectedModels.Add(SelectedModel);
                        SavedMaterials.Add(NormalMaterial);

                        ShowSelectedAtomInfo(SelectedModel, NormalMaterial);
                    }
                }
                else
                {
                    for (int i = 0; i < SelectedModels.Count; i++)
                    {
                        SelectedModels[i].Material = SavedMaterials[i];
                    }

                    SelectedModels.Clear();
                    SavedMaterials.Clear();
                    SelectedModel = null;
                }
            }
        }

        public void DistanceButtonClicked(object sender, RoutedEventArgs e)
        {
            Atom atom1 = new Atom();
            Atom atom2 = new Atom();

            if(SelectedModels.Count == 2)
            { 
                for (int i = 1; i < cells_and_atoms.Children.Count; i++)
                {
                    var iGroup = (Model3DGroup)cells_and_atoms.Children[i];

                    if (iGroup.Children.IndexOf(SelectedModels[0]) > -1)
                    {
                        atom1 = atomsList[i - 1][iGroup.Children.IndexOf(SelectedModels[0])];
                        break;
                    }
                }

                for (int i = 1; i < cells_and_atoms.Children.Count; i++)
                {
                    var iGroup = (Model3DGroup)cells_and_atoms.Children[i];

                    if (iGroup.Children.IndexOf(SelectedModels[1]) > -1)
                    {
                        atom2 = atomsList[i - 1][iGroup.Children.IndexOf(SelectedModels[1])];
                        break;
                    }
                }

                System.Windows.Forms.MessageBox.Show(DistanceTwoAtoms(atom1, atom2).ToString());
            }


        }


        public void AngleButtonClicked(object sender, RoutedEventArgs e)
        {
            Atom atom1 = new Atom();
            Atom atom2 = new Atom();
            Atom atom3 = new Atom();

            if (SelectedModels.Count == 3)
            {
                for (int i = 1; i < cells_and_atoms.Children.Count; i++)
                {
                    var iGroup = (Model3DGroup)cells_and_atoms.Children[i];

                    if (iGroup.Children.IndexOf(SelectedModels[0]) > -1)
                    {
                        atom1 = atomsList[i - 1][iGroup.Children.IndexOf(SelectedModels[0])];
                        break;
                    }
                }

                for (int i = 1; i < cells_and_atoms.Children.Count; i++)
                {
                    var iGroup = (Model3DGroup)cells_and_atoms.Children[i];

                    if (iGroup.Children.IndexOf(SelectedModels[1]) > -1)
                    {
                        atom2 = atomsList[i - 1][iGroup.Children.IndexOf(SelectedModels[1])];
                        break;
                    }
                }

                for (int i = 1; i < cells_and_atoms.Children.Count; i++)
                {
                    var iGroup = (Model3DGroup)cells_and_atoms.Children[i];

                    if (iGroup.Children.IndexOf(SelectedModels[2]) > -1)
                    {
                        atom3 = atomsList[i - 1][iGroup.Children.IndexOf(SelectedModels[2])];
                        break;
                    }
                }

                System.Windows.Forms.MessageBox.Show(ThreeAtomsAngle(atom1, atom2, atom3).ToString() + "°");
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
                //RotateDiscreteAxis();
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
            generateLabels();

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

        public void RotateDiscreteAxis()
        {
            double cameraX = TheCamera.Position.X;
            double cameraY = TheCamera.Position.Y;
            double cameraZ = TheCamera.Position.Z;
            DiscreteAxisGroup.Transform = new TranslateTransform3D(-cameraX, -cameraY, -cameraZ);
        }

        private void draw_translations(object sender, RoutedEventArgs e)
        {
            if (!translationsSwitch)
            {
                translationsSwitch = true;
                Translate_Cell("up");
                Translate_Cell("down");
                Translate_Cell("left");
                Translate_Cell("right");
                Translate_Cell("front");
                Translate_Cell("back");
                MainModel3Dgroup.Children.Add(TranslationsGroup);
            }
            else
            {
                translationsSwitch = false;
                MainModel3Dgroup.Children.Remove(TranslationsGroup);
                TranslationsGroup.Children.Clear();
            }
        }

        private void saveTableButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog1.Filter = "SPG Image|*.spg";
            saveFileDialog1.Title = "Save Atoms State";
            saveFileDialog1.ShowDialog();

            if(saveFileDialog1.FileName != "")
                SerializeTableList(saveFileDialog1.FileName);
        }

        private void SerializeTableList(string fileName)
        {
            List<Atom> atoms = new List<Atom>();
            foreach(Atom a in atomCell.atomCollection)
            {
                atoms.Add(a);
            }
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Atom>));
            TextWriter writer = new StreamWriter(fileName);

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

                cells_and_atoms.Children.Clear();

                buildCellBorders(out BordersModel);
                cells_and_atoms.Children.Add(BordersModel);

                try
                {
                    foreach (Atom a in atomCell.atomCollection)
                    {
                        visualizeAtom(cells_and_atoms, a);
                    }

                    DataGridAtoms.ItemsSource = atomCell.atomCollection;
                }
                catch (System.NullReferenceException)
                {
                    MessageBox.Show("Не выбрана группа!"); //ВНИМАНИЕ
                }
                //visualizeAtom(MainModel3Dgroup, atom);
                //atomCell.atomCollection.Add(atom);
            }
        }

        private void draw_polyhydras(object sender, RoutedEventArgs e)
        {
            if (!polySwitch)
            {
                polySwitch = true;
                DrawPolyhedra();
            }
            else
            {
                polySwitch = false;
                MainModel3Dgroup.Children.Remove(polyhedra_model);
            }
        }

        private void ShowSelectedAtomInfo(GeometryModel3D selectedModel, Material selectedMaterial)
        {
            for(int i = 1; i < cells_and_atoms.Children.Count; i++)
            {
                var iGroup = (Model3DGroup)cells_and_atoms.Children[i];
                if(iGroup.Children.IndexOf(selectedModel) > -1)
                {
                    selectedAtomName.Content = atomsList[i - 1][iGroup.Children.IndexOf(selectedModel)].Element;
                    selectedAtomX.Content = atomsList[i - 1][iGroup.Children.IndexOf(selectedModel)].X;
                    selectedAtomY.Content = atomsList[i - 1][iGroup.Children.IndexOf(selectedModel)].Y;
                    selectedAtomZ.Content = atomsList[i - 1][iGroup.Children.IndexOf(selectedModel)].Z;
                }
            }
        }

        private void CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            try
            {
                if (checkBox == checkboxXaxis)
                {
                    MainModel3Dgroup.Children.Add(y_Axis);
                    zLabel.Visibility = Visibility.Visible;
                }
                else if (checkBox == checkboxYaxis)
                {
                    MainModel3Dgroup.Children.Add(x_Axis);
                    xLabel.Visibility = Visibility.Visible;
                }
                else if (checkBox == checkboxZaxis)
                {
                    MainModel3Dgroup.Children.Add(z_Axis);
                    yLabel.Visibility = Visibility.Visible;
                }
            }

            catch (System.ArgumentException)
            {
                Console.WriteLine("first time check");
            }
        }

        private void CheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;

            if (checkBox == checkboxXaxis)
            {
                MainModel3Dgroup.Children.Remove(y_Axis);
                zLabel.Visibility = Visibility.Hidden;
            }
            else if (checkBox == checkboxYaxis)
            {
                MainModel3Dgroup.Children.Remove(x_Axis);
                xLabel.Visibility = Visibility.Hidden;
            }
            else if (checkBox == checkboxZaxis)
            {
                MainModel3Dgroup.Children.Remove(z_Axis);
                yLabel.Visibility = Visibility.Hidden;
            }
        }

        private double DistanceTwoAtoms(Atom atom1, Atom atom2)
        {
            return Math.Sqrt((atom1.X - atom2.X) * (atom1.X - atom2.X) * atomCell.XAxisL * atomCell.XAxisL
                + (atom1.Y - atom2.Y) * (atom1.Y - atom2.Y) * atomCell.YAxisL * atomCell.YAxisL
                + (atom1.Z - atom2.Z) * (atom1.Z - atom2.Z) * atomCell.ZAxisL * atomCell.ZAxisL);
        }

        private double ThreeAtomsAngle(Atom atomA, Atom atomB, Atom atomC)
        {
            double ABx = atomB.X - atomA.X;
            double ABy = atomB.Y - atomA.Y;
            double ABz = atomB.Z - atomA.Z;

            double BCx = atomC.X - atomB.X;
            double BCy = atomC.Y - atomB.Y;
            double BCz = atomC.Z - atomB.Z;

            double ABBC = ABx * BCx + ABy * BCy + ABz * BCz;

            double AB_length = Math.Sqrt(ABx * ABx + ABy * ABy + ABz * ABz);
            double BC_length = Math.Sqrt(BCx * BCx + BCy * BCy + BCz * BCz);

            return 180 - Math.Acos(ABBC/(AB_length * BC_length)) * (180 / Math.PI);
        }
    }
}
