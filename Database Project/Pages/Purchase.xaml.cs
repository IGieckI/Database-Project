using Database_Project.Entities;
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
        private List<ShoppingCartElement> _shoppingCartElements;
        public Purchase()
        {
            InitializeComponent();

            /*foreach(var detail in ShoppingCart)
            {
                Product product = Database.GetProducts(Database.GetOfferts(detail.OffertId));
                _shoppingCartElements.Add(new ShoppingCartElement(detail.));
            }*/

            grdShoppingCart.ItemsSource = ShoppingCart;
            grdShoppingCart.Items.Refresh();

            decimal totalPrice = ShoppingCart.Sum(p => p.Price * p.Quantity);

            lblTotale.Content = totalPrice.ToString();

            List<int> ratings = new List<int>();
            for(int i= 0; i<= 10; i++)
                ratings.Add(i);
            cmbRating.ItemsSource = ratings;
            cmbRating.Items.Refresh();
        }

        private void btnPurchase_Click(object sender, RoutedEventArgs e)
        {
            Feedback feedback = new Feedback(-1, int.Parse(cmbRating.Text), txtFeedback.Text, Username);

            try
            {
                Database.Buy(Username, feedback, ShoppingCart, Coupons);
                lblResponse.Content = "Acquisto effettuato!";
            }
            catch (Exception ex)
            {
                lblResponse.Content = ex.Message;
            }
        }

        private void btnCoupon_Click(object sender, RoutedEventArgs e)
        {
            Coupon? coupon = Database.GetCoupon(txtCoupon.Text);
            if (coupon is not null && Database.CheckCoupon(coupon.CouponCode))
            {
                Database.UseCoupon(Username, txtCoupon.Text);
                Coupons.Add(coupon);
                txtCoupon.Text = "";
                lblCoupon.Content = "Coupon used!";
            }
            else
            {
                lblCoupon.Content = "Coupon error!";
            }
        }
    }
}
