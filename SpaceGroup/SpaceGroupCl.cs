using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGroup
{
    class SpaceGroupCl
    {
        string name;
        string[] expressions;

        public string Name { get => name; set => name = value; }
        public string[] Expressions { get => expressions; set => expressions = value; }

        public SpaceGroupCl(string name, string[] expressions)
        {
            this.name = name;
            this.expressions = expressions;
        }

        public static double Evaluate(string expression)
        {
            DataTable table = new DataTable();
            table.Columns.Add("expression", typeof(string), expression);
            DataRow row = table.NewRow();
            table.Rows.Add(row);
            return double.Parse((string)row["expression"]);
        }
    }
}
