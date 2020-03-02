using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGroup
{
    public static class Polyhedra
    {
        public static List<Atom> CalculatePolyhedra(List<Atom> atoms)
        {
            List<Atom> allOxygens = new List<Atom>();
            foreach(Atom atom in atoms.Where(a => a.Element[0] != 'O'))
            {
                var result = atoms.Where(a => a.Element[0] == 'O').OrderBy(a => Distance(atom, a)).Take(4);
                Console.WriteLine(result.ToString());
                allOxygens = allOxygens.Concat(result.ToList()).ToList();
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
    }
}
