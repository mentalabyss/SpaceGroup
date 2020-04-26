using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
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

        public ObservableCollection<Expr> exprs;

        public SpaceGroupCl()
        {

        }

        public string Name { get => name; set => name = value; }
        public string[] Expressions { get => expressions; set => expressions = value; }

        public SpaceGroupCl(string name, ObservableCollection<Expr> obsColExpr)
        {
            this.name = name;
            exprs = obsColExpr;
            expressions = new string[0];
            //for (int i = 0; i < obsColExpr.Count; i++)
            //{
            //    expressions[i] = obsColExpr[i].Text;
            //}

            foreach (Expr expr in obsColExpr)
            {
                string[] split = expr.Text.Split(',');
                expressions = new List<string>().Concat(expressions).Concat(split).ToArray();
            }
        }

        public static double Evaluate(string expression, double x, double y, double z)
        {
            char[] vars = new char[] { 'x', 'y', 'z' };
            string[] tmp = expression.Split(vars[0]);

            try
            {
                CultureInfo culture = new CultureInfo("en-US");

                expression = String.Join(Convert.ToString(x, culture), tmp);

                tmp = expression.Split(vars[1]);
                expression = String.Join(Convert.ToString(y, culture), tmp);

                tmp = expression.Split(vars[2]);
                expression = String.Join(Convert.ToString(z, culture), tmp);

                DataTable table = new DataTable();
                table.Columns.Add("expression", typeof(string), expression);
                DataRow row = table.NewRow();
                table.Rows.Add(row);
                return double.Parse((string)row["expression"], culture);
            }
            catch(Exception)
            {
                System.Windows.Forms.MessageBox.Show("Test");

                CultureInfo culture = new CultureInfo("ru-RU");

                expression = String.Join(Convert.ToString(x, culture), tmp);

                tmp = expression.Split(vars[1]);
                expression = String.Join(Convert.ToString(y, culture), tmp);

                tmp = expression.Split(vars[2]);
                expression = String.Join(Convert.ToString(z, culture), tmp);
                DataTable table = new DataTable();
                table.Columns.Add("expression", typeof(string), expression);
                DataRow row = table.NewRow();
                table.Rows.Add(row);
                return double.Parse((string)row["expression"], culture);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
