using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGroup
{
    public class SpaceGroupCl
    {
        string name;
        string[] expressions;
        public bool dummy = false;

        public SpaceGroupCl()
        {

        }

        public string Name { get => name; set => name = value; }
        public string[] Expressions { get => expressions; set => expressions = value; }

        public SpaceGroupCl(string name, string[] expressions)
        {
            this.name = name;
            foreach(string expression in expressions)
            {
                string[] split = expression.Split(',');
                expressions = new List<string>().Concat(expressions).Concat(split).ToArray();
            }


        }

        public static double Evaluate(string expression, double x, double y, double z)
        {
            char[] vars = new char[] { 'x', 'y', 'z' };
            string[] tmp = expression.Split(vars[0]);
            expression = String.Join(Convert.ToString(x), tmp);

            tmp = expression.Split(vars[1]);
            expression = String.Join(Convert.ToString(y), tmp);

            tmp = expression.Split(vars[2]);
            expression = String.Join(Convert.ToString(z), tmp);

            DataTable table = new DataTable();
            table.Columns.Add("expression", typeof(string), expression);
            DataRow row = table.NewRow();
            table.Rows.Add(row);
            return double.Parse((string)row["expression"]);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
