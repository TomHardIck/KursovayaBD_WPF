using DesktopApp.PraktikaShimbirevDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Windows.Shapes;

namespace DesktopApp
{
    /// <summary>
    /// Логика взаимодействия для PaymentWindow.xaml
    /// </summary>
    public partial class PaymentWindow : Window
    {
        OrderTableAdapter orders = new OrderTableAdapter();
        ProductTableAdapter products = new ProductTableAdapter();
        public PaymentWindow()
        {
            InitializeComponent();
        }

        private void payment_Click(object sender, RoutedEventArgs e)
        {
            if (numCard.Text != null && date.Text != null && cvc.Text != null)
            {
                Random random = new Random();
                var num = random.Next(1000000, 9999999).ToString();
                SqlConnection connection = orders.Connection;
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                SqlCommand command = connection.CreateCommand();
                command.Transaction = transaction;
                try
                {
                    command.CommandText = $"insert into [Order] values ('{num}', '{DateTime.Now}', {MainWindow.loggedId}, 1, {CustomerWindow.cartSum}); SELECT SCOPE_IDENTITY()";
                    int idOrder = Convert.ToInt32(command.ExecuteScalar());
                    foreach (var item in CustomerWindow.cart)
                    {
                        var productId = products.GetData().Where(x => x.Product_Name.Equals(item.Split('-')[0])).FirstOrDefault().ID_Product;
                        command.CommandText = $"insert into [Products_In_Order] values ({productId}, {idOrder})";
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    MessageBox.Show("Заказ успешно оформлен!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                connection.Close();
                Close();
            }
        }
    }
}

