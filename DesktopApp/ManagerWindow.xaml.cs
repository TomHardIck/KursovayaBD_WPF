using System;
using System.Collections.Generic;
using System.Data;
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
using DesktopApp.PraktikaShimbirevDataSetTableAdapters;

namespace DesktopApp
{
    /// <summary>
    /// Логика взаимодействия для ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        UserTableAdapter allUsers = new UserTableAdapter();
        OrderTableAdapter allOrders = new OrderTableAdapter();
        OrdersDataTableAdapter ordersData = new OrdersDataTableAdapter();
        StatusTableAdapter statuses = new StatusTableAdapter();
        QueriesTableAdapter queries = new QueriesTableAdapter();
        ProductInOrderViewTableAdapter view = new ProductInOrderViewTableAdapter();
        ProductTableAdapter products = new ProductTableAdapter();
        Products_In_OrderTableAdapter prodInOrders = new Products_In_OrderTableAdapter();
        public ManagerWindow()
        {
            InitializeComponent();
            usersChoosingForOrders.ItemsSource = allUsers.GetData().Select(x => x.User_Login);
            ordersGrid.ItemsSource = ordersData.GetData();
            statusCombo.ItemsSource = statuses.GetData().Select(x => x.Status_Name);
            prodInOrdersGrid.ItemsSource = view.GetData();
            productCombo.ItemsSource = products.GetData().Select(x => x.Product_Name);
            ordersCombo.ItemsSource = allOrders.GetData().Select(x => x.Order_Num);
        }

        private void usersChoosingForOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var userChoosed = allUsers.GetData().FirstOrDefault(x => x.User_Login.Equals(usersChoosingForOrders.SelectedItem.ToString()));
            try
            {
                ordersOfUserGrid.ItemsSource = ordersData.GetData().Where(x => x.Логин_заказчика.Equals(userChoosed.User_Login)).CopyToDataTable().DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void editStatus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRowId = allOrders.GetData().Where(x => x.Order_Num.Equals(((DataRowView)ordersGrid.SelectedItem)[0].ToString())).FirstOrDefault().ID_Order;
                int newStatus = statuses.GetData().Where(x => x.Status_Name.Equals(statusCombo.SelectedItem.ToString())).FirstOrDefault().ID_Status;
                queries.ChangeOrderStatus(newStatus, selectedRowId);
                MessageBox.Show("Статус заказа успешно обновлен!");
                ordersGrid.ItemsSource = ordersData.GetData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void addProductToOrder_Click(object sender, RoutedEventArgs e)
        {
            int orderForInsert = allOrders.GetData().Where(x => x.Order_Num.Equals(ordersCombo.SelectedItem.ToString())).FirstOrDefault().ID_Order;
            int productToInsert = products.GetData().Where(x => x.Product_Name.Equals(productCombo.SelectedItem.ToString())).FirstOrDefault().ID_Product;
            prodInOrders.Insert(productToInsert, orderForInsert);
            MessageBox.Show("Товар успешно добавлен в заказ!");
            prodInOrdersGrid.ItemsSource = view.GetData().CopyToDataTable().DefaultView;
        }
    }
}
