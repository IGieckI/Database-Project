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
    /// Interaction logic for Administration.xaml
    /// </summary>
    public partial class Administration : Page
    {
        private readonly IDatabase _database = new DatabaseImpl();

        public Administration()
        {
            InitializeComponent();
            Reset();
        }

        //Reset the page and refresh datas from database
        private void Reset()
        {
            txtGame.Text = "";
            txtRarity.Text = "";
            txtExpansion.Text = "";
            txtCondition.Text = "";
            txtProductName.Text = "";

            List<string> rarityList = new List<string>();
            List<string> gameList = new List<string>();
            List<string> expansionList = new List<string>();

            rarityList.AddRange(_database.GetRarities());
            gameList.AddRange(_database.GetGames());
            expansionList.AddRange(_database.GetExpansions());

            cmbRarity.ItemsSource = rarityList;
            cmbGame.ItemsSource = gameList;
            cmbExpansion.ItemsSource = expansionList;

            cmbRarity.Items.Refresh();
            cmbGame.Items.Refresh();
            cmbExpansion.Items.Refresh();

            cmbRarity.SelectedIndex = 0;
            cmbGame.SelectedIndex = 0;
            cmbExpansion.SelectedIndex = 0;
        }

        private void btnAddGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _database.AddGame(txtGame.Text);
                lblGame.Content = "New game added to the database!";
            }
            catch (Exception ex)
            {
                lblGame.Content = ex.Message;
            }

            Reset();
        }

        private void btnRarity_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _database.AddRarity(txtRarity.Text);
                lblRarity.Content = "New rarity added to the database!";
            }
            catch (Exception ex)
            {
                lblRarity.Content = ex.Message;
            }

            Reset();
        }

        private void btnExpansion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _database.AddExpansion(txtExpansion.Text);
                lblExpansion.Content = "New expansion added to the database!";
            }
            catch (Exception ex)
            {
                lblExpansion.Content = ex.Message;
            }

            Reset();
        }

        private void btnCondition_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _database.AddCondition(txtCondition.Text);
                lblCondition.Content = "New condition added to the database!";
            }
            catch (Exception ex)
            {
                lblCondition.Content = ex.Message;
            }

            Reset();
        }

        private void btnProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _database.AddProduct(txtProductName.Text, txtDescription.Text, cmbRarity.Text, cmbGame.Text, cmbExpansion.Text);
                lblProduct.Content = "New Product added to the database!";
            }
            catch(Exception ex)
            {
                lblProduct.Content = ex.Message;
            }

            Reset();
        }
    }
}
