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
    /// Interaction logic for Profile.xaml
    /// </summary>
    public partial class Profile : Page
    {
        private List<WishlistItem> _wishlist = new List<WishlistItem>();
        private Account _account;
        private readonly IDatabase _database = new DatabaseImpl();

        public Profile()
        {
            InitializeComponent();

            _account = _database.GetUserAccount(MainWindow.Username);

            lblUsername.Content = _account.Username;
            lblCredit.Content = _account.Credit;
            txtEmail.Text = _account.Email;
            txtStreet.Text = _account.Street;
            txtCivicNumber.Text = _account.CivicNumber.ToString();
            txtCap.Text = _account.Cap.ToString();
            txtCity.Text = _account.City;
            txtCountry.Text = _account.Country;
            txtTelephoneNumber.Text = _account.TelephoneNumber.ToString();

            _wishlist = _database.GetWishlist(MainWindow.Username);
            grdWishlist.ItemsSource = _wishlist;
            grdWishlist.Items.Refresh();
        }
    }
}
