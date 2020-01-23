using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SpaceGroup
{
    /// <summary>
    /// Interaction logic for SpaceGroupSettings.xaml
    /// </summary>
    /// 
    public partial class SpaceGroupSettings : Window
    {
        public int a;

        public ObservableCollection<Expr> Expressions
        {
            get; set;
        }

        public SpaceGroupSettings()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Expressions = new ObservableCollection<Expr>();
            expressionsGrid.ItemsSource = Expressions;
        }

        public class Expr
        {
            public string Text { get; set; }
        }

        private void deleteButtonClick(object sender, RoutedEventArgs e)
        {
            var expr = expressionsGrid.SelectedItem as Expr;
            Expressions.Remove(expr);
        }

        private void addGroupButtonClicked(object sender, RoutedEventArgs e)
        {
            string[] stringExpressions = new string[Expressions.Count];
            for(int i = 0; i < Expressions.Count; i++)
            {
                stringExpressions[i] = Expressions[i].Text;
            }

            SpaceGroupCl spaceGroupCl = new SpaceGroupCl(sgName.Text, stringExpressions);
        }
    }
}
