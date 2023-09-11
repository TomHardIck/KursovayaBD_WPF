using DesktopApp.PraktikaShimbirevDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Security.Cryptography;

namespace DesktopApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static int loggedId;
        private static UserTableAdapter users = new UserTableAdapter();
        private static UserRoleTableAdapter roles = new UserRoleTableAdapter();
        private static HashAlgorithm hash = new HashAlgorithm();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void authButton_Click(object sender, RoutedEventArgs e)
        {
            int roleId = 0;
            foreach(var role in roles.GetData())
            {
                if(role.User_Role_Name == "Администратор")
                {
                    roleId = role.ID_User_Role;
                    break;
                }
            }
            foreach(var user in users.GetData())
            {
                if(user.User_Login.ToString() == loginInput.Text && hash.AreEqual(passwordInput.Text, user.User_Password, user.Salt) && user.User_Role_ID == roleId)
                {
                    loggedId = user.ID_User;
                    MessageBox.Show("Успешная авторизация!");
                    DataWindow dataWindow = new DataWindow();
                    dataWindow.Show();
                }
            }
        }
    }
}
