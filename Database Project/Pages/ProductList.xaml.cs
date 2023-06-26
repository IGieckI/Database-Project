using Database_Project.Entities;
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

using static Database_Project.Entities.Session;

namespace Database_Project.Pages
{
    /// <summary>
    /// Interaction logic for ProductList.xaml
    /// </summary>
    public partial class ProductList : Page
    {
        private List<Product> _productList = new List<Product>();

        public ProductList()
        {
            InitializeComponent();

            //initialize game's combobox
            var games = Database.GetGames();

            foreach (var game in games)
            {
                cmbGame.Items.Add(game);
            }
            cmbGame.Items.Add("");

            //initialize raritie's combobox
            var rarities = Database.GetRarities();

            foreach(var rarity in rarities)
            {
                cmbRarity.Items.Add(rarity);
            }
            cmbRarity.Items.Add("");

            if (Login != LoginType.User)
            {
                btnCart.IsEnabled = false;
                btnWishlist.IsEnabled = false;
            }

            RefreshGrid();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            RefreshGrid();
        }

        private void btnWishlist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Login == LoginType.User && grdProducts.SelectedIndex != -1)
                {
                    Database.AddToWishlist(Username, _productList[grdProducts.SelectedIndex].ProductId, 1);
                    lblWishlistResult.Content = "Item added to your wishlist!";
                }
            }
            catch (Exception ex)
            {
                lblWishlistResult.Content = ex.Message;
            }
        }

        private void btnSeeOfferts_Click(object sender, RoutedEventArgs e)
        {
            if (grdProducts.SelectedIndex != -1)
            {
                NavigationService ns = NavigationService.GetNavigationService(this);
                ns.Navigate(new Offerts(_productList[grdProducts.SelectedIndex]));
            }            
        }

        private void btnCart_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Purchase());
        }

        private void RefreshGrid()
        {
            _productList.Clear();

            _productList.AddRange(Database.GetProducts(txtSearch.Text, cmbRarity.Text, cmbGame.Text));

            grdProducts.ItemsSource = _productList;

            grdProducts.Columns.Remove(grdProducts.Columns.FirstOrDefault(c => c.Header.ToString() == "ProductId"));
            grdProducts.Columns.Remove(grdProducts.Columns.FirstOrDefault(c => c.Header.ToString() == "Description"));

            lblWishlistResult.Content = "";
            grdProducts.Items.Refresh();
        }
    }
}
