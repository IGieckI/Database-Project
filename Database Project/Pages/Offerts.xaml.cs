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
    /// Interaction logic for Offerts.xaml
    /// </summary>
    public partial class Offerts : Page
    {
        private readonly IDatabase _database = new DatabaseImpl();
        private List<Offert> _offertsList = new List<Offert>();

        public Offerts(Product product)
        {
            InitializeComponent();

            _offertsList = _database.GetOfferts(product.ProductId);
            grdOfferts.ItemsSource = _offertsList;
            grdOfferts.Items.Refresh();

            lblName.Content = product.Name;
            lblDate.Content = product.Date;
            lblRarity.Content = product.Rarity;
            lblGame.Content = product.Game;
            lblExpansion.Content = product.Expansion;
            lblDescription.Content = product.Description;
        }


    }
}
