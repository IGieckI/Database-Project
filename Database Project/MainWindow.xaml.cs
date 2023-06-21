using Database_Project.Pages;
using Database_Project.Utilities;
using System.Windows;
using System.Net;

namespace Database_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IDatabase _database = new DatabaseImpl();

        private string _username = "";
        private LoginType _login = LoginType.None;
        private enum LoginType
        {
            None,
            User,
            Seller,
            Admin
        }

        public MainWindow()
        {
            InitializeComponent();
            frmPages.Navigate(new Administration());
        }

        private void btnProducts_Click(object sender, RoutedEventArgs e)
        {
            frmPages.Navigate(new ProductList());
        }

        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {
            frmPages.Navigate(new Profile());
        }

        private void btnWishlist_Click(object sender, RoutedEventArgs e)
        {
            frmPages.Navigate(new Wishlist());
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            frmPages.Navigate(new Registration());
        }

        private void btnAdministration_Click(object sender, RoutedEventArgs e)
        {
            frmPages.Navigate(new Administration());
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if(rdbUser.IsChecked == true)
            {
                _login = _database.UserLogin(txtUsername.Text, new NetworkCredential(string.Empty, txtPassword.SecurePassword).Password) == true ?
                    LoginType.User : LoginType.None;
            }
            else if (rdbSeller.IsChecked == true)
            {
                _login = _database.SellerLogin(txtUsername.Text, new NetworkCredential(string.Empty, txtPassword.SecurePassword).Password) == true ?
                    LoginType.Seller : LoginType.None;
            }
            else if (rdbAdmin.IsChecked == true)
            {
                _login = _database.AdminLogin(txtUsername.Text, new NetworkCredential(string.Empty, txtPassword.SecurePassword).Password) == true ?
                    LoginType.Admin : LoginType.None;
            }

            switch (_login)
            {
                case LoginType.None:
                    lblResult.Content = "Credenziali errate!";
                    break;
                case LoginType.User:
                    loginSetup();
                    btnProfile.IsEnabled = true;
                    btnProfile.IsEnabled = true;
                    break;
                case LoginType.Seller:
                    loginSetup();
                    btnProfile.IsEnabled = true;
                    break;
                case LoginType.Admin:
                    loginSetup();
                    btnAdministration.IsEnabled = true;
                    break;
            }

            txtUsername.Clear();
            txtPassword.Clear();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            btnProfile.IsEnabled = false;
            btnWishlist.IsEnabled = false;
            btnAdministration.IsEnabled=false;
            txtUsername.IsEnabled = true;
            txtPassword.IsEnabled = true;

            _login = LoginType.None;

            btnLogin.Visibility = Visibility.Visible;
            btnLogout.Visibility = Visibility.Hidden;

            lblUsername.Visibility = Visibility.Visible;
            lblPassword.Visibility = Visibility.Visible;
            txtUsername.Visibility = Visibility.Visible;
            txtPassword.Visibility = Visibility.Visible;

            frmPages.Navigate(new ProductList());
        }

        private void loginSetup()
        {
            lblResult.Content = "";

            txtUsername.Clear();
            txtPassword.Clear();

            btnLogout.Visibility = Visibility.Visible;

            lblUsername.Visibility = Visibility.Hidden;
            lblPassword.Visibility = Visibility.Hidden;
            txtUsername.Visibility = Visibility.Hidden;
            txtPassword.Visibility = Visibility.Hidden;
        }
    }
}
