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
        private Model3DGroup PolyhedraGroup = new Model3DGroup();


        private List<Model3DGroup> atomReproductions = new List<Model3DGroup>();
        private List<GeometryModel3D> SelectableModels = new List<GeometryModel3D>();
        private List<List<Atom>> atomsList = new List<List<Atom>>(); 

        private GeometryModel3D BordersModel;
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


        bool translationsSwitch = false;


        private SolidColorBrush atomColor;
        // The camera.
        private OrthographicCamera TheCamera = new OrthographicCamera();
        private OrthographicCamera AxisSceneCamera = new OrthographicCamera();


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
            atomCell.setCellParams(20.06999, 19.92, 13.42, 90, 90, 90);
            DataGridAtoms.ItemsSource = atomCell.atomCollection;

            // Give the camera its initial position.
            //TheCamera = new OrthographicCamera();
            //AxisSceneCamera = new OrthographicCamera();
            //TheCamera = new PerspectiveCamera();
            TheCamera.Width = 90;
            AxisSceneCamera.Width = 90;
            //TheCamera.FieldOfView = 60;
            MainViewport.Camera = TheCamera;
            AxisViewport.Camera = AxisSceneCamera;
            TheCamera.PositionCamera(atomCell);
            AxisSceneCamera.PositionCamera();

            DefineLights();

            ModelBuilder.buildStaticAxes(out x_Axis, out y_Axis, out z_Axis);
            ModelBuilder.buildDiscreteAxis_TEST(out discrete_y_axis, out discrete_x_axis, out discrete_z_axis, atomCell);
            DiscreteAxisGroup.Children.Add(discrete_x_axis);
            DiscreteAxisGroup.Children.Add(discrete_y_axis);
            DiscreteAxisGroup.Children.Add(discrete_z_axis);
            ModelBuilder.buildCellBorders(out BordersModel, atomCell);

            ModelVisual3D model_visual = new ModelVisual3D();
            model_visual.Content = MainAndDA;

            ModelVisual3D axis_model_visual = new ModelVisual3D();
            axis_model_visual.Content = DiscreteAxisGroup;

            AxisViewport.Children.Add(axis_model_visual);

            //MainModel3Dgroup.Children.Add(x_Axis);
            //MainModel3Dgroup.Children.Add(y_Axis);
            //MainModel3Dgroup.Children.Add(z_Axis);

            MainModel3Dgroup.Children.Add(cells_and_atoms);
            MainModel3Dgroup.Children.Add(PolyhedraGroup);
          
            MainAndDA.Children.Add(MainModel3Dgroup);

            cells_and_atoms.Children.Add(BordersModel);

            MainViewport.Children.Add(model_visual);
            moveFromCenter();
            generateLabels();

        }

        //private void generateLabels()
        //{
        //    //-atomCell.YAxisL / 2, -atomCell.ZAxisL / 2, -atomCell.XAxisL / 2
        //    canvasOn3D.Children.Clear();
        //    //X
        //    Point LabelXPoint = (Point)Point3DToScreen2D(new Point3D(25 - atomCell.YAxisL / 2, -atomCell.ZAxisL / 2, -atomCell.XAxisL / 2), MainViewport);
        //    Canvas.SetLeft(xLabel, LabelXPoint.X);
        //    Canvas.SetTop(xLabel, LabelXPoint.Y);
        //    canvasOn3D.Children.Add(xLabel);

        //    Point LabelYPoint = (Point)Point3DToScreen2D(new Point3D(-atomCell.YAxisL / 2, 25 - atomCell.ZAxisL / 2, -atomCell.XAxisL / 2), MainViewport);
        //    Canvas.SetLeft(yLabel, LabelYPoint.X);
        //    Canvas.SetTop(yLabel, LabelYPoint.Y);
        //    canvasOn3D.Children.Add(yLabel);

        //    Point LabelZPoint = (Point)Point3DToScreen2D(new Point3D(-atomCell.YAxisL / 2, - atomCell.ZAxisL / 2, 25 - atomCell.XAxisL / 2), MainViewport);
        //    Canvas.SetLeft(zLabel, LabelZPoint.X);
        //    Canvas.SetTop(zLabel, LabelZPoint.Y);
        //    canvasOn3D.Children.Add(zLabel);
        //}

        private void generateLabels()
        {
            //-atomCell.YAxisL / 2, -atomCell.ZAxisL / 2, -atomCell.XAxisL / 2
            canvasOn3D.Children.Clear();
            //X
            Point LabelXPoint = (Point)Point3DToScreen2D(new Point3D(5, 0, 0), AxisViewport);
            Canvas.SetLeft(xLabel, LabelXPoint.X);
            Canvas.SetTop(xLabel, LabelXPoint.Y);
            canvasOn3D.Children.Add(xLabel);

            Point LabelYPoint = (Point)Point3DToScreen2D(new Point3D(0, 5, 0), AxisViewport);
            Canvas.SetLeft(yLabel, LabelYPoint.X);
            Canvas.SetTop(yLabel, LabelYPoint.Y);
            canvasOn3D.Children.Add(yLabel);

            Point LabelZPoint = (Point)Point3DToScreen2D(new Point3D(0, 0, 5), AxisViewport);
            Canvas.SetLeft(zLabel, LabelZPoint.X);
            Canvas.SetTop(zLabel, LabelZPoint.Y);
            canvasOn3D.Children.Add(zLabel);
        }

        private void moveFromCenter()
        {
            //var transform = new TranslateTransform3D(-atomCell.XAxisL / 2, -atomCell.YAxisL / 2, -atomCell.ZAxisL / 2);
            var transform = new TranslateTransform3D(-atomCell.YAxisL / 2, -atomCell.ZAxisL / 2, -atomCell.XAxisL / 2);
            MainModel3Dgroup.Transform = transform;
        }

        private void DefineLights()
        {
            AmbientLight ambient_light = new AmbientLight(Colors.Gray);
            DirectionalLight directional_light =
                new DirectionalLight(Colors.Gray, new Vector3D(-1.0, -3.0, -2.0));
            MainModel3Dgroup.Children.Add(ambient_light);
            MainModel3Dgroup.Children.Add(directional_light);

            DiscreteAxisGroup.Children.Add(ambient_light);
            DiscreteAxisGroup.Children.Add(directional_light);
        }

        private void addAtomButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string s = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(atomName.Text.ToLower());
                Atom atom = new Atom(s, xCoord.Text, yCoord.Text, zCoord.Text, atomColor);
                ModelBuilder.visualizeAtom(ref cells_and_atoms, atom, atomColor, selectedSpaceGroup, atomCell, ref SelectableModels, ref multipliedAtoms,
                    ref atomsList, ref atomReproductions);
                atomCell.atomCollection.Add(atom);

                DataGridAtoms.Items.Refresh();
            }
            catch (NullReferenceException)
            {
                System.Windows.Forms.MessageBox.Show("Не выбран цвет!");
            }
            catch (IndexOutOfRangeException)
            {
                System.Windows.Forms.MessageBox.Show("Указаны не все параметры!");
                //MessageBox.Show("Не выбрана группа!");
            }

        }

        private void deleteButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = DataGridAtoms.SelectedIndex;
                Model3DGroup groupToRemove = atomReproductions[index];
                cells_and_atoms.Children.Remove(groupToRemove);
                atomCell.atomCollection.Remove(DataGridAtoms.SelectedItem as Atom);
                //MessageBox.Show((DataGridAtoms.SelectedItem as Atom).Element);
                DataGridAtoms.Items.Refresh();
            }
            catch(ArgumentOutOfRangeException)
            {
                MessageBox.Show("Не выбран элемент для удаления!");
            }
            
        }

        public Point? Point3DToScreen2D(Point3D point3D, Viewport3D viewPort)
        {
            bool bOK = false;

            // We need a Viewport3DVisual but we only have a Viewport3D.
            
            //for(int i = 0; i < viewPort.Children.Count; i++)
            //{
                //Console.WriteLine(viewPort.Children[i].ToString());
            //}

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
                Point mouse_pos = e.GetPosition(MainViewport);

                HitTestResult result = VisualTreeHelper.HitTest(MainViewport, mouse_pos);

                RayMeshGeometry3DHitTestResult mesh_result = result as RayMeshGeometry3DHitTestResult;

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
                    ClearSelectedAtomInfo();
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

                System.Windows.Forms.MessageBox.Show(Atom.DistanceTwoAtoms(atom1, atom2, atomCell).ToString() + " Å");
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

                System.Windows.Forms.MessageBox.Show(Atom.ThreeAtomsAngle(atom1, atom2, atom3).ToString() + "°");
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

                TheCamera.RotateCamera(angleX, angleY);
                AxisSceneCamera.RotateCamera(angleX, angleY);
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

                TheCamera.MoveRight(- mouseDeltaX * 0.5);
                TheCamera.MoveUp(mouseDeltaY * 0.5);
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

                //TheCamera.MoveForward(mouseDeltaY * 0.5);
            }

            //moveAxesWithCamera();
            generateLabels();

        }

        private void newGroup_Click(object sender, RoutedEventArgs e)
        {
            SpaceGroupSettings spaceGroupSettings = new SpaceGroupSettings();
            spaceGroupSettings.Owner = this;
            spaceGroupSettings.Show();
        }

        private void pickColor(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                atomColor = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
            }
        }

        private void draw_translations(object sender, RoutedEventArgs e)
        {
            if (!translationsSwitch)
            {
                translationsSwitch = true;
                ModelBuilder.Translate_Cell("up", ref Upper_Cell_Model, ref cells_and_atoms, atomCell, ref TranslationsGroup);
                ModelBuilder.Translate_Cell("down", ref Upper_Cell_Model, ref cells_and_atoms, atomCell, ref TranslationsGroup);
                ModelBuilder.Translate_Cell("left", ref Upper_Cell_Model, ref cells_and_atoms, atomCell, ref TranslationsGroup);
                ModelBuilder.Translate_Cell("right", ref Upper_Cell_Model, ref cells_and_atoms, atomCell, ref TranslationsGroup);
                ModelBuilder.Translate_Cell("front", ref Upper_Cell_Model, ref cells_and_atoms, atomCell, ref TranslationsGroup);
                ModelBuilder.Translate_Cell("back", ref Upper_Cell_Model, ref cells_and_atoms, atomCell, ref TranslationsGroup);
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
            saveFileDialog1.Filter = "SPG File|*.spg";
            saveFileDialog1.Title = "Save Atoms State";
            saveFileDialog1.ShowDialog();

            if(saveFileDialog1.FileName != "")
                SerializeTableList(saveFileDialog1.FileName);
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

                ModelBuilder.buildCellBorders(out BordersModel, atomCell);
                cells_and_atoms.Children.Add(BordersModel);

                try
                {
                    foreach (Atom a in atomCell.atomCollection)
                    {
                        ModelBuilder.visualizeAtom(ref cells_and_atoms, a, atomColor, selectedSpaceGroup, atomCell, ref SelectableModels, ref multipliedAtoms,
                    ref atomsList, ref atomReproductions);
                    }

                    DataGridAtoms.ItemsSource = atomCell.atomCollection;
                }
                catch (System.NullReferenceException)
                {
                    MessageBox.Show("Не выбрана группа!"); //ВНИМАНИЕ
                }
            }
        }

        private void draw_polyhydras(object sender, RoutedEventArgs e)
        {
            if (PolyhedraGroup.Children.Count == 0)
            {
                ModelBuilder.DrawPolyhedra_TEST(ref PolyhedraGroup, atomCell, multipliedAtoms);
            }

            else
            {
                PolyhedraGroup.Children.Clear();
            }

            // if(polyhedra_model == null)
            //{
            //TranslatePolyhedra();
            //}
            //else
            //{
            //MainModel3Dgroup.Children.Remove(polyhedra_model);
            //polyhedra_model = null;
            //}
        }

        //private void TranslatePolyhedra()
        //{
        //    if(polyhedra_model != null)
        //    {
        //        var polyhedra_model_translated = polyhedra_model.Clone();

        //        var transform = new TranslateTransform3D(0, atomCell.ZAxisL, 0);

        //        polyhedra_model_translated.Transform = transform;
        //        MainModel3Dgroup.Children.Add(polyhedra_model_translated);
        //    }
        //}

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

        private void ClearSelectedAtomInfo()
        {
            selectedAtomName.Content = "";
            selectedAtomX.Content = "";
            selectedAtomY.Content = "";
            selectedAtomZ.Content = "";
        }

        private void CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            //CheckBox checkBox = (CheckBox)sender;
            MenuItem checkBox = (MenuItem)sender;
            try
            {
                if (checkBox == checkboxXaxis)
                {
                    DiscreteAxisGroup.Children.Add(discrete_x_axis);

                    //MainModel3Dgroup.Children.Add(y_Axis);
                    zLabel.Visibility = Visibility.Visible;
                }
                else if (checkBox == checkboxYaxis)
                {
                    DiscreteAxisGroup.Children.Add(discrete_y_axis);
                    //MainModel3Dgroup.Children.Add(x_Axis);
                    xLabel.Visibility = Visibility.Visible;
                }
                else if (checkBox == checkboxZaxis)
                {
                    DiscreteAxisGroup.Children.Add(discrete_z_axis);
                    //MainModel3Dgroup.Children.Add(z_Axis);
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
            MenuItem checkBox = (MenuItem)sender;

            if (checkBox == checkboxXaxis)
            {
                DiscreteAxisGroup.Children.Remove(discrete_x_axis);
                MainModel3Dgroup.Children.Remove(y_Axis);
                zLabel.Visibility = Visibility.Hidden;
            }
            else if (checkBox == checkboxYaxis)
            {
                DiscreteAxisGroup.Children.Remove(discrete_y_axis);

                MainModel3Dgroup.Children.Remove(x_Axis);
                xLabel.Visibility = Visibility.Hidden;
            }
            else if (checkBox == checkboxZaxis)
            {
                DiscreteAxisGroup.Children.Remove(discrete_z_axis);

                MainModel3Dgroup.Children.Remove(z_Axis);
                yLabel.Visibility = Visibility.Hidden;
            }
        }

        // Serializers

        private void SerializeTableList(string fileName)
        {
            List<Atom> atoms = new List<Atom>();
            foreach (Atom a in atomCell.atomCollection)
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
            try
            {
                var mySerializer = new XmlSerializer(typeof(List<Atom>));
                var myFileStream = new FileStream(filename, FileMode.Open);
                List<Atom> myObject = (List<Atom>)mySerializer.Deserialize(myFileStream);
                ObservableCollection<Atom> ret = new ObservableCollection<Atom>();
                foreach (Atom a in myObject)
                {
                    a.StringToColor();
                    ret.Add(a);
                }
                return ret;
            }
            catch (InvalidOperationException)
            {
                System.Windows.Forms.MessageBox.Show("Невозможно открыть файл!");
            }
            return new ObservableCollection<Atom>();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double newWidth = grid.ActualWidth;
            double originalWidth = 509.0;
            double originalNearPlaneDistance = 0.125;
            double originalFieldOfView = 45.0;
            double scale = newWidth / originalWidth * 1.5;

            double fov = Math.Atan(Math.Tan(originalFieldOfView / 2.0 / 180.0 * Math.PI) * scale) * 2.0;
            TheCamera.Width = fov / Math.PI * 180.0;
            TheCamera.NearPlaneDistance = originalNearPlaneDistance * scale;

            //AxisSceneCamera.Width = fov / Math.PI * 180.0 * 0.1;
            //AxisSceneCamera.NearPlaneDistance = originalNearPlaneDistance * scale;        
        }

        private void OnViewportMouseScroll(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                TheCamera.Width -= 5;
            else
                TheCamera.Width += 5;
        }

        private void OnAddCompoundButtonClick(object sender, RoutedEventArgs e)
        {
            CellParamsWindow addCompoundName = new CellParamsWindow();
            addCompoundName.Owner = this;
            addCompoundName.Show();
        }
    }
}
