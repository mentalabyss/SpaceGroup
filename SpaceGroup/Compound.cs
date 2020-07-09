using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGroup
{
    public class Compound
    {
        private string name;
        private List<Atom> atoms;
        private CrystalCell crystalCell;
        public bool dummy = false;

        public Compound()
        {
            atoms = new List<Atom>();
            crystalCell = new CrystalCell();
        }

        public List<Atom> Atoms { get => atoms; set => atoms = value; }
        public CrystalCell CrystalCell { get => crystalCell; set => crystalCell = value; }
        public string Name { get => name; set => name = value; }

        public override string ToString()
        {
            return Name;
        }
    }
}
