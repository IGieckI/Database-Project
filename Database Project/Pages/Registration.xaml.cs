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
using Database_Project.Entities;
using System.Net;

namespace Database_Project.Pages
{
    /// <summary>
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : Page
    {
        private readonly IDatabase _database = new DatabaseImpl();

        public Registration()
        {
            InitializeComponent();
        }

        private void rdbUser_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void rdbSeller_Checked(object sender, RoutedEventArgs e)
        {
            lblCountry.Content = "Country*:";
            lblIban.Content = "IBAN*:";
            lblBankName.Content = "Bank name*:";
            lblBicswift.Content = "BIC/SWIFT*:";
        }
        private void rdbSeller_Unchecked(object sender, RoutedEventArgs e)
        {
            lblCountry.Content = "Country:";
            lblIban.Content = "IBAN:";
            lblBankName.Content = "Bank name:";
            lblBicswift.Content = "BIC/SWIFT:";
        }

        private void rdbAdmin_Checked(object sender, RoutedEventArgs e)
        {
            txtStreet.IsEnabled = false;
            txtCivicNumber.IsEnabled = false;
            txtCity.IsEnabled = false;
            txtCountry.IsEnabled = false;
            txtTelephoneNumber.IsEnabled = false;
            txtCap.IsEnabled = false;
            txtIban.IsEnabled = false;
            txtBank.IsEnabled = false;
            txtBicswift.IsEnabled = false;
        }
        private void rdbAdmin_Unchecked(object sender, RoutedEventArgs e)
        {
            txtStreet.IsEnabled = true;
            txtCivicNumber.IsEnabled = true;
            txtCity.IsEnabled = true;
            txtCountry.IsEnabled = true;
            txtTelephoneNumber.IsEnabled = true;
            txtCap.IsEnabled = true;
            txtIban.IsEnabled = true;
            txtBank.IsEnabled = true;
            txtBicswift.IsEnabled = true;
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            bool registrationResult = false;
            try
            {
                if (rdbUser.IsChecked == true)
                {
                    if (txtUsername.Text == "" || txtPassword.SecurePassword.Length == 0 || txtEmail.Text == "")
                    {
                        lblResponse.Content = "Missing important fields!";
                        return;
                    }
                    BankAccount? bankAccount = txtIban.Text == "" || txtBank.Text == "" || txtBicswift.Text == "" ? null : BuildBankAccount();
                    registrationResult = _database.UserRegistration(BuildAccount(), bankAccount);
                }
                else if (rdbSeller.IsChecked == true)
                {
                    if (txtUsername.Text == "" || txtPassword.SecurePassword.Length == 0 || txtEmail.Text == "" || txtCountry.Text == "" || txtIban.Text == "" || txtBank.Text == "" || txtBicswift.Text == "")
                    {
                        lblResponse.Content = "Missing important fields!";
                        return;
                    }

                    registrationResult = _database.SellerRegistration(BuildAccount(), BuildBankAccount());
                }
                else if (rdbAdmin.IsChecked == true)
                {
                    if (txtUsername.Text == "" || txtPassword.SecurePassword.Length == 0 || txtEmail.Text == "")
                    {
                        lblResponse.Content = "Missing important fields!";
                        return;
                    }

                    registrationResult = _database.AdminRegistration(BuildAccount());
                }

                lblResponse.Content = registrationResult ? "Registration completed!" : "Registration error, try choose another Username or check if (*) fields are filled.";

                if (registrationResult)
                {
                    txtUsername.Clear();
                    txtPassword.Clear();
                    txtEmail.Clear();
                    txtStreet.Clear();
                    txtCivicNumber.Clear();
                    txtCap.Clear();
                    txtCity.Clear();
                    txtCountry.Clear();
                    txtTelephoneNumber.Clear();
                    txtIban.Clear();
                    txtBank.Clear();
                    txtBicswift.Clear();
                }
            }
            catch (Exception ex)
            {
                lblResponse.Content = ex.Message;
            }            
        }

        /// <summary>
        /// Build an account object from the form class
        /// </summary>
        /// <returns>An Account object with new user's informations</returns>
        private Account BuildAccount()
        {
            string username = txtUsername.Text;
            string email = txtEmail.Text;
            string password = new NetworkCredential(string.Empty, txtPassword.SecurePassword).Password;
            string? street = txtStreet.Text;
            int? civicNumber = txtCivicNumber.Text == "" ? null : Int32.Parse(txtCivicNumber.Text);
            int? cap = txtCap.Text == "" ? null : Int32.Parse(txtCap.Text);
            string? city = txtCity.Text;
            string? country = txtCountry.Text;
            string telephoneNumber = txtTelephoneNumber.Text;

            Account account = new Account(username, email, password, street, civicNumber, cap, city, country, telephoneNumber, 0, null);

            return account;
        }

        /// <summary>
        /// Build a bank account object from the form class
        /// </summary>
        /// <returns>A BankAccount object with new user's informations</returns>
        private BankAccount BuildBankAccount()
        {
            string IBAN = txtIban.Text;
            string BankName = txtBank.Text;
            string BIC_SWIFT = txtBicswift.Text;

            BankAccount bankAccount = new BankAccount(-1, IBAN, BankName, BIC_SWIFT);

            return bankAccount;
        }
    }
}
