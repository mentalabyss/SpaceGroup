using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Xml.Serialization;

namespace SpaceGroup
{
    /// <summary>
    /// Interaction logic for SpaceGroupSettings.xaml
    /// </summary>
    /// 
    public partial class SpaceGroupSettings : Window
    {
        public int a;

        public List<SpaceGroupCl> spaceGroupGroup;

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
            try
            {
                spaceGroupGroup = DeserializeSpaceGroupList();
            }
            catch(FileNotFoundException)
            {
                spaceGroupGroup = new List<SpaceGroupCl>();
            }
            catch(Exception)
            {
                spaceGroupGroup = new List<SpaceGroupCl>();
            }

            combobox.ItemsSource = spaceGroupGroup;
            spaceGroupGroup.Add(new SpaceGroupCl { Name = "Добавить группу...", dummy = true });
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

            spaceGroupGroup.Add(spaceGroupCl);
            SerializeSpaceGroupList();
        }

        private void SerializeSpaceGroupList()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<SpaceGroupCl>));
            TextWriter writer = new StreamWriter("spacegroups.xml");
            xmlSerializer.Serialize(writer, spaceGroupGroup);
            writer.Close();
        }

        private List<SpaceGroupCl> DeserializeSpaceGroupList()
        {
            var mySerializer = new XmlSerializer(typeof(List<SpaceGroupCl>));
            var myFileStream = new FileStream("spacegroups.xml", FileMode.Open);
            var myObject = (List<SpaceGroupCl>)mySerializer.Deserialize(myFileStream);
            return myObject;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            var selectedItem = comboBox.SelectedItem as SpaceGroupCl;
            if (selectedItem != null && selectedItem.dummy)
            {
                //Creating the new item

                //Adding to the datasource

                //Removing and adding the dummy item from the collection, thus it is always the last on the 'list'
                spaceGroupGroup.Remove(selectedItem);
                spaceGroupGroup.Add(selectedItem);

                //Select the new item
                
            }
        }
    }
}
