using DesktopApp.PraktikaShimbirevDataSetTableAdapters;
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

namespace DesktopApp
{
    /// <summary>
    /// Логика взаимодействия для DataWindow.xaml
    /// </summary>
    public partial class DataWindow : Window
    {
        UserTableAdapter users = new UserTableAdapter();
        UserRoleTableAdapter roles = new UserRoleTableAdapter();
        HashAlgorithm hash = new HashAlgorithm();
        Act_HistoryTableAdapter actHistory = new Act_HistoryTableAdapter();
        public DataWindow()
        {
            InitializeComponent();
            FillData();
        }

        void FillData()
        {
            usersGrid.ItemsSource = users.GetData();
            roleBox.ItemsSource = roles.GetData().AsEnumerable().Select(x => x[1]).ToList();
        }

        private void usersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedRow = (DataRowView)usersGrid.SelectedItem;
            if(selectedRow != null)
            {
                loginBox.Text = selectedRow[1].ToString();
                hashedPasswordBox.Text = selectedRow[2].ToString();
                fNameBox.Text = selectedRow[3].ToString();
                lNameBox.Text = selectedRow[4].ToString();
                for(int i = 0; i < roles.GetData().Rows.Count; i++)
                {
                    if ((int)roles.GetData().Rows[i][0] == (int)selectedRow[5])
                    {
                        roleBox.Text = roles.GetData().Rows[i][1].ToString();
                    }
                }
                saltBox.Text = selectedRow[6].ToString();
            }
        }

        private void addUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var salt = hash.CreateSalt(8);
                int roleId = 0;
                for (int i = 0; i < roles.GetData().Rows.Count; i++)
                {
                    if (roles.GetData().Rows[i][1].ToString() == roleBox.Text)
                    {
                        roleId = (int)roles.GetData().Rows[i][0];
                    }
                }
                users.Insert(loginBox.Text, hash.GenerateHash(hashedPasswordBox.Text, salt) , fNameBox.Text, lNameBox.Text, roleId, salt);
                FillData();
                var loggedUserLogin = users.GetData().AsEnumerable().First(x => (int)x[0] == MainWindow.loggedId)[1];
                actHistory.Insert(MainWindow.loggedId, $"Пользователь {loggedUserLogin} добавил новую запись с Id {users.GetData().Last().ID_User} в таблицу Пользователи. Дата {DateTime.Now}");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void editUser_Click(object sender, RoutedEventArgs e)
        {
            var selectedRow = (DataRowView)usersGrid.SelectedItem;
            if (selectedRow != null)
            {
                try
                {
                    int roleId = 0;
                    for (int i = 0; i < roles.GetData().Rows.Count; i++)
                    {
                        if (roles.GetData().Rows[i][1].ToString() == roleBox.Text)
                        {
                            roleId = (int)roles.GetData().Rows[i][0];
                        }
                    }
                    var editedId = (int)selectedRow[0];
                    users.Update(loginBox.Text, hashedPasswordBox.Text, fNameBox.Text, lNameBox.Text, roleId, saltBox.Text, (int)selectedRow[0], selectedRow[1].ToString(), selectedRow[2].ToString(), selectedRow[3].ToString(), selectedRow[4].ToString(), (int)selectedRow[5], selectedRow[6].ToString());
                    FillData();
                    var loggedUserLogin = users.GetData().AsEnumerable().First(x => (int)x[0] == MainWindow.loggedId)[1];
                    actHistory.Insert(MainWindow.loggedId, $"Пользователь {loggedUserLogin} изменил запись с Id {editedId} в таблице Пользователи. Дата {DateTime.Now}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void deleteUser_Click(object sender, RoutedEventArgs e)
        {
            var selectedRow = (DataRowView)usersGrid.SelectedItem;
            if (selectedRow != null)
            {
                try
                {
                    int roleId = 0;
                    for (int i = 0; i < roles.GetData().Rows.Count; i++)
                    {
                        if (roles.GetData().Rows[i][1].ToString() == roleBox.Text)
                        {
                            roleId = (int)roles.GetData().Rows[i][0];
                        }
                    }
                    var deletedId = (int)selectedRow[0];
                    users.Delete((int)selectedRow[0], selectedRow[1].ToString(), selectedRow[2].ToString(), selectedRow[3].ToString(), selectedRow[4].ToString(), (int)selectedRow[5], selectedRow[6].ToString());
                    FillData();
                    var loggedUserLogin = users.GetData().AsEnumerable().First(x => (int)x[0] == MainWindow.loggedId)[1];
                    actHistory.Insert(MainWindow.loggedId, $"Пользователь {loggedUserLogin} удалил запись с Id {deletedId} в таблице Пользователи. Дата {DateTime.Now}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
