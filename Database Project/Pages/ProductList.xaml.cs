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

namespace Database_Project.Pages
{
    /// <summary>
    /// Interaction logic for ProductList.xaml
    /// </summary>
    public partial class ProductList : Page
    {
        private readonly IDatabase _db = new DatabaseImpl();
        private List<Product> _productList = new List<Product>();

        public ProductList()
        {
            InitializeComponent();

            //initialize game's combobox
            var games = _db.GetGames();

            foreach (var game in games)
            {
                cmbGame.Items.Add(game);
            }
            cmbGame.Items.Add("");

            //initialize raritie's combobox
            var rarities = _db.GetRarities();

            foreach(var rarity in rarities)
            {
                cmbRarity.Items.Add(rarity);
            }
            cmbRarity.Items.Add("");

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            _productList.Clear();

            _productList.AddRange(_db.GetProducts(txtSearch.Text, cmbRarity.Text, cmbGame.Text));

            grdProducts.ItemsSource = _productList;

            grdProducts.Columns.Remove(grdProducts.Columns.FirstOrDefault(c => c.Header.ToString() == "ProductId"));
            grdProducts.Columns.Remove(grdProducts.Columns.FirstOrDefault(c => c.Header.ToString() == "Description"));

            grdProducts.Items.Refresh();
        }
    }
}
