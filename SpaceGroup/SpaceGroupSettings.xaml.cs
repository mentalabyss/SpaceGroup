using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        private List<SpaceGroupCl> spaceGroupGroup;
        public List<SpaceGroupCl> SpaceGroupGroup
        {
            get
            {
                return spaceGroupGroup;
            }

            set
            {
                spaceGroupGroup = value;
            }
        }

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
            spaceGroupGroup.RemoveAll(item => item.dummy);
            spaceGroupGroup.Add(new SpaceGroupCl { Name = "Добавить группу...", dummy = true });
        }

        private void deleteButtonClick(object sender, RoutedEventArgs e)
        {
            var expr = expressionsGrid.SelectedItem as Expr;
            Expressions.Remove(expr);
        }

        private void addGroupButtonClicked(object sender, RoutedEventArgs e)
        {
            SpaceGroupCl spaceGroupCl = new SpaceGroupCl(sgName.Text, Expressions);
            spaceGroupGroup.RemoveAll(x => x.Name == sgName.Text);
            spaceGroupGroup.Add(spaceGroupCl);
            SerializeSpaceGroupList();
            readdDummy();
            combobox.ItemsSource = null;
            combobox.ItemsSource = spaceGroupGroup;
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
                sgName.Text = String.Empty;
                Console.WriteLine("BEFORE: ", Expressions.Count);

                for (int i = 0; i < Expressions.Count; i++)
                {
                    Expressions.Remove(Expressions[i]);
                }

                Console.WriteLine("AFTER", Expressions.Count);
                expressionsGrid.ItemsSource = Expressions;
                //Creating the new item
                addButton.Content = "Добавить новую группу";
                //Adding to the datasource

                //Removing and adding the dummy item from the collection, thus it is always the last on the 'list'
                spaceGroupGroup.Remove(selectedItem);
                spaceGroupGroup.Add(selectedItem);
                comboBox.ItemsSource = spaceGroupGroup;
                //Select the new item
            }
            else
            {
                addButton.Content = "Применить";
                try
                {
                    sgName.Text = combobox.SelectedItem.ToString();
                    var selectedGroup = (SpaceGroupCl)combobox.SelectedItem;
                    Expressions = new ObservableCollection<Expr>();
                    Expressions = selectedGroup.exprs;
                    expressionsGrid.ItemsSource = null;
                    expressionsGrid.ItemsSource = Expressions;
                }
                catch(NullReferenceException nullref)
                {
                    Console.WriteLine(nullref.Message);
                }
                
                //foreach(string s in selectedGroup.Expressions)
                //{
                //    Expr expr = new Expr { Text = s };
                //    Expressions.Add(expr);
                //}
                
                readdDummy();
            }
        }

        private void readdDummy()
        {
            spaceGroupGroup.RemoveAll(item => item.dummy);
            //spaceGroupGroup.Add()
            spaceGroupGroup.Add(new SpaceGroupCl { Name = "Добавить новую группу...", dummy = true });
        }

        private void selectButton_click(object sender, RoutedEventArgs e)
        {
            var selectedGroup = (SpaceGroupCl)combobox.SelectedItem;
            ((MainWindow)this.Owner).selectGroup(selectedGroup);
        }
    }
}
