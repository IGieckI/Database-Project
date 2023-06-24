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
    /// Interaction logic for Purchase.xaml
    /// </summary>
    public partial class Purchase : Page
    {
        public Purchase()
        {
            InitializeComponent();

            grdShoppingCart.ItemsSource = ShoppingCart;
            grdShoppingCart.Items.Refresh();

            float totalPrice = ShoppingCart.Sum(p => p.Price * p.Quantity);

            lblTotale.Content = totalPrice.ToString();

        }
    }
}
