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

        private Dictionary<int, string> colorTypeDictionary = new Dictionary<int, string>();

        public CompoundVisual() { }

        public CompoundVisual(Compound compound, SpaceGroupCl spaceGroup, List<GeometryModel3D> SelectableModels, List<Atom> multipliedAtoms)
        {
            MiscModel3DGroup = new Model3DGroup();
            Content = MiscModel3DGroup;
            DefineLights();
            this.compound = compound;
            atomCell = compound.CrystalCell;
            selectedSpaceGroup = spaceGroup;
            this.SelectableModels = SelectableModels;
            this.multipliedAtoms = multipliedAtoms;
            MiscModel3DGroup.Children.Add(BuildCellBorders());

            initializeDictionary();

            AddAtoms();

            Polyhedra.CalculatePolyhedra(multipliedAtoms, atomCell.YAxisL, atomCell.ZAxisL, atomCell.XAxisL);

            foreach (AtomVisual atomVisual in Children)
                foreach (AtomVisual atomVisualRep in atomVisual.Children)
                    atomVisualRep.AddPolyhedra(atomCell);
        }

        private GeometryModel3D BuildCellBorders()
        {
            MeshGeometry3D borders_mesh = new MeshGeometry3D();

            Point3D origin = new Point3D(0, 0, 0);
            Point3D xLine = new Point3D(atomCell.YAxisL, 0, 0);
            Point3D yLine = new Point3D(0, atomCell.ZAxisL, 0);
            Point3D zLine = new Point3D(0, 0, atomCell.XAxisL);

            borders_mesh.AddSegment(origin, xLine, new Vector3D(0, 1, 0), 0.04);
            borders_mesh.AddSegment(origin, yLine, new Vector3D(1, 0, 0), 0.04);
            borders_mesh.AddSegment(origin, zLine, new Vector3D(0, 1, 0), 0.04);

            borders_mesh.AddSegment(new Point3D(atomCell.YAxisL, 0, 0), new Point3D(atomCell.YAxisL, 0, atomCell.XAxisL), new Vector3D(0, 1, 0), 0.04);
            borders_mesh.AddSegment(new Point3D(atomCell.YAxisL, atomCell.ZAxisL, 0), new Point3D(atomCell.YAxisL, atomCell.ZAxisL, atomCell.XAxisL), new Vector3D(0, 1, 0), 0.04);
            borders_mesh.AddSegment(new Point3D(0, atomCell.ZAxisL, 0), new Point3D(0, atomCell.ZAxisL, atomCell.XAxisL), new Vector3D(0, 1, 0), 0.04);


            borders_mesh.AddSegment(new Point3D(0, 0, atomCell.XAxisL), new Point3D(atomCell.YAxisL, 0, atomCell.XAxisL), new Vector3D(0, 1, 0), 0.04);
            borders_mesh.AddSegment(new Point3D(0, atomCell.ZAxisL, atomCell.XAxisL), new Point3D(atomCell.YAxisL, atomCell.ZAxisL, atomCell.XAxisL), new Vector3D(0, 1, 0), 0.04);
            borders_mesh.AddSegment(new Point3D(0, atomCell.ZAxisL, 0), new Point3D(atomCell.YAxisL, atomCell.ZAxisL, 0), new Vector3D(0, 1, 0), 0.04);


            borders_mesh.AddSegment(new Point3D(atomCell.YAxisL, 0, 0), new Point3D(atomCell.YAxisL, atomCell.ZAxisL, 0), new Vector3D(1, 0, 0), 0.04);
            borders_mesh.AddSegment(new Point3D(0, 0, atomCell.XAxisL), new Point3D(0, atomCell.ZAxisL, atomCell.XAxisL), new Vector3D(1, 0, 0), 0.04);
            borders_mesh.AddSegment(new Point3D(atomCell.YAxisL, 0, atomCell.XAxisL), new Point3D(atomCell.YAxisL, atomCell.ZAxisL, atomCell.XAxisL), new Vector3D(1, 0, 0), 0.04);


            SolidColorBrush borders_brush = System.Windows.Media.Brushes.Black;
            DiffuseMaterial borders_material = new DiffuseMaterial(borders_brush);
            return new GeometryModel3D(borders_mesh, borders_material);
        }

        private void AddAtoms()
        {
            foreach (Atom atom in compound.Atoms)
            {
                try
                {
                    AtomVisual atomVisual = new AtomVisual(atom, colorTypeDictionary[atom.TypeID], selectedSpaceGroup, atomCell,
                        ref SelectableModels, ref multipliedAtoms);
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

        private void initializeDictionary()
        {
            colorTypeDictionary.Add(0, "#FFFF0000");

            for (int i = 1; i < compound.atomTypesDict.Count; i++)
            {
                var color = String.Format("#{0:X6}", random.Next(0x1000000)); // = "#A197B9"
                colorTypeDictionary.Add(i, color);
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
            AmbientLight ambient_light = new AmbientLight(Colors.Gray);
            DirectionalLight directional_light =
                new DirectionalLight(Colors.Gray, new Vector3D(-1.0, -3.0, -2.0));

            MiscModel3DGroup.Children.Add(ambient_light);
            MiscModel3DGroup.Children.Add(directional_light);
        }
    }
}
