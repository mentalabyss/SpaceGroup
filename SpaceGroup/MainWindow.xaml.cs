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
using MessageBox = System.Windows.Forms.MessageBox;

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
        private AtomVisual selectedAtomVisual;
        CompoundVisual compoundVisual;
        Compound compound;

        private Model3DGroup DiscreteAxisGroup = new Model3DGroup();
        private GeometryModel3D discrete_x_axis;
        private GeometryModel3D discrete_y_axis;
        private GeometryModel3D discrete_z_axis;

        private List<GeometryModel3D> SelectableModels = new List<GeometryModel3D>();

        private bool groupSelected;
        private bool compoundSelected;

        Label xLabel = new Label();
        Label yLabel = new Label();
        Label zLabel = new Label();

        // The camera.
        private OrthographicCamera TheCamera = new OrthographicCamera();
        private OrthographicCamera AxisSceneCamera = new OrthographicCamera();

        //Compound loadedCompound = new Compound();

        public MainWindow()
        {
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            groupSelected = false;
            compoundSelected = false;
            xLabel.Content = "Y";
            yLabel.Content = "Z";
            zLabel.Content = "X";
            xLabel.FontSize = 25;
            yLabel.FontSize = 25;
            zLabel.FontSize = 25;
            multipliedAtoms = new List<Atom>();
            atomCell = new CrystalCell();
            TheCamera.Width = 90;
            AxisSceneCamera.Width = 90;
            MainViewport.Camera = TheCamera;
            AxisViewport.Camera = AxisSceneCamera;
            TheCamera.PositionCamera(atomCell);
            AxisSceneCamera.PositionCamera();
        }

        public void SelectGroup(SpaceGroupCl spaceGroup)
        {
            selectedSpaceGroup = spaceGroup;
            groupSelected = true;
            BuildCompound();
        }

        public void InitCompound(Compound compound)
        {
            this.compound = compound;
            compoundSelected = true;
            BuildCompound();
        }

        private void BuildCompound()
        {
            if (!compoundSelected || !groupSelected) return;
            
            atomCell = compound.CrystalCell;
            compoundVisual = new CompoundVisual(compound, selectedSpaceGroup, SelectableModels, multipliedAtoms);

            MainViewport.Children.Add(compoundVisual);
            MoveFromCenter(compoundVisual);

            ModelBuilder.buildDiscreteAxis(out discrete_y_axis, out discrete_x_axis, out discrete_z_axis, atomCell);

            DiscreteAxisGroup.Children.Add(discrete_x_axis);
            DiscreteAxisGroup.Children.Add(discrete_y_axis);
            DiscreteAxisGroup.Children.Add(discrete_z_axis);

            ModelVisual3D axisModelVisual = new ModelVisual3D();

            AmbientLight ambientLight = new AmbientLight(Colors.Gray);
            DirectionalLight directionalLight =
                new DirectionalLight(Colors.Gray, new Vector3D(-1.0, -3.0, -2.0));

            DiscreteAxisGroup.Children.Add(ambientLight);
            DiscreteAxisGroup.Children.Add(directionalLight);

            axisModelVisual.Content = DiscreteAxisGroup;
            AxisViewport.Children.Add(axisModelVisual);
        }

        private void GenerateLabels()
        {
            try
            {
                //-atomCell.YAxisL / 2, -atomCell.ZAxisL / 2, -atomCell.XAxisL / 2
                canvasOn3D.Children.Clear();
                //X
                Point labelXPoint = (Point)Point3DToScreen2D(new Point3D(5, 0, 0), AxisViewport);
                Canvas.SetLeft(xLabel, labelXPoint.X);
                Canvas.SetTop(xLabel, labelXPoint.Y);
                canvasOn3D.Children.Add(xLabel);

                Point labelYPoint = (Point)Point3DToScreen2D(new Point3D(0, 5, 0), AxisViewport);
                Canvas.SetLeft(yLabel, labelYPoint.X);
                Canvas.SetTop(yLabel, labelYPoint.Y);
                canvasOn3D.Children.Add(yLabel);

                Point labelZPoint = (Point)Point3DToScreen2D(new Point3D(0, 0, 5), AxisViewport);
                Canvas.SetLeft(zLabel, labelZPoint.X);
                Canvas.SetTop(zLabel, labelZPoint.Y);
                canvasOn3D.Children.Add(zLabel);
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("no Axis");
            }
        }

        private void MoveFromCenter(ModelVisual3D model)
        {
            var transform = new TranslateTransform3D(-atomCell.YAxisL / 2, -atomCell.ZAxisL / 2, -atomCell.XAxisL / 2);
            //compoundVisual.Transform = transform;
            model.Transform = transform;
        }

        public Point? Point3DToScreen2D(Point3D point3D, Viewport3D viewPort)
        {
            bool bOK = false;

            // We need a Viewport3DVisual but we only have a Viewport3D.

            Viewport3DVisual vpv = VisualTreeHelper.GetParent(viewPort.Children[0]) as Viewport3DVisual;

            // Get the world to viewport transform matrix
            Matrix3D m = MUtils.TryWorldToViewportTransform(vpv, out bOK);

            if (bOK)
            {
                // Transform the 3D point to 2D
                Point3D transformedPoint = m.Transform(point3D);

                var screen2DPoint = new Point(transformedPoint.X, transformedPoint.Y);

                return new Point?(screen2DPoint);
            }
            else
            {
                return null;
            }
        }

        bool _mouseDragged;

        List<GeometryModel3D> SelectedModels = new List<GeometryModel3D>();

        public void OnViewportMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                if (!contextMenu1.IsOpen)
                {
                    Point position = e.GetPosition(this);
                    _mouseX = position.X;
                    _mouseY = position.Y;
                    if (!_mouseDragged)
                    {
                        _mouseOldX = position.X;
                        _mouseOldY = position.Y;
                    }

                    var hitModel = GetHitModel(e);

                    if (hitModel != null)
                    {
                        SelectedModels.Add(hitModel);
                    }
                    else
                        SelectedModels.Clear();
                }
            }
        }

        public void SinglePolyhedraButtonClicked(object sender, RoutedEventArgs e)
        {
            foreach (var selectedModel in SelectedModels)
            {
                var selectedVisual = GetHitModelAtomVisual(selectedModel);
                selectedVisual?.showPolyhedra();
            }
        }

        public void SinglePolyhedraDeleteButtonClicked(object sender, RoutedEventArgs e)
        {
            foreach (var selectedModel in SelectedModels)
            {
                var selectedVisual = GetHitModelAtomVisual(selectedModel);
                selectedVisual?.hidePolyhedra();
            }
        }

        private double _mouseX;
        private double _mouseY;
        private double _mouseOldX;
        private double _mouseOldY;
        private double _mouseDeltaX;
        private double _mouseDeltaY;

        private GeometryModel3D GetHitModel(MouseEventArgs e)
        {
            Point mouse_pos = e.GetPosition(MainViewport);
            HitTestResult result = VisualTreeHelper.HitTest(MainViewport, mouse_pos);

            if (result is RayMeshGeometry3DHitTestResult meshResult)
            {
                 var model = (GeometryModel3D)meshResult.ModelHit;
                 return model;
            }

            return null;
        }

        public void OnViewportMouseMove(object sender, MouseEventArgs e)
        {
            var hitModel = GetHitModel(e);
            var hitModelAtomVisual = GetHitModelAtomVisual(hitModel);
            ShowSelectedAtomInfo(hitModelAtomVisual);
            selectedAtomVisual = hitModelAtomVisual;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _mouseDragged = true;
                _mouseOldX = _mouseX;
                _mouseOldY = _mouseY;
                Point position = e.GetPosition(this);
                _mouseX = position.X;
                _mouseY = position.Y;
                _mouseDeltaX = (_mouseX - _mouseOldX);
                _mouseDeltaY = (_mouseY - _mouseOldY);

                double angleX = _mouseDeltaX * 0.1;
                double angleY = _mouseDeltaY * 0.1;

                TheCamera.RotateCamera(angleX, angleY);
                AxisSceneCamera.RotateCamera(angleX, angleY);
            }
            else
            {
                _mouseDragged = false;
            }

            if(e.MiddleButton == MouseButtonState.Pressed)
            {
                _mouseOldX = _mouseX;
                _mouseOldY = _mouseY;
                Point position = e.GetPosition(this);
                _mouseX = position.X;
                _mouseY = position.Y;
                _mouseDeltaX = (_mouseX - _mouseOldX);
                _mouseDeltaY = (_mouseY - _mouseOldY);

                TheCamera.MoveRight(- _mouseDeltaX * 0.5);
                TheCamera.MoveUp(_mouseDeltaY * 0.5);
            }

            if(e.RightButton == MouseButtonState.Pressed)
            {
                _mouseOldX = _mouseX;
                _mouseOldY = _mouseY;
                Point position = e.GetPosition(this);
                _mouseX = position.X;
                _mouseY = position.Y;
                _mouseDeltaX = (_mouseX - _mouseOldX);
                _mouseDeltaY = (_mouseY - _mouseOldY);


                //TheCamera.MoveForward(mouseDeltaY * 0.5);
            }

            GenerateLabels();
        }

        private void newGroup_Click(object sender, RoutedEventArgs e)
        {
            SpaceGroupSettings spaceGroupSettings = new SpaceGroupSettings {Owner = this};
            spaceGroupSettings.Show();
        }

        private void VisualizeTranslations(object sender, RoutedEventArgs e)
        {
            List<Atom> multipliedAtomsTransl = new List<Atom>();

            var translatedCompoundVisual =
                new CompoundVisual(compound, selectedSpaceGroup, SelectableModels, multipliedAtomsTransl, compoundVisual.ColorTypeDictionary);

            MainViewport.Children.Add(translatedCompoundVisual);

            var transform = new TranslateTransform3D(-atomCell.YAxisL / 2, -atomCell.ZAxisL / 2 + atomCell.ZAxisL, -atomCell.XAxisL / 2);

            translatedCompoundVisual.Transform = transform;
        }

        private void SaveTableButtonClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog
            {
                Filter = "SPG File|*.spg", Title = "Save Atoms State"
            };
            saveFileDialog1.ShowDialog();

            if(saveFileDialog1.FileName != "")
                SerializeTableList(saveFileDialog1.FileName);
        }

        private void openBtnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void ShowAllPolyhedras(object sender, RoutedEventArgs e)
        {
            foreach (CompoundVisual cVisual in MainViewport.Children)
                foreach (AtomVisual atomVisual in cVisual.Children)
                    foreach (AtomVisual atomVisualRep in atomVisual.Children)
                        atomVisualRep.showPolyhedra();
        }

        private void DeleteAllPolyhedras(object sender, RoutedEventArgs e)
        {
            foreach (CompoundVisual cVisual in MainViewport.Children)
                foreach (AtomVisual atomVisual in compoundVisual.Children)
                    foreach (AtomVisual atomVisualRep in atomVisual.Children)
                        atomVisualRep.hidePolyhedra();
        }

        private void ShowSelectedAtomInfo(AtomVisual atomVisual)
        {
            if (atomVisual != null)
            {
                selectedAtomName.Content = atomVisual.Atom.Element;
                selectedAtomX.Content = atomVisual.Atom.X;
                selectedAtomY.Content = atomVisual.Atom.Y;
                selectedAtomZ.Content = atomVisual.Atom.Z;
            }
        }

        private AtomVisual GetHitModelAtomVisual(GeometryModel3D selectedModel)
        {
            if (!(selectedModel != null & compoundVisual != null)) return null;
            foreach (CompoundVisual cVisual in MainViewport.Children)
            foreach (AtomVisual atomVisual in cVisual.Children)
            foreach (AtomVisual atomVisualRep in atomVisual.Children)
            foreach (GeometryModel3D model in ((Model3DGroup)atomVisualRep.Content).Children)
                if (model == selectedModel)
                    return atomVisualRep;

            return null;
        }

        private void ClearSelectedAtomInfo()
        {
            selectedAtomName.Content = "";
            selectedAtomX.Content = "";
            selectedAtomY.Content = "";
            selectedAtomZ.Content = "";
        }

        //HOMAGE TO GAME DEVS
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
