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
    /// Interaction logic for Profile.xaml
    /// </summary>
    public partial class Profile : Page
    {
        private List<WishlistItem> _wishlist = new List<WishlistItem>();
        private Account _account;
        private BankAccount _bankAccount;

        public Profile()
        {
            InitializeComponent();

            _account = new Account("", "", "", "", null, null, "", "", "", 0, null);

            try
            {
                _account = Database.GetUserAccount(Username);
                _wishlist = Database.GetWishlist(Username);

                if ( _account.BankAccountID != null )
                {
                    _bankAccount = Database.GetBankAccount(_account.BankAccountID.Value);

                    txtIBAN.Text = _bankAccount.IBAN;
                    txtBankName.Text = _bankAccount.BankName;
                    txtBicswift.Text = _bankAccount.BIC_SWIFT;
                }

            }
            catch (Exception ex)
            {
                lblResponse.Content = ex.Message;
            }

            lblUsername.Content = _account.Username;
            lblCredit.Content = _account.Credit;
            txtEmail.Text = _account.Email;
            txtStreet.Text = _account.Street;
            txtCivicNumber.Text = _account.CivicNumber.ToString();
            txtCap.Text = _account.Cap.ToString();
            txtCity.Text = _account.City;
            txtCountry.Text = _account.Country;
            txtTelephoneNumber.Text = _account.TelephoneNumber.ToString();

            grdWishlist.ItemsSource = _wishlist;
            grdWishlist.Items.Refresh();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = txtEmail.Text;
                string? street = txtStreet.Text;
                int? civicNumber = txtCivicNumber.Text == "" ? null : Int32.Parse(txtCivicNumber.Text);
                int? cap = txtCap.Text == "" ? null : Int32.Parse(txtCap.Text);
                string? city = txtCity.Text;
                string? country = txtCountry.Text;
                string telephoneNumber = txtTelephoneNumber.Text;

                Database.EditUserProfile(new Account(_account.Username, email, _account.Password, street, civicNumber, cap, city, country, telephoneNumber, _account.Credit, _account.BankAccountID));

                if (txtIBAN.Text + txtBankName.Text + txtBicswift.Text != "")
                {
                    if (_account.BankAccountID is null)
                    {
                        Database.AddUserBankAccount(_account.Username, new BankAccount(-1, txtIBAN.Text, txtBankName.Text, txtBicswift.Text));
                    }
                    else
                    {
                        Database.UpdateUserBankAccount(new BankAccount(_account.BankAccountID.Value, txtIBAN.Text, txtBankName.Text, txtBicswift.Text));
                    }                    
                }
                else if (_account.BankAccountID is not null)
                {
                    Database.RemoveUserBankAccount(_account);
                }

                _account = Database.GetUserAccount(_account.Username);

                lblResponse.Content = "Profile updated successfully!";
            }
            catch (Exception ex)
            {
                lblResponse.Content = ex.Message;
            }
        }
    }
}
