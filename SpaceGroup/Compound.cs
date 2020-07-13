using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SpaceGroup
{
    public class Compound
    {
        private string name;
        private List<Atom> atoms;
        private CrystalCell crystalCell;
        [XmlIgnore]
        public Dictionary<string, int> atomTypesDict = new Dictionary<string, int>();
        public bool dummy;

        public Compound()
        {
            name = "";
            atoms = new List<Atom>();
            crystalCell = new CrystalCell();
            dummy = false;
        }
        public List<Atom> Atoms { get => atoms; set => atoms = value; }
        public CrystalCell CrystalCell { get => crystalCell; set => crystalCell = value; }
        public string Name { get => name; set => name = value; }

        public void GetAtomKeyValuePairs()
        {
            //adding oxygen pair
            atomTypesDict.Add("O", 0);

            //adding all the other existing elements

            foreach (Atom atom in Atoms)
            {
                string element = Regex.Replace(atom.Element, @"[\d-]", string.Empty);

                if (element != "O")
                {
                    if (atomTypesDict.ContainsKey(element))
                        atom.TypeID = atomTypesDict[element];
                    else
                    {
                        atom.TypeID = atomTypesDict.Count;
                        atomTypesDict.Add(element, atomTypesDict.Count);
                    }
                }
            }
        }


        public override string ToString()
        {
            return Name;
        }
    }
}
