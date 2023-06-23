using Database_Project.Utilities;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Database_Project.Pages
{
    /// <summary>
    /// Interaction logic for AddOffert.xaml
    /// </summary>
    public partial class AddOffert : Page
    {
        private readonly IDatabase _database = new DatabaseImpl();

        public AddOffert()
        {
            InitializeComponent();

            List<string> conditionsList = new List<string>();
            conditionsList.AddRange(_database.GetConditions());
            cmbConditions.ItemsSource = conditionsList;
            cmbConditions.Items.Refresh();
            cmbConditions.SelectedIndex = 0;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _database.AddOffert(MainWindow.Username, float.Parse(txtPrice.Text), int.Parse(txtQuantity.Text), txtLanguage.Text,
                   txtxLocation.Text, cmbConditions.Text, int.Parse(txtProductId.Text));

                lblResult.Content = "Offert add to the database!";
            }
            catch(Exception ex)
            {
                lblResult.Content = ex.Message;
            }            
        }
    }
}
