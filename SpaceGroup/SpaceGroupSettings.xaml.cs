using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;
using ComboBox = System.Windows.Controls.ComboBox;
using MessageBox = System.Windows.Forms.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

namespace SpaceGroup
{
    /// <summary>
    /// Interaction logic for SpaceGroupSettings.xaml
    /// </summary>
    /// 
    public partial class SpaceGroupSettings : Window
    {
        private List<SpaceGroupCl> _spaceGroupGroup;
        private bool _newElement = false;
        private SpaceGroupCl _currentGroup;
        public List<SpaceGroupCl> SpaceGroupGroup
        {
            get => _spaceGroupGroup;

            set => _spaceGroupGroup = value;
        }

        public ObservableCollection<Expr> Expressions
        {
            get; set;
        }

        public SpaceGroupSettings(SpaceGroupCl loadedSpaceGroupCl)
        {
            InitializeComponent();
            if (loadedSpaceGroupCl != null)
                _currentGroup = loadedSpaceGroupCl;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ExpressionsGrid.CellEditEnding += RowAdded;
            Expressions = new ObservableCollection<Expr>();
            ExpressionsGrid.ItemsSource = Expressions;
            try
            {
                _spaceGroupGroup = DeserializeSpaceGroupList();
            }
            catch(FileNotFoundException)
            {
                _spaceGroupGroup = new List<SpaceGroupCl>();
            }
            catch(Exception)
            {
                _spaceGroupGroup = new List<SpaceGroupCl>();
            }

            if (_currentGroup != null)
            {
                var currentIndex = _spaceGroupGroup.FindIndex(x => x.Name == _currentGroup.Name);
                combobox.SelectedIndex = currentIndex;
            }

            combobox.ItemsSource = _spaceGroupGroup;
            _spaceGroupGroup.RemoveAll(item => item.dummy);
            _spaceGroupGroup.Add(new SpaceGroupCl { Name = "Добавить группу...", dummy = true });
        }

        private void SerializeSpaceGroupList()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<SpaceGroupCl>));
            TextWriter writer = new StreamWriter("spacegroups.xml");
            xmlSerializer.Serialize(writer, _spaceGroupGroup);
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
            var selectedItem = (SpaceGroupCl)comboBox.SelectedItem;

            if (selectedItem == null) return;

            _currentGroup = selectedItem;
            if (selectedItem.dummy)
            {
                SgName.Text = String.Empty;
                Console.WriteLine("BEFORE: ", Expressions.Count);

                for (int i = 0; i < Expressions.Count; i++)
                {
                    Expressions.Remove(Expressions[i]);
                }

                Console.WriteLine("AFTER", Expressions.Count);
                ExpressionsGrid.ItemsSource = Expressions;
                //Creating the new item
                addButton.Content = "Добавить новую группу";
                //Adding to the datasource

                //Removing and adding the dummy item from the collection, thus it is always the last on the 'list'
                _spaceGroupGroup.Remove(selectedItem);
                _spaceGroupGroup.Add(selectedItem);
                combobox.ItemsSource = _spaceGroupGroup;
                //Select the new item
            }
            else
            {
                addButton.Content = "Применить";
                try
                {
                    SgName.Text = comboBox.SelectedItem.ToString();
                    var selectedGroup = (SpaceGroupCl) combobox.SelectedItem;
                    Expressions = new ObservableCollection<Expr>();
                    Expressions = selectedGroup.exprs;
                    ExpressionsGrid.ItemsSource = null;
                    ExpressionsGrid.ItemsSource = Expressions;
                }
                catch (ArgumentException argumentException)
                {
                    Console.WriteLine(argumentException.Message);
                }
                catch (NullReferenceException nullRef)
                {
                    Console.WriteLine(nullRef.Message);
                }

                AddDummy();
            }
        }

        private void AddDummy()
        {
            _spaceGroupGroup.RemoveAll(item => item.dummy);
            //spaceGroupGroup.Add()
            _spaceGroupGroup.Add(new SpaceGroupCl { Name = "Добавить новую группу...", dummy = true });
        }

        private void SelectButtonClick(object sender, RoutedEventArgs e)
        {
            _currentGroup = (SpaceGroupCl)combobox.SelectedItem;
            ((MainWindow)this.Owner).SelectGroup(_currentGroup);
            this.Close();
        }

        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            var expr = ExpressionsGrid.SelectedItem as Expr;
            Expressions.Remove(expr);
        }

        private void AddGroupButtonClicked(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Сохранить изменения?", "Сохранить", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) ==
                System.Windows.Forms.DialogResult.No)
                return;

            SpaceGroupCl spaceGroupCl = new SpaceGroupCl(SgName.Text, Expressions);
            _spaceGroupGroup.RemoveAll(x => x.Name == SgName.Text);
            _spaceGroupGroup.Add(spaceGroupCl);
            SerializeSpaceGroupList();
            AddDummy();
            combobox.ItemsSource = null;
            combobox.ItemsSource = _spaceGroupGroup;
            combobox.SelectedItem = spaceGroupCl;
        }

        private void NewElementAdded(object sender, EventArgs e)
        {
            _newElement = true;
        }

        private void RowAdded(object sender, DataGridCellEditEndingEventArgs e)
        {
            var textBox = e.EditingElement as TextBox;
            try
            {
                var expres = textBox.Text.Split(',');
                foreach (string f in expres)
                    SpaceGroupCl.Evaluate(f, 1, 1, 1);
            }
            catch (Exception ex)
            {
                if (ex is SyntaxErrorException || ex is EvaluateException)
                {
                    MessageBox.Show("Ошибка в записи симметрии!");
                    Expressions.RemoveAt(e.Row.GetIndex());
                    ExpressionsGrid.ItemsSource = null;
                    ExpressionsGrid.ItemsSource = Expressions;
                }
                else
                {
                    MessageBox.Show("Возможная ошибка в записи симметрии!");
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
