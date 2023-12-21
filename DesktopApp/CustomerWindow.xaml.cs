using DesktopApp.PraktikaShimbirevDataSetTableAdapters;
using Newtonsoft.Json.Linq;
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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DesktopApp
{
    /// <summary>
    /// Логика взаимодействия для CustomerWindow.xaml
    /// </summary>
    public partial class CustomerWindow : Window
    {
        ProductDataTableAdapter productsView = new ProductDataTableAdapter();
        OrderTableAdapter orders = new OrderTableAdapter();
        public static List<string> cart = new List<string>();
        CategoryTableAdapter cats = new CategoryTableAdapter();
        public static double cartSum;
        public static DataRowView curRow;
        public CustomerWindow()
        {
            InitializeComponent();
            productsGrid.ItemsSource = productsView.GetData();
            Style rowStyle = new Style(typeof(DataGridRow));
            rowStyle.Setters.Add(new EventSetter(DataGridRow.MouseDoubleClickEvent,
                                     new MouseButtonEventHandler(Row_DoubleClick)));
            productsGrid.RowStyle = rowStyle;
            cartData.ItemsSource = cart;
            categoriesFilter.ItemsSource = cats.GetData().Select(x => x.Category_Name);
            myOrdersGrid.ItemsSource = orders.GetData().Where(x => x.User_ID.Equals(MainWindow.loggedId)).CopyToDataTable().DefaultView;
            cartData.ItemsSource = null;
            cartSum = 0;
        }

        private void addToCart_Click(object sender, RoutedEventArgs e)
        {
            if ((DataRowView)productsGrid.SelectedItem != null)
            {
                try
                {
                    var value = ((DataRowView)productsGrid.SelectedItem)[0].ToString() + "-" + ((DataRowView)productsGrid.SelectedItem)[2].ToString();
                    cart.Add(value);
                    cartData.ItemsSource = null;
                    cartData.ItemsSource = cart;
                    cartSum += double.Parse(value.Split('-')[1]);
                    itogSum.Text = cartSum.ToString();
                    MessageBox.Show("Товар добавлен в корзину!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для добавления в корзину!");
            }
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataRowView dataRow = (DataRowView)productsGrid.SelectedItem;
            try
            {
                if (dataRow != null)
                {
                    curRow = dataRow;
                    ProductWindow productWindow = new ProductWindow();
                    productWindow.Show();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void categoriesFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (categoriesFilter.SelectedItem != null)
            {
                var filteredData = productsView.GetData().Where(x => x.Категория.Equals(categoriesFilter.SelectedItem.ToString()));
                if (filteredData.Count() > 0)
                {
                    productsGrid.ItemsSource = filteredData.CopyToDataTable().DefaultView;
                }
                else
                {
                    productsGrid.ItemsSource = null;
                    MessageBox.Show("Товаров этой категории не найдено!");
                }
            }
        }

        private void createOrder_Click(object sender, RoutedEventArgs e)
        {
            PaymentWindow payment = new PaymentWindow();
            payment.Show();
        }

        private void deletePos_Click(object sender, RoutedEventArgs e)
        {
            var selectedValue = cartData.SelectedItem.ToString();
            cart.Remove(selectedValue);
            cartSum -= double.Parse(selectedValue.Split('-')[1]);
            cartData.ItemsSource = null;
            cartData.ItemsSource = cart;
            itogSum.Text = cartSum.ToString();
            MessageBox.Show("Товар удален из корзины!");
        }
    }
}
