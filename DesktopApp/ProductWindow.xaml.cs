using DesktopApp.PraktikaShimbirevDataSetTableAdapters;
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
using System.Windows.Shapes;

namespace DesktopApp
{
    /// <summary>
    /// Логика взаимодействия для ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        ProductTableAdapter products = new ProductTableAdapter();
        CategoryTableAdapter categories = new CategoryTableAdapter();
        public ProductWindow()
        {
            InitializeComponent();
            BitmapImage imageSource = new BitmapImage();
            imageSource.BeginInit();
            imageSource.UriSource = new Uri(products.GetData().Where(x => x.Product_Name.Equals(CustomerWindow.curRow[0].ToString())).FirstOrDefault().Photo_URL);
            productName.Text = products.GetData().Where(x => x.Product_Name.Equals(CustomerWindow.curRow[0].ToString())).FirstOrDefault().Product_Name;
            productQuantity.Text = "В наличии: " + products.GetData().Where(x => x.Product_Name.Equals(CustomerWindow.curRow[0].ToString())).FirstOrDefault().Product_Quantity.ToString();
            productCategory.Text = categories.GetData().FindByID_Category(products.GetData().Where(x => x.Product_Name.Equals(CustomerWindow.curRow[0].ToString())).FirstOrDefault().Category_ID).Category_Name;
            imageSource.EndInit();
            image.Source = imageSource;
        }
    }
}
