﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace SpaceGroup
{
    public class AtomVisual : ModelVisual3D
    {
        bool isHidden = false;
        bool hasPolyhedra = false;
        double x;
        double y;
        double z;
        double atomVisualSize;
        string atomColor;
        
        Atom atom;
        Model3DGroup MiscModel3DGroup;

        public AtomVisual() { }

        public AtomVisual(double x, double y, double z, double atomVisualSize, string atomColor) //для создания атомов размножений
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.atomVisualSize = atomVisualSize;
            this.atomColor = atomColor;

            MeshGeometry3D atomMesh = new MeshGeometry3D();

            atomMesh.AddSphere(new Point3D(x, y, z), atomVisualSize, 20, 30);

            SolidColorBrush brush1 = (SolidColorBrush)(new BrushConverter().ConvertFrom(atomColor));

            DiffuseMaterial material1 = new DiffuseMaterial(brush1);

            GeometryModel3D atomModel = new GeometryModel3D(atomMesh, material1);

            Content = atomModel;
        }

        public AtomVisual(Atom atom, string atomColor, SpaceGroupCl selectedSpaceGroup, CrystalCell atomCell,
            ref List<GeometryModel3D> SelectableModels, ref List<Atom> multipliedAtoms)
        {
            if (selectedSpaceGroup == null)
            {
                throw new NotImplementedException();
            }

            this.atom = atom;

            Content = MiscModel3DGroup;

            double atomVisualSize = 0.4;
            if (atom.Element[0] == 'O' && (atom.Element[1] != 's'))
            {
                atomVisualSize = 0.2;
            }

            //Model3DGroup atomRepro = new Model3DGroup();
            List<Atom> atomReproList = new List<Atom>();

            for (int i = 0; i < selectedSpaceGroup.Expressions.Length; i += 3)
            {
                
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

                if (x > atomCell.YAxisL)
                    x -= atomCell.YAxisL;
                if (y > atomCell.ZAxisL)
                    y -= atomCell.ZAxisL;
                if (z > atomCell.XAxisL)
                    z -= atomCell.XAxisL;

                if (X > 1)
                    X -= 1;
                if (Y > 1)
                    Y -= 1;
                if (Z > 1)
                    Z -= 1;

                if (!multipliedAtoms.Contains(new Atom(atom.Element, Z.ToString(), X.ToString(), Y.ToString(), null)))
                {
                    AtomVisual multipliedAtomVisual = new AtomVisual(x, y, z, atomVisualSize, atomColor);
                    Children.Add(multipliedAtomVisual);
                    //AtomsModel3DGroup.Children.Add(atomModel);
                    //SelectableModels.Add(multipliedAtomVisual);
                    multipliedAtoms.Add(new Atom(atom.Element, Z.ToString(), X.ToString(), Y.ToString(), null));
                    atomReproList.Add(new Atom(atom.Element, Z.ToString(), X.ToString(), Y.ToString(), atom.Brush));
                }

                else
                {
                    Console.WriteLine("Found duplicate");
                }
            }
        }

        public void AddPolyhedra(CrystalCell atomCell)
        {
            if (atom.hasPolyhedra)
            {
                MeshGeometry3D polyhedraMesh = new MeshGeometry3D();
                var polyhedraModel = polyhedraMesh.DrawSinglePolyhedra(atom, atomCell, 4);
                MiscModel3DGroup.Children.Add(polyhedraModel);
            }
        }
    }
}
