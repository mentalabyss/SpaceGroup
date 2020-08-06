using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace SpaceGroup
{
    public static class Polyhedra
    {
        private static void AddToList(List<Atom> oxygensWithTranslations, Atom atom)
        {
            if (! oxygensWithTranslations.Any(atom.PositionEquals))
                oxygensWithTranslations.Add(atom);
            else
                Console.WriteLine("Already exists");
        }

        public static List<Atom> CalculatePolyhedra(List<Atom> atoms, double Yx, double Zy, double Xz, CrystalCell crystalCell)
        {
            List<Atom> oxygensWithTranslations = new List<Atom>();
            var result = atoms.Where(a => a.Element[0] == 'O');
            List<Atom> oxygens = result.ToList();

            foreach(Atom ox in oxygens)
            {
                AddToList(oxygensWithTranslations, ox);
                AddToList(oxygensWithTranslations, new Atom(ox.Element, (ox.X + 1).ToString(), ox.Y.ToString(), ox.Z.ToString(), ox.Brush));
                AddToList(oxygensWithTranslations, new Atom(ox.Element, (ox.X - 1).ToString(), ox.Y.ToString(), ox.Z.ToString(), ox.Brush));
                AddToList(oxygensWithTranslations, new Atom(ox.Element, ox.X.ToString(), (ox.Y + 1).ToString(), ox.Z.ToString(), ox.Brush));
                AddToList(oxygensWithTranslations, new Atom(ox.Element, ox.X.ToString(), (ox.Y - 1).ToString(), ox.Z.ToString(), ox.Brush));
                AddToList(oxygensWithTranslations, new Atom(ox.Element, ox.X.ToString(), ox.Y.ToString(), (ox.Z + 1).ToString(), ox.Brush));
                AddToList(oxygensWithTranslations, new Atom(ox.Element, ox.X.ToString(), ox.Y.ToString(), (ox.Z - 1).ToString(), ox.Brush));
            }

            List<Atom> allOxygens = new List<Atom>();

            foreach(Atom atom in atoms.Where(a => a.Element[0] != 'O'))
            {
                var res = oxygensWithTranslations.Where(a => a.Element[0] == 'O').OrderBy(a => Distance(atom, a)).Take(4); //oxygens_with_translations
                if (CheckIfPointIsInTetrahydron(atom, res.ToList()))
                {
                    allOxygens = allOxygens.Concat(res.ToList()).ToList();
                    atom.hasPolyhedra = true;
                    atom.PolyhedraAtoms = res.ToList();
                    atom.PolyhedronVolume = CalculatePolyhedronVolume(res.ToList(), crystalCell);
                }
            }

            return allOxygens;
        }

        static double Distance(Atom atom1, Atom atom2)
        {
            double deltaX = atom1.X - atom2.X;
            double deltaY = atom1.Y - atom2.Y;
            double deltaZ = atom1.Z - atom2.Z;

            return (double)Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
        }

        static bool CheckIfPointIsInTetrahydron(Atom centerAtom, List<Atom> oxygens)
        {
            double centerX = (oxygens[0].X + oxygens[1].X + oxygens[2].X + oxygens[3].X) / 4;
            double centerY = (oxygens[0].Y + oxygens[1].Y + oxygens[2].Y + oxygens[3].Y) / 4;
            double centerZ = (oxygens[0].Z + oxygens[1].Z + oxygens[2].Z + oxygens[3].Z) / 4;

            List<Atom> threePoints = new List<Atom> {oxygens[0], oxygens[1], oxygens[2]};


            if (!CalculatePlaneEquation(out var a, out var b, out var c, out var d, threePoints))
                return false;

            //Console.WriteLine($"{a} * {centerAtom.X} + {b} * {centerAtom.Y} + {c} * {centerAtom.Z} + {d} = {a * centerAtom.X + b * centerAtom.Y + c * centerAtom.Z + d}");

            if ((a * centerAtom.X + b * centerAtom.Y + c * centerAtom.Z + d) * (a * centerX + b * centerY + c * centerZ + d) <= 0)
                return false;


            threePoints = new List<Atom> {oxygens[0], oxygens[1], oxygens[3]};


            if (!CalculatePlaneEquation(out a, out b, out c, out d, threePoints))
                return false;

            //Console.WriteLine($"{a} * {centerAtom.X} + {b} * {centerAtom.Y} + {c} * {centerAtom.Z} + {d} = {a * centerAtom.X + b * centerAtom.Y + c * centerAtom.Z + d}");


            if ((a * centerAtom.X + b * centerAtom.Y + c * centerAtom.Z + d) * (a * centerX + b * centerY + c * centerZ + d) <= 0)
                return false;

            threePoints = new List<Atom> {oxygens[3], oxygens[1], oxygens[2]};


            if (!CalculatePlaneEquation(out a, out b, out c, out d, threePoints))
                return false;

            //Console.WriteLine($"{a} * {centerAtom.X} + {b} * {centerAtom.Y} + {c} * {centerAtom.Z} + {d} = {a * centerAtom.X + b * centerAtom.Y + c * centerAtom.Z + d}");


            if ((a * centerAtom.X + b * centerAtom.Y + c * centerAtom.Z + d) * (a * centerX + b * centerY + c * centerZ + d) <= 0)
                return false;

            threePoints = new List<Atom> {oxygens[0], oxygens[3], oxygens[2]};


            if (!CalculatePlaneEquation(out a, out b, out c, out d, threePoints))
                return false;

            //Console.WriteLine($"{a} * {centerAtom.X} + {b} * {centerAtom.Y} + {c} * {centerAtom.Z} + {d} = {a * centerAtom.X + b * centerAtom.Y + c * centerAtom.Z + d}");

            return !((a * centerAtom.X + b * centerAtom.Y + c * centerAtom.Z + d) * (a * centerX + b * centerY + c * centerZ + d) <= 0);
        }

        static bool CalculatePlaneEquation(out double a, out double b, out double c, out double d, List<Atom> threePoints)
        {
            double x1 = threePoints[0].X;
            double y1 = threePoints[0].Y;
            double z1 = threePoints[0].Z;
            double x2 = threePoints[1].X;
            double y2 = threePoints[1].Y;
            double z2 = threePoints[1].Z;
            double x3 = threePoints[2].X;
            double y3 = threePoints[2].Y;
            double z3 = threePoints[2].Z;

            double a1 = x2 - x1;
            double b1 = y2 - y1;
            double c1 = z2 - z1;
            double a2 = x3 - x1;
            double b2 = y3 - y1;
            double c2 = z3 - z1;
            a = b1 * c2 - b2 * c1;
            b = a2 * c1 - a1 * c2;
            c = a1 * b2 - b1 * a2;
            d = (-a * x1 - b * y1 - c * z1);


            return true;
        }

        private static double CalculatePolyhedronVolume(List<Atom> oxygenAtoms, CrystalCell crystalCell)
        {
            Vector3D aVector = new Vector3D(oxygenAtoms[0].X * crystalCell.YAxisL, oxygenAtoms[0].Y * crystalCell.ZAxisL, oxygenAtoms[0].Z * crystalCell.XAxisL);
            Vector3D bVector = new Vector3D(oxygenAtoms[1].X * crystalCell.YAxisL, oxygenAtoms[1].Y * crystalCell.ZAxisL, oxygenAtoms[1].Z * crystalCell.XAxisL);
            Vector3D cVector = new Vector3D(oxygenAtoms[2].X * crystalCell.YAxisL, oxygenAtoms[2].Y * crystalCell.ZAxisL, oxygenAtoms[2].Z * crystalCell.XAxisL);
            Vector3D dVector = new Vector3D(oxygenAtoms[3].X * crystalCell.YAxisL, oxygenAtoms[3].Y * crystalCell.ZAxisL, oxygenAtoms[3].Z * crystalCell.XAxisL);

            var volume = Math.Abs(
                Vector3D.DotProduct(
                    Vector3D.Subtract(aVector, dVector),
                    Vector3D.CrossProduct(
                        Vector3D.Subtract(bVector, dVector),
                        Vector3D.Subtract(cVector, dVector)
                        )
                    )
                );

            return volume;
        }
    }
}
