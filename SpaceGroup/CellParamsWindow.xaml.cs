using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
    /// Interaction logic for CellParamsWindow.xaml
    /// </summary>
    public partial class CellParamsWindow : Window
    {

        private SolidColorBrush atomColor;

        public ObservableCollection<Atom> addedAtomsList { get; set; }

        private ObservableCollection<Compound> compoundsList;

        private Compound selectedCompound;

        private Compound dummy;

        public CellParamsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //load or create compound list
            DeserializeTableList("Compounds.comp");

            addedAtomsList = new ObservableCollection<Atom>();
            DataGridAtoms.ItemsSource = addedAtomsList;

            combobox.ItemsSource = compoundsList;

            combobox.Items.Refresh();
        }

        private void addAtomButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string s = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(atomName.Text.ToLower());
                Atom atom = new Atom(s, xCoord.Text, yCoord.Text, zCoord.Text, atomColor);
                addedAtomsList.Add(atom);
                DataGridAtoms.Items.Refresh();
            }
            catch (NullReferenceException)
            {
                System.Windows.Forms.MessageBox.Show("Не выбран цвет!");
            }
            catch (IndexOutOfRangeException)
            {
                System.Windows.Forms.MessageBox.Show("Указаны не все параметры!");
                //MessageBox.Show("Не выбрана группа!");
            }

        }

        private void pickColor(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                atomColor = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
            }
        }

        private void SerializeTableList(string fileName)
        {
            List<Compound> cmpList = new List<Compound>();

            foreach (Compound compound in compoundsList)
            {
                cmpList.Add(compound);
            }

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Compound>));
            TextWriter writer = new StreamWriter(fileName);

            xmlSerializer.Serialize(writer, cmpList);
            writer.Close();
        }

        private void DeserializeTableList(string filename)
        {
            try
            {
                var mySerializer = new XmlSerializer(typeof(List<Compound>));
                var myFileStream = new FileStream(filename, FileMode.Open);
                List<Compound> myObject = (List<Compound>)mySerializer.Deserialize(myFileStream);
                compoundsList = new ObservableCollection<Compound>();

                foreach (Compound a in myObject)
                {
                    compoundsList.Add(a);
                }
                
            }
            catch (FileNotFoundException)
            {
                compoundsList = new ObservableCollection<Compound>();

                dummy = new Compound();
                dummy.dummy = true;
                dummy.Name = "Добавить соединение...";

                compoundsList.Add(dummy);

                SerializeTableList("Compounds.comp");
            }
        }

        private void deleteButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = DataGridAtoms.SelectedIndex;
                addedAtomsList.Remove(DataGridAtoms.SelectedItem as Atom);
                //MessageBox.Show((DataGridAtoms.SelectedItem as Atom).Element);
                DataGridAtoms.Items.Refresh();
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Не выбран элемент для удаления!");
            }

        }

        private void saveTableButton_Click(object sender, RoutedEventArgs e)
        {
            // КОСТЫЛЬ - TODO

            if ((string)saveTableButton.Content == "Изменить" && selectedCompound != null && !selectedCompound.dummy)
            {
                selectedCompound.Name = compoundNameTextBox.Text;
                selectedCompound.CrystalCell.setCellParams(Convert.ToDouble(aText.Text), Convert.ToDouble(bText.Text), Convert.ToDouble(cText.Text),
                    Convert.ToDouble(alphaText.Text), Convert.ToDouble(betaText.Text), Convert.ToDouble(gammaText.Text));

                selectedCompound.Atoms = new List<Atom>(addedAtomsList);
            }

            if ((string)saveTableButton.Content == "Добавить")
            {
                Compound compound = new Compound();

                compound.Name = compoundNameTextBox.Text;

                compound.CrystalCell.setCellParams(Convert.ToDouble(aText.Text), Convert.ToDouble(bText.Text), Convert.ToDouble(cText.Text),
                    Convert.ToDouble(alphaText.Text), Convert.ToDouble(betaText.Text), Convert.ToDouble(gammaText.Text));

                compound.Atoms = new List<Atom>(addedAtomsList);

                compoundsList.Remove(dummy);

                compoundsList.Add(compound);

                compoundsList.Add(dummy);
            }

            SerializeTableList("Compounds.comp");
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            selectedCompound = comboBox.SelectedItem as Compound;

            if (selectedCompound != null && selectedCompound.dummy)
            {

                saveTableButton.Content = "Добавить";

                compoundNameTextBox.Text = String.Empty;

                addedAtomsList = new ObservableCollection<Atom>(selectedCompound.Atoms);

                DataGridAtoms.ItemsSource = addedAtomsList;

                DataGridAtoms.Items.Refresh();

                aText.Text = "";
                bText.Text = "";
                cText.Text = "";

                alphaText.Text = "";
                betaText.Text = "";
                gammaText.Text = "";
            }

            else if (selectedCompound != null)
            {

                saveTableButton.Content = "Изменить";

                addedAtomsList = new ObservableCollection<Atom>(selectedCompound.Atoms);

                DataGridAtoms.ItemsSource = addedAtomsList;

                compoundNameTextBox.Text = selectedCompound.Name;

                DataGridAtoms.Items.Refresh();

                aText.Text = selectedCompound.CrystalCell.XAxisL.ToString();
                bText.Text = selectedCompound.CrystalCell.YAxisL.ToString();
                cText.Text = selectedCompound.CrystalCell.ZAxisL.ToString();

                alphaText.Text = selectedCompound.CrystalCell.Alpha.ToString();
                betaText.Text = selectedCompound.CrystalCell.Beta.ToString();
                gammaText.Text = selectedCompound.CrystalCell.Gamma.ToString();

                System.Windows.Forms.MessageBox.Show(selectedCompound.Atoms.Count().ToString());
            }


        }

        public void saveToNewFormat(Compound compound)
        {
            compoundsList.Add(compound);
        }

        private void selectCompoundButton_Click(object sender, RoutedEventArgs e)
        {
            selectedCompound.GetAtomKeyValuePairs();
            ((MainWindow)Owner).initCompound(selectedCompound);
            ((MainWindow)Owner).buildCompound();
            this.Close();
        }
    }
}
