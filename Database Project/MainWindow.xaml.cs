using Database_Project.Pages;
using Database_Project.Utilities;
using System.Windows;
using System.Net;
using System;

namespace Database_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IDatabase _database = new DatabaseImpl();

        public static string Username = "";
        public static LoginType Login = LoginType.None;
        public enum LoginType
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

        private void btnAddOfferts_Click(object sender, RoutedEventArgs e)
        {
            frmPages.Navigate(new AddOffert());
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
            try
            {
                if (rdbUser.IsChecked == true)
                {
                    Login = _database.UserLogin(txtUsername.Text, new NetworkCredential(string.Empty, txtPassword.SecurePassword).Password) == true ?
                        LoginType.User : LoginType.None;
                }
                else if (rdbSeller.IsChecked == true)
                {
                    Login = _database.SellerLogin(txtUsername.Text, new NetworkCredential(string.Empty, txtPassword.SecurePassword).Password) == true ?
                        LoginType.Seller : LoginType.None;
                }
                else if (rdbAdmin.IsChecked == true)
                {
                    Login = _database.AdminLogin(txtUsername.Text, new NetworkCredential(string.Empty, txtPassword.SecurePassword).Password) == true ?
                        LoginType.Admin : LoginType.None;
                }

                switch (Login)
                {
                    case LoginType.None:
                        lblResult.Content = "Credenziali errate!";
                        break;
                    case LoginType.User:
                        Username = txtUsername.Text;
                        loginSetup();
                        btnProfile.IsEnabled = true;
                        btnProfile.IsEnabled = true;
                        break;
                    case LoginType.Seller:
                        Username = txtUsername.Text;
                        loginSetup();
                        btnProfile.IsEnabled = true;
                        break;
                    case LoginType.Admin:
                        Username = txtUsername.Text;
                        loginSetup();
                        btnAdministration.IsEnabled = true;
                        break;
                }
            }
            catch(Exception ex)
            {
                lblResult.Content = ex.Message;
            }
            
            txtUsername.Clear();
            txtPassword.Clear();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            btnProfile.IsEnabled = false;
            btnAdministration.IsEnabled=false;
            txtUsername.IsEnabled = true;
            txtPassword.IsEnabled = true;

            Login = LoginType.None;

            btnLogin.Visibility = Visibility.Visible;
            btnLogout.Visibility = Visibility.Hidden;

            lblPassword.Visibility = Visibility.Visible;
            txtUsername.Visibility = Visibility.Visible;
            txtPassword.Visibility = Visibility.Visible;

            rdbUser.IsEnabled = true;
            rdbSeller.IsEnabled = true;
            rdbAdmin.IsEnabled = true;

            lblUsername.Content = "Username";

            frmPages.Navigate(new ProductList());
        }

        private void loginSetup()
        {
            lblResult.Content = "";

            txtUsername.Clear();
            txtPassword.Clear();

            btnLogout.Visibility = Visibility.Visible;

            lblPassword.Visibility = Visibility.Hidden;
            txtUsername.Visibility = Visibility.Hidden;
            txtPassword.Visibility = Visibility.Hidden;

            rdbUser.IsEnabled = false;
            rdbSeller.IsEnabled = false;
            rdbAdmin.IsEnabled = false;

            lblUsername.Content = $"Welcome {Username}";
        }
    }
}
