using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.Threading;
using MessageBox = System.Windows.Forms.MessageBox;

namespace SpaceGroup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Data and Boilerplate


        private List<Atom> _multipliedAtoms;
        private CrystalCell _atomCell;
        private SpaceGroupCl _selectedSpaceGroup;
        private CompoundVisual _compoundVisual;
        private Compound _compound;


        private readonly Model3DGroup _discreteAxisGroup = new Model3DGroup();
        private GeometryModel3D _discreteXAxis;
        private GeometryModel3D _discreteYAxis;
        private GeometryModel3D _discreteZAxis;

        private readonly List<GeometryModel3D> _selectableModels = new List<GeometryModel3D>();
        private List<GeometryModel3D> SelectedModels = new List<GeometryModel3D>();
        private List<CompoundVisual> TranslationsList = new List<CompoundVisual>();
        private ObservableCollection<Atom> selectedAtomBondOxygens = new ObservableCollection<Atom>();


        private bool _groupSelected;
        private bool _compoundSelected;

        private readonly Label _xLabel = new Label();
        private readonly Label _yLabel = new Label();
        private readonly Label _zLabel = new Label();

        // The camera.
        private readonly OrthographicCamera _theCamera = new OrthographicCamera();
        private readonly OrthographicCamera _axisSceneCamera = new OrthographicCamera();

        private double _mouseX;
        private double _mouseY;
        private double _mouseOldX;
        private double _mouseOldY;
        private double _mouseDeltaX;
        private double _mouseDeltaY;
        private bool _mouseDragged;

        public MainWindow()
        {
            InitializeComponent();
        }


        #endregion


        #region Event Handlers
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _groupSelected = false;
            _compoundSelected = false;
            _xLabel.Content = "Y";
            _yLabel.Content = "Z";
            _zLabel.Content = "X";
            _xLabel.FontSize = 25;
            _yLabel.FontSize = 25;
            _zLabel.FontSize = 25;
            _multipliedAtoms = new List<Atom>();
            _atomCell = new CrystalCell();
            _theCamera.Width = 90;
            _axisSceneCamera.Width = 90;
            MainViewport.Camera = _theCamera;
            AxisViewport.Camera = _axisSceneCamera;
            _theCamera.PositionCamera(_atomCell);
            _axisSceneCamera.PositionCamera();
        }

        public void OnViewportMouseDown(object sender, MouseButtonEventArgs e) //On Mouse Click
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
                        var hitModelAtomVisual = GetHitModelAtomVisual(hitModel);
                        if (hitModelAtomVisual != null)
                        {
                            UpdateDistancesTable(hitModelAtomVisual.Atom);
                        }
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

        public void OnViewportMouseMove(object sender, MouseEventArgs e) //On Mouse Hover
        {
            var hitModel = GetHitModel(e);
            var hitModelAtomVisual = GetHitModelAtomVisual(hitModel);
            ShowSelectedAtomInfo(hitModelAtomVisual);
            ShowPopupAtomInfo(hitModelAtomVisual);

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

                _theCamera.RotateCamera(angleX, angleY);
                _axisSceneCamera.RotateCamera(angleX, angleY);
            }
            else
            {
                _mouseDragged = false;
            }

            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                _mouseOldX = _mouseX;
                _mouseOldY = _mouseY;
                Point position = e.GetPosition(this);
                _mouseX = position.X;
                _mouseY = position.Y;
                _mouseDeltaX = (_mouseX - _mouseOldX);
                _mouseDeltaY = (_mouseY - _mouseOldY);

                _theCamera.MoveRight(-_mouseDeltaX * 0.5);
                _theCamera.MoveUp(_mouseDeltaY * 0.5);
            }

            if (e.RightButton == MouseButtonState.Pressed)
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

        private void ShowPopupAtomInfo(AtomVisual atomVisual)
        {
            if (atomVisual == null)
            {
                atomInfoPopText.Text = "";
                atomInfoPopup.IsOpen = false;
                return;
            }
            Atom atom = atomVisual.Atom;
            atomInfoPopText.Text = $"{atom.Element}\n{atom.X}\n{atom.Y}\n{atom.Z}";
            atomInfoPopup.IsOpen = true;
        }

        private void newGroup_Click(object sender, RoutedEventArgs e)
        {
            SpaceGroupSettings spaceGroupSettings = new SpaceGroupSettings { Owner = this };
            spaceGroupSettings.Show();
        }

        private void VisualizeTranslations(object sender, RoutedEventArgs e)
        {
            List<Atom> multipliedAtomsTransl = new List<Atom>();

            var translatedCompoundVisual =
                new CompoundVisual(_compound, _selectedSpaceGroup, _selectableModels, multipliedAtomsTransl, _compoundVisual.ColorTypeDictionary);

            MainViewport.Children.Add(translatedCompoundVisual);

            var transform = new TranslateTransform3D(-_atomCell.YAxisL / 2, -_atomCell.ZAxisL / 2 + _atomCell.ZAxisL, -_atomCell.XAxisL / 2);

            translatedCompoundVisual.Transform = transform;

            TranslationsList.Add(translatedCompoundVisual);
        }

        private void HideTranslations(object sender, RoutedEventArgs e)
        {
            foreach (CompoundVisual cv in TranslationsList)
                MainViewport.Children.Remove(cv);
            TranslationsList = new List<CompoundVisual>();
        }

        private void SaveTableButtonClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog
            {
                Filter = "SPG File|*.spg",
                Title = "Save Atoms State"
            };
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
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
                foreach (AtomVisual atomVisual in cVisual.Children)
                    foreach (AtomVisual atomVisualRep in atomVisual.Children)
                        atomVisualRep.hidePolyhedra();
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
                    _discreteAxisGroup.Children.Add(_discreteXAxis);

                    //MainModel3Dgroup.Children.Add(y_Axis);
                    _zLabel.Visibility = Visibility.Visible;
                }
                else if (checkBox == checkboxYaxis)
                {
                    _discreteAxisGroup.Children.Add(_discreteYAxis);
                    //MainModel3Dgroup.Children.Add(x_Axis);
                    _xLabel.Visibility = Visibility.Visible;
                }
                else if (checkBox == checkboxZaxis)
                {
                    _discreteAxisGroup.Children.Add(_discreteZAxis);
                    //MainModel3Dgroup.Children.Add(z_Axis);
                    _yLabel.Visibility = Visibility.Visible;
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
                _discreteAxisGroup.Children.Remove(_discreteXAxis);
                _zLabel.Visibility = Visibility.Hidden;
            }
            else if (checkBox == checkboxYaxis)
            {
                _discreteAxisGroup.Children.Remove(_discreteYAxis);

                _xLabel.Visibility = Visibility.Hidden;
            }
            else if (checkBox == checkboxZaxis)
            {
                _discreteAxisGroup.Children.Remove(_discreteZAxis);
                _yLabel.Visibility = Visibility.Hidden;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double newWidth = grid.ActualWidth;
            double originalWidth = 509.0;
            double originalNearPlaneDistance = 0.125;
            double originalFieldOfView = 45.0;
            double scale = newWidth / originalWidth * 1.5;

            double fov = Math.Atan(Math.Tan(originalFieldOfView / 2.0 / 180.0 * Math.PI) * scale) * 2.0;
            _theCamera.Width = fov / Math.PI * 180.0;
            _theCamera.NearPlaneDistance = originalNearPlaneDistance * scale;

            //AxisSceneCamera.Width = fov / Math.PI * 180.0 * 0.1;
            //AxisSceneCamera.NearPlaneDistance = originalNearPlaneDistance * scale;        
        }

        private void OnViewportMouseScroll(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                _theCamera.Width -= 5;
            else
                _theCamera.Width += 5;
        }

        private void OnAddCompoundButtonClick(object sender, RoutedEventArgs e)
        {
            CellParamsWindow addCompoundName = new CellParamsWindow { Owner = this };
            addCompoundName.Show();
            //addCompoundName.saveToNewFormat(loadedCompound);
        }

        #endregion


        public void SelectGroup(SpaceGroupCl spaceGroup)
        {
            _selectedSpaceGroup = spaceGroup;
            _groupSelected = true;
            BuildCompound();
        }

        public void InitCompound(Compound compound)
        {
            this._compound = compound;
            _compoundSelected = true;
            BuildCompound();
        }

        private void BuildCompound()
        {
            if (!_compoundSelected || !_groupSelected) return;
            


            _atomCell = _compound.CrystalCell;
            _compoundVisual = new CompoundVisual(_compound, _selectedSpaceGroup, _selectableModels, _multipliedAtoms);

            MainViewport.Children.Clear();
            MainViewport.Children.Add(_compoundVisual);
            MoveFromCenter(_compoundVisual);

            ModelBuilder.BuildDiscreteAxis(out _discreteYAxis, out _discreteXAxis, out _discreteZAxis, _atomCell);


            _discreteAxisGroup.Children.Clear();
            _discreteAxisGroup.Children.Add(_discreteXAxis);
            _discreteAxisGroup.Children.Add(_discreteYAxis);
            _discreteAxisGroup.Children.Add(_discreteZAxis);

            ModelVisual3D axisModelVisual = new ModelVisual3D();

            AmbientLight ambientLight = new AmbientLight(Colors.Gray);
            DirectionalLight directionalLight =
                new DirectionalLight(Colors.Gray, new Vector3D(-1.0, -3.0, -2.0));

            _discreteAxisGroup.Children.Add(ambientLight);
            _discreteAxisGroup.Children.Add(directionalLight);

            axisModelVisual.Content = _discreteAxisGroup;
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
                Canvas.SetLeft(_xLabel, labelXPoint.X);
                Canvas.SetTop(_xLabel, labelXPoint.Y);
                canvasOn3D.Children.Add(_xLabel);

                Point labelYPoint = (Point)Point3DToScreen2D(new Point3D(0, 5, 0), AxisViewport);
                Canvas.SetLeft(_yLabel, labelYPoint.X);
                Canvas.SetTop(_yLabel, labelYPoint.Y);
                canvasOn3D.Children.Add(_yLabel);

                Point labelZPoint = (Point)Point3DToScreen2D(new Point3D(0, 0, 5), AxisViewport);
                Canvas.SetLeft(_zLabel, labelZPoint.X);
                Canvas.SetTop(_zLabel, labelZPoint.Y);
                canvasOn3D.Children.Add(_zLabel);
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("no Axis");
            }
        }

        private void MoveFromCenter(ModelVisual3D model)
        {
            var transform = new TranslateTransform3D(-_atomCell.YAxisL / 2, -_atomCell.ZAxisL / 2, -_atomCell.XAxisL / 2);
            //compoundVisual.Transform = transform;
            model.Transform = transform;
        }

        public Point? Point3DToScreen2D(Point3D point3D, Viewport3D viewPort)
        {
            // We need a Viewport3DVisual but we only have a Viewport3D.

            Viewport3DVisual vpv = VisualTreeHelper.GetParent(viewPort.Children[0]) as Viewport3DVisual;

            // Get the world to viewport transform matrix
            Matrix3D m = MUtils.TryWorldToViewportTransform(vpv, out var bOk);

            if (bOk)
            {
                // Transform the 3D point to 2D
                Point3D transformedPoint = m.Transform(point3D);

                var screen2DPoint = new Point(transformedPoint.X, transformedPoint.Y);

                return screen2DPoint;
            }
            else
            {
                return null;
            }
        }

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


        private void ShowSelectedAtomInfo(AtomVisual atomVisual)
        {
            if (atomVisual == null) return;

            selectedAtomName.Content = atomVisual.Atom.Element;
            selectedAtomX.Content = atomVisual.Atom.X;
            selectedAtomY.Content = atomVisual.Atom.Y;
            selectedAtomZ.Content = atomVisual.Atom.Z;
            selectedAtomPolyhedronVolume.Content = atomVisual.Atom.PolyhedronVolume;
        }

        private AtomVisual GetHitModelAtomVisual(GeometryModel3D selectedModel)
        {
            if (!(selectedModel != null & _compoundVisual != null)) return null;
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


        // Serializers

        private void SerializeTableList(string fileName)
        {
            List<Atom> atoms = new List<Atom>();
            foreach (Atom a in _atomCell.atomCollection)
                atoms.Add(a);

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

        private void UpdateDistancesTable(Atom mainAtom)
        {
            distanceDataGrid.Items.Clear();
            foreach (Atom atom in mainAtom.PolyhedraAtoms)
            {
                distanceDataGrid.Items.Add(new DistancesData { element = atom.Element, distance = Atom.DistanceTwoAtoms(atom, mainAtom, _atomCell).ToString() });
            }
        }

        public struct DistancesData
        {
            public string element { set; get; }
            public string distance { set; get; }
        }
    }
}
