using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using MessageBox = System.Windows.Forms.MessageBox;

namespace SpaceGroup
{
    public class CompoundVisual : ModelVisual3D
    {
        Model3DGroup MiscModel3DGroup;
        Compound compound;
        CrystalCell atomCell;
        Random random = new Random();
        SpaceGroupCl selectedSpaceGroup;
        List<GeometryModel3D> SelectableModels;
        List<Atom> multipliedAtoms;

        public Dictionary<int, string> ColorTypeDictionary { get; set; } = new Dictionary<int, string>();

        public CrystalCell AtomCell
        {
            get => atomCell;
            set => atomCell = value;
        }

        public CompoundVisual() { }



        public CompoundVisual(Compound compound, SpaceGroupCl spaceGroup, List<GeometryModel3D> selectableModels, List<Atom> multipliedAtoms, Dictionary<int, string> colorTypeDictionary = null)
        {
            MiscModel3DGroup = new Model3DGroup();
            Content = MiscModel3DGroup;
            this.compound = compound;
            
            AtomCell = compound.CrystalCell;

            selectedSpaceGroup = spaceGroup;
            this.SelectableModels = selectableModels;
            this.multipliedAtoms = multipliedAtoms;
            MiscModel3DGroup.Children.Add(BuildCellBorders());

            if (colorTypeDictionary == null)
            {
                DefineLights();
            }

            InitializeDictionary(colorTypeDictionary);
            AddAtoms();

            Polyhedra.CalculatePolyhedra(multipliedAtoms, AtomCell.YAxisL, AtomCell.ZAxisL, AtomCell.XAxisL, AtomCell);

            foreach (AtomVisual atomVisual in Children)
                foreach (AtomVisual atomVisualRep in atomVisual.Children)
                    atomVisualRep.AddPolyhedra(AtomCell);
        }

        private GeometryModel3D BuildCellBorders()
        {
            MeshGeometry3D bordersMesh = new MeshGeometry3D();

            Point3D origin = new Point3D(0, 0, 0);
            Point3D xLine = new Point3D(AtomCell.YAxisL, 0, 0);
            Point3D yLine = new Point3D(0, AtomCell.ZAxisL, 0);
            Point3D zLine = new Point3D(0, 0, AtomCell.XAxisL);

            bordersMesh.AddSegment(origin, xLine, new Vector3D(0, 1, 0), 0.04);
            bordersMesh.AddSegment(origin, yLine, new Vector3D(1, 0, 0), 0.04);
            bordersMesh.AddSegment(origin, zLine, new Vector3D(0, 1, 0), 0.04);

            bordersMesh.AddSegment(new Point3D(AtomCell.YAxisL, 0, 0), new Point3D(AtomCell.YAxisL, 0, AtomCell.XAxisL), new Vector3D(0, 1, 0), 0.04);
            bordersMesh.AddSegment(new Point3D(AtomCell.YAxisL, AtomCell.ZAxisL, 0), new Point3D(AtomCell.YAxisL, AtomCell.ZAxisL, AtomCell.XAxisL), new Vector3D(0, 1, 0), 0.04);
            bordersMesh.AddSegment(new Point3D(0, AtomCell.ZAxisL, 0), new Point3D(0, AtomCell.ZAxisL, AtomCell.XAxisL), new Vector3D(0, 1, 0), 0.04);


            bordersMesh.AddSegment(new Point3D(0, 0, AtomCell.XAxisL), new Point3D(AtomCell.YAxisL, 0, AtomCell.XAxisL), new Vector3D(0, 1, 0), 0.04);
            bordersMesh.AddSegment(new Point3D(0, AtomCell.ZAxisL, AtomCell.XAxisL), new Point3D(AtomCell.YAxisL, AtomCell.ZAxisL, AtomCell.XAxisL), new Vector3D(0, 1, 0), 0.04);
            bordersMesh.AddSegment(new Point3D(0, AtomCell.ZAxisL, 0), new Point3D(AtomCell.YAxisL, AtomCell.ZAxisL, 0), new Vector3D(0, 1, 0), 0.04);


            bordersMesh.AddSegment(new Point3D(AtomCell.YAxisL, 0, 0), new Point3D(AtomCell.YAxisL, AtomCell.ZAxisL, 0), new Vector3D(1, 0, 0), 0.04);
            bordersMesh.AddSegment(new Point3D(0, 0, AtomCell.XAxisL), new Point3D(0, AtomCell.ZAxisL, AtomCell.XAxisL), new Vector3D(1, 0, 0), 0.04);
            bordersMesh.AddSegment(new Point3D(AtomCell.YAxisL, 0, AtomCell.XAxisL), new Point3D(AtomCell.YAxisL, AtomCell.ZAxisL, AtomCell.XAxisL), new Vector3D(1, 0, 0), 0.04);


            SolidColorBrush bordersBrush = System.Windows.Media.Brushes.Black;
            DiffuseMaterial bordersMaterial = new DiffuseMaterial(bordersBrush);
            return new GeometryModel3D(bordersMesh, bordersMaterial);
        }

        private void AddAtoms()
        {
            foreach (Atom atom in compound.Atoms)
            {
                try
                {
                    AtomVisual atomVisual = new AtomVisual(atom, ColorTypeDictionary[atom.TypeID], selectedSpaceGroup, AtomCell, multipliedAtoms);
                    Children.Add(atomVisual);
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

        private void AddTranslatedAtoms(double xL, double yL, double zL)
        {
            foreach (Atom atom in compound.Atoms)
            {
                try
                {
                    AtomVisual atomVisual = new AtomVisual(atom, ColorTypeDictionary[atom.TypeID], selectedSpaceGroup, AtomCell, multipliedAtoms,
                        xL, yL, zL);
                    Children.Add(atomVisual);
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

        private void InitializeDictionary(Dictionary<int, string> colorTypeDictionary = null)
        {
            if (colorTypeDictionary != null)
            {
                ColorTypeDictionary = colorTypeDictionary;
                return;
            }


            ColorTypeDictionary.Add(0, "#FFFF0000");

            for (int i = 1; i < compound.atomTypesDict.Count; i++)
            {
                var color = $"#{random.Next(0x1000000):X6}"; // = "#A197B9"
                ColorTypeDictionary.Add(i, color);
                //imageSettingsGrid.RowDefinitions.Add(new RowDefinition());

                var colorPickerButton = new Button();
                colorPickerButton.MinWidth = 5;
                colorPickerButton.MinHeight = 20;
                colorPickerButton.Margin = new Thickness(5);
                colorPickerButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(color));
            }
        }

        internal void DefineLights()
        {
            AmbientLight ambientLight = new AmbientLight(Colors.Gray);
            DirectionalLight directionalLight =
                new DirectionalLight(Colors.Gray, new Vector3D(-1.0, -3.0, -2.0));

            MiscModel3DGroup.Children.Add(ambientLight);
            MiscModel3DGroup.Children.Add(directionalLight);
        }

    }
}
