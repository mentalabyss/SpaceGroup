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
using System.Collections;
using System.Text.RegularExpressions;

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
        Compound selectedCompound;

        // The main object model group.
        private Model3DGroup cells_and_atoms = new Model3DGroup();
        private Model3DGroup MainModel3Dgroup = new Model3DGroup();
        private Model3DGroup DiscreteAxisGroup = new Model3DGroup();
        private Model3DGroup MainAndDA = new Model3DGroup();
        private Model3DGroup TranslationsGroup = new Model3DGroup();
        private Model3DGroup PolyhedraGroup = new Model3DGroup();
        private Model3DGroup TranslationsGr;


        private List<Model3DGroup> atomReproductions = new List<Model3DGroup>();
        private List<GeometryModel3D> SelectableModels = new List<GeometryModel3D>();
        private List<List<Atom>> atomsList = new List<List<Atom>>(); 

        private GeometryModel3D BordersModel;
        private GeometryModel3D discrete_x_axis;
        private GeometryModel3D discrete_y_axis;
        private GeometryModel3D discrete_z_axis;
        private GeometryModel3D SelectedModel = null;


        private Material SelectedMaterial = new DiffuseMaterial(Brushes.Black);
        private Material NormalMaterial = new DiffuseMaterial();

        private Dictionary<int, string> colorTypeDictionary = new Dictionary<int, string>();

        private bool groupSelected;
        private bool compoundSelected;

        Label xLabel = new Label();
        Label yLabel = new Label();
        Label zLabel = new Label();

        private Random random = new Random();

        bool translationsSwitch = false;


        private SolidColorBrush atomColor;
        // The camera.
        private OrthographicCamera TheCamera = new OrthographicCamera();
        private OrthographicCamera AxisSceneCamera = new OrthographicCamera();

        Compound loadedCompound = new Compound();

        public MainWindow()
        {
            InitializeComponent();
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
            //selectedSpaceGroup = new SpaceGroupCl();
            TheCamera.Width = 90;
            AxisSceneCamera.Width = 90;
            MainViewport.Camera = TheCamera;
            AxisViewport.Camera = AxisSceneCamera;
            TheCamera.PositionCamera(atomCell);
            AxisSceneCamera.PositionCamera();

            DefineLights();


            moveFromCenter();

            MainModel3Dgroup.Children.Add(cells_and_atoms);
            MainModel3Dgroup.Children.Add(PolyhedraGroup);

            MainAndDA.Children.Add(MainModel3Dgroup);

            ModelVisual3D model_visual = new ModelVisual3D();
            model_visual.Content = MainAndDA;

            MainViewport.Children.Add(model_visual);

            //add red-oxygen pair to dictionary
            colorTypeDictionary.Add(0, "#FFFF0000");

            groupSelected = false;
            compoundSelected = false;
        }

        public void selectGroup(SpaceGroupCl spaceGroup)
        {
            if (!groupSelected)
            {
                selectedSpaceGroup = spaceGroup;

                visualizeAtom();
            }
        }

        public void initCompound(Compound compound)
        {
            selectedCompound = compound;
            
            for (int i = 1; i < compound.atomTypesDict.Count; i++)
            {
                var color = String.Format("#{0:X6}", random.Next(0x1000000)); // = "#A197B9"
                colorTypeDictionary.Add(i, color);
                imageSettingsGrid.RowDefinitions.Add(new RowDefinition());

                var colorPickerButton = new Button();
                colorPickerButton.MinWidth = 5;
                colorPickerButton.MinHeight = 20;
                colorPickerButton.Margin = new Thickness(5);
                colorPickerButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(color));

                var elementLabel = new Label();

                //elementLabel.Content

                //var dockPanel = new DockPanel();

                imageSettingsGrid.Children.Add(colorPickerButton);
                Grid.SetRow(colorPickerButton, imageSettingsGrid.RowDefinitions.Count - 1);
            }

            compoundSelected = true;
        }

        public void buildCompound()
        {
            atomCell = selectedCompound.CrystalCell;

            ModelBuilder.buildCellBorders(out BordersModel, atomCell);

            cells_and_atoms.Children.Add(BordersModel);

            moveFromCenter();

            ModelBuilder.buildDiscreteAxis(out discrete_y_axis, out discrete_x_axis, out discrete_z_axis, atomCell);
            DiscreteAxisGroup.Children.Add(discrete_x_axis);
            DiscreteAxisGroup.Children.Add(discrete_y_axis);
            DiscreteAxisGroup.Children.Add(discrete_z_axis);

            ModelVisual3D axis_model_visual = new ModelVisual3D();
            axis_model_visual.Content = DiscreteAxisGroup;

            AxisViewport.Children.Add(axis_model_visual);

            visualizeAtom();
        }

        private void generateLabels()
        {
            try
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
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("no Axis");
            }
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

                    //SelectedModel = GetHitModel(e);
                    var hitModel = GetHitModel(e);

                    if (hitModel != null)
                        SelectedModels.Add(hitModel);
                    else
                        SelectedModels.Clear();
                }
            }
        }

        public void SinglePolyhedraButtonClicked(object sender, RoutedEventArgs e)
        {
            foreach (var selectedModel in SelectedModels)
            {
                for (int i = 1; i < cells_and_atoms.Children.Count; i++)
                {
                    var iGroup = (Model3DGroup)cells_and_atoms.Children[i];
                    if (iGroup.Children.IndexOf(selectedModel) > -1)
                    {
                        MeshGeometry3D polyhedraMesh = new MeshGeometry3D();
                        //Polyhedra.CalculatePolyhedra(multipliedAtoms, atomCell.YAxisL, atomCell.ZAxisL, atomCell.XAxisL);
                        var polyhedraModel = polyhedraMesh.DrawSinglePolyhedra(multipliedAtoms[iGroup.Children.IndexOf(selectedModel) + iGroup.Children.Count * (i - 1)], atomCell, PolyhedraGroup, 4);
                        SelectableModels.Remove(selectedModel);
                    }
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

        private GeometryModel3D GetHitModel(MouseEventArgs e)
        {
            Point mouse_pos = e.GetPosition(MainViewport);

            HitTestResult result = VisualTreeHelper.HitTest(MainViewport, mouse_pos);

            RayMeshGeometry3DHitTestResult mesh_result = result as RayMeshGeometry3DHitTestResult;

            if (mesh_result != null)
            {
                GeometryModel3D model = (GeometryModel3D)mesh_result.ModelHit;

                if (PolyhedraGroup.Children.Contains(model))
                {
                    var polyModel = model;
                    PolyhedraGroup.Children.Remove(polyModel);

                    mouse_pos = e.GetPosition(MainViewport);

                    result = VisualTreeHelper.HitTest(MainViewport, mouse_pos);

                    mesh_result = result as RayMeshGeometry3DHitTestResult;

                    PolyhedraGroup.Children.Add(polyModel);
                }

                if (mesh_result != null)
                {
                    model = (GeometryModel3D)mesh_result.ModelHit;
                    return model;
                }
            }

            return null;
        }

        public void OnViewportMouseMove(object sender, MouseEventArgs e)
        {
            var hitModel = GetHitModel(e);
            ShowSelectedAtomInfo(hitModel);

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

        private int originalPolyCount = 0;

        private void draw_translations(object sender, RoutedEventArgs e)
        {
            if (!translationsSwitch)
            {
                translationsSwitch = true;
                //TODO
                ModelBuilder.Translate_Cell("up", TranslationsGr, cells_and_atoms, atomCell, TranslationsGroup);
                ModelBuilder.Translate_Cell("down", TranslationsGr, cells_and_atoms, atomCell, TranslationsGroup);
                ModelBuilder.Translate_Cell("left", TranslationsGr, cells_and_atoms, atomCell, TranslationsGroup);
                ModelBuilder.Translate_Cell("right", TranslationsGr, cells_and_atoms, atomCell, TranslationsGroup);
                ModelBuilder.Translate_Cell("front", TranslationsGr, cells_and_atoms, atomCell, TranslationsGroup);
                ModelBuilder.Translate_Cell("back", TranslationsGr, cells_and_atoms, atomCell, TranslationsGroup);
                originalPolyCount = PolyhedraGroup.Children.Count;
                ModelBuilder.Translate_Polyhedra(PolyhedraGroup, atomCell);
                MainModel3Dgroup.Children.Add(TranslationsGroup);
            }
            else
            {
                translationsSwitch = false;
                MainModel3Dgroup.Children.Remove(TranslationsGroup);
                TranslationsGroup.Children.Clear();
                for (int i = originalPolyCount; i < PolyhedraGroup.Children.Count; i++)
                {
                    PolyhedraGroup.Children.RemoveAt(i);
                }
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

                atomCell.setCellParams(20.06999, 19.92, 13.42, 90, 90, 90);

                atomCell.atomCollection = DeserializeTableList(filename);


                //TEMP
                //loadedCompound.CrystalCell = atomCell;
                //loadedCompound.Atoms = atomCell.atomCollection.ToList();
                //loadedCompound.Name = "Si12O40";

                //TEMP

                cells_and_atoms.Children.Clear();

                ModelBuilder.buildCellBorders(out BordersModel, atomCell);

                cells_and_atoms.Children.Add(BordersModel);

                moveFromCenter();

                visualizeAtom();
            }
        }

        private void visualizeAtom()
        {
            if (compoundSelected)
                foreach (Atom atom in selectedCompound.Atoms)
                {
                    try
                    {
                        ModelBuilder.visualizeAtom(cells_and_atoms, atom, colorTypeDictionary[atom.TypeID], selectedSpaceGroup, atomCell, ref SelectableModels, ref multipliedAtoms,
                           ref atomsList, ref atomReproductions);

                        groupSelected = true;
                    }
                    catch (NullReferenceException)
                    {
                        System.Windows.Forms.MessageBox.Show("Не выбрана группа!");
                        break;
                    }
                    catch (NotImplementedException)
                    {
                        System.Windows.Forms.MessageBox.Show("Test");
                        break;
                    }
                }
        }

        private void draw_polyhydras(object sender, RoutedEventArgs e)
        {
            if (PolyhedraGroup.Children.Count == 0)
            {
                ModelBuilder.DrawPolyhedra(PolyhedraGroup, atomCell, multipliedAtoms, ref SelectableModels, ref atomsList);
            }

            else
            {
                PolyhedraGroup.Children.Clear();
            }
        }

        private void ShowSelectedAtomInfo(GeometryModel3D selectedModel)
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
                zLabel.Visibility = Visibility.Hidden;
            }
            else if (checkBox == checkboxYaxis)
            {
                DiscreteAxisGroup.Children.Remove(discrete_y_axis);

                xLabel.Visibility = Visibility.Hidden;
            }
            else if (checkBox == checkboxZaxis)
            {
                DiscreteAxisGroup.Children.Remove(discrete_z_axis);
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
            //addCompoundName.saveToNewFormat(loadedCompound);
        }
    }
}
