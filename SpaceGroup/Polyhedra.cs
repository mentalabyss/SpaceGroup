using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGroup
{
    public static class Polyhedra
    {
        private static void AddToList(List<Atom> oxygens_with_translations, Atom atom)
        {
            if (! oxygens_with_translations.Any(item => atom.PositionEquals(item)))
                oxygens_with_translations.Add(atom);
            else
                Console.WriteLine("Already exists");
                //System.Windows.Forms.MessageBox.Show("Already exists");
        }

        public static List<Atom> CalculatePolyhedra(List<Atom> atoms, double Yx, double Zy, double Xz)
        {
            List<Atom> oxygens_with_translations = new List<Atom>();
            var result = atoms.Where(a => a.Element[0] == 'O');
            List<Atom> oxygens = result.ToList();
            foreach(Atom ox in oxygens)
            {
                AddToList(oxygens_with_translations, ox);
                AddToList(oxygens_with_translations, new Atom(ox.Element, (ox.X + 1).ToString(), ox.Y.ToString(), ox.Z.ToString(), ox.Brush));
                AddToList(oxygens_with_translations, new Atom(ox.Element, (ox.X - 1).ToString(), ox.Y.ToString(), ox.Z.ToString(), ox.Brush));
                AddToList(oxygens_with_translations, new Atom(ox.Element, ox.X.ToString(), (ox.Y + 1).ToString(), ox.Z.ToString(), ox.Brush));
                AddToList(oxygens_with_translations, new Atom(ox.Element, ox.X.ToString(), (ox.Y - 1).ToString(), ox.Z.ToString(), ox.Brush));
                AddToList(oxygens_with_translations, new Atom(ox.Element, ox.X.ToString(), ox.Y.ToString(), (ox.Z + 1).ToString(), ox.Brush));
                AddToList(oxygens_with_translations, new Atom(ox.Element, ox.X.ToString(), ox.Y.ToString(), (ox.Z - 1).ToString(), ox.Brush));
            }

            List<Atom> allOxygens = new List<Atom>();

            foreach (Atom atom in oxygens_with_translations.OrderBy(a => a.X).OrderBy(a => a.Y).OrderBy(a => a.Z))
            {
                Console.WriteLine(atom.ToString());
            }

            foreach(Atom atom in atoms.Where(a => a.Element[0] != 'O'))
            {
                var res = oxygens_with_translations.Where(a => a.Element[0] == 'O').OrderBy(a => Distance(atom, a)).Take(4);
                if (check_point_in_tetrahydron(atom, res.ToList()))
                    allOxygens = allOxygens.Concat(res.ToList()).ToList();
                else
                {
                    string show = "";
                    show = res.ToList()[0].X + " " + res.ToList()[1].X + " " + res.ToList()[2].X + " " + res.ToList()[3].X + "\n" +
                        res.ToList()[0].Y + " " + res.ToList()[1].Y + " " + res.ToList()[2].Y + " " + res.ToList()[3].Y + "\n" +
                        res.ToList()[0].Z + " " + res.ToList()[1].Z + " " + res.ToList()[2].Z + " " + res.ToList()[3].Z + "\n";

                    //System.Windows.Forms.MessageBox.Show(show);
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

        static bool check_point_in_tetrahydron(Atom centerAtom, List<Atom> oxygens)
        {
            double center_x = (oxygens[0].X + oxygens[1].X + oxygens[2].X + oxygens[3].X) / 4;
            double center_y = (oxygens[0].Y + oxygens[1].Y + oxygens[2].Y + oxygens[3].Y) / 4;
            double center_z = (oxygens[0].Z + oxygens[1].Z + oxygens[2].Z + oxygens[3].Z) / 4;

            double a;
            double b;
            double c;
            double d;
            List<Atom> three_points = new List<Atom>();

            three_points.Add(oxygens[0]);
            three_points.Add(oxygens[1]);
            three_points.Add(oxygens[2]);

            if (!calculate_plane_equation(out a, out b, out c, out d, three_points))
                return false;

            //Console.WriteLine($"{a} * {centerAtom.X} + {b} * {centerAtom.Y} + {c} * {centerAtom.Z} + {d} = {a * centerAtom.X + b * centerAtom.Y + c * centerAtom.Z + d}");

            if ((a * centerAtom.X + b * centerAtom.Y + c * centerAtom.Z + d) * (a * center_x + b * center_y + c * center_z + d) <= 0)
                return false;

            
            three_points = new List<Atom>();

            three_points.Add(oxygens[0]);
            three_points.Add(oxygens[1]);
            three_points.Add(oxygens[3]);

            if (!calculate_plane_equation(out a, out b, out c, out d, three_points))
                return false;

            //Console.WriteLine($"{a} * {centerAtom.X} + {b} * {centerAtom.Y} + {c} * {centerAtom.Z} + {d} = {a * centerAtom.X + b * centerAtom.Y + c * centerAtom.Z + d}");


            if ((a * centerAtom.X + b * centerAtom.Y + c * centerAtom.Z + d) * (a * center_x + b * center_y + c * center_z + d) <= 0)
                return false;

            three_points = new List<Atom>();

            three_points.Add(oxygens[3]);
            three_points.Add(oxygens[1]);
            three_points.Add(oxygens[2]);

            if (!calculate_plane_equation(out a, out b, out c, out d, three_points))
                return false;

            //Console.WriteLine($"{a} * {centerAtom.X} + {b} * {centerAtom.Y} + {c} * {centerAtom.Z} + {d} = {a * centerAtom.X + b * centerAtom.Y + c * centerAtom.Z + d}");


            if ((a * centerAtom.X + b * centerAtom.Y + c * centerAtom.Z + d) * (a * center_x + b * center_y + c * center_z + d) <= 0)
                return false;

            three_points = new List<Atom>();

            three_points.Add(oxygens[0]);
            three_points.Add(oxygens[3]);
            three_points.Add(oxygens[2]);

            if (!calculate_plane_equation(out a, out b, out c, out d, three_points))
                return false;

            //Console.WriteLine($"{a} * {centerAtom.X} + {b} * {centerAtom.Y} + {c} * {centerAtom.Z} + {d} = {a * centerAtom.X + b * centerAtom.Y + c * centerAtom.Z + d}");

            if ((a * centerAtom.X + b * centerAtom.Y + c * centerAtom.Z + d) * (a * center_x + b * center_y + c * center_z + d) <= 0)
                return false;

            return true;
        }

        static bool calculate_plane_equation(out double a, out double b, out double c, out double d, List<Atom> three_points)
        {
            double x1 = three_points[0].X;
            double y1 = three_points[0].Y;
            double z1 = three_points[0].Z;
            double x2 = three_points[1].X;
            double y2 = three_points[1].Y;
            double z2 = three_points[1].Z;
            double x3 = three_points[2].X;
            double y3 = three_points[2].Y;
            double z3 = three_points[2].Z;

            a = 0;
            b = 0;
            c = 0;
            d = 0;

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
    }
}
