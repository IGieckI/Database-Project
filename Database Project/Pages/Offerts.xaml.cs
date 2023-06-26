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
    /// Interaction logic for Offerts.xaml
    /// </summary>
    public partial class Offerts : Page
    {
        private List<Offert> _offertsList = new List<Offert>();
        private Product _product;

        public Offerts(Product product)
        {
            _product = product;

            InitializeComponent();

            RefreshGrid();

            lblName.Content = product.Name;
            lblDate.Content = product.Date;
            lblRarity.Content = product.Rarity;
            lblGame.Content = product.Game;
            lblExpansion.Content = product.Expansion;
            lblDescription.Content = product.Description;

            if (Login != LoginType.User)
            {
                btnAdd.IsEnabled = false;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (grdOfferts.SelectedIndex == -1)
            {
                lblResponse.Content = "No elements selected!";
                return;
            }

            Offert offert = (Offert)(grdOfferts.SelectedItem);

            for (int i=0; i<ShoppingCart.Count; i++)
            {
                if (ShoppingCart[i].OffertId == offert.OffertId)
                {
                    ShoppingCart[i] = new Detail(ShoppingCart[i].DetailId, ShoppingCart[i].Price, ShoppingCart[i].Quantity+1, ShoppingCart[i].OffertId, ShoppingCart[i].SellId);
                    break;
                }
            }

            ShoppingCart.Add(new Detail(-1, offert.Price, 1, offert.OffertId, -1));

            Database.AddToCart(offert.OffertId);
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            _offertsList = Database.GetOfferts(_product.ProductId);
            grdOfferts.ItemsSource = _offertsList;
            grdOfferts.Items.Refresh();
        }
    }
}
