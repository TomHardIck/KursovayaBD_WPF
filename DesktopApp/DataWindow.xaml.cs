using DesktopApp.PraktikaShimbirevDataSetTableAdapters;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using Table = Microsoft.SqlServer.Management.Smo.Table;

namespace DesktopApp
{
    /// <summary>
    /// Логика взаимодействия для DataWindow.xaml
    /// </summary>
    public partial class DataWindow : Window
    {
        UserTableAdapter users = new UserTableAdapter();
        UserRoleTableAdapter roles = new UserRoleTableAdapter();
        ProductTableAdapter products = new ProductTableAdapter();
        HashAlgorithm hash = new HashAlgorithm();
        Act_HistoryTableAdapter actHistory = new Act_HistoryTableAdapter();
        QueriesTableAdapter queries = new QueriesTableAdapter();
        public ChartValues<double> values = new ChartValues<double>();
        public List<String> Labels = new List<string>();
        public SeriesCollection SeriesCollection { get; set; }
        public DataWindow()
        {
            InitializeComponent();
            FillData();
        }

        void FillData()
        {
            usersGrid.ItemsSource = users.GetData();
            roleBox.ItemsSource = roles.GetData().AsEnumerable().Select(x => x[1]).ToList();
            logsGrid.ItemsSource = actHistory.GetData();
            SqlConnection connection = users.Connection;
            connection.Open();
            foreach (DataRow row in connection.GetSchema("Tables").Rows)
            {
                string tablename = (string)row[2];
                tableCombo.Items.Add(tablename);
            }
            connection.Close();
            productCombo.ItemsSource = products.GetData().Select(x => x.Product_Name);
        }

        private void usersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedRow = (DataRowView)usersGrid.SelectedItem;
            if (selectedRow != null)
            {
                loginBox.Text = selectedRow[1].ToString();
                hashedPasswordBox.Text = selectedRow[2].ToString();
                fNameBox.Text = selectedRow[3].ToString();
                lNameBox.Text = selectedRow[4].ToString();
                for (int i = 0; i < roles.GetData().Rows.Count; i++)
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
                users.Insert(loginBox.Text, hash.EncryptPassword(hashedPasswordBox.Text, salt), fNameBox.Text, lNameBox.Text, roleId, salt);
                FillData();
                var loggedUserLogin = users.GetData().AsEnumerable().First(x => (int)x[0] == MainWindow.loggedId)[1];
                actHistory.Insert(MainWindow.loggedId, $"Пользователь {loggedUserLogin} добавил новую запись с Id {users.GetData().Last().ID_User} в таблицу Пользователи. Дата {DateTime.Now}");
            }
            catch (Exception ex)
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

        private void csvButtonExport_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = users.Connection;
            connection.Open();
            try
            {
                if (tableCombo.SelectedItem != null)
                {
                    SqlCommand sqlCmd = new SqlCommand($"Select * from {tableCombo.Text}", connection);
                    SqlDataReader reader = sqlCmd.ExecuteReader();

                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "CSV Files | *.csv";
                    saveFileDialog.DefaultExt = "csv";
                    saveFileDialog.ShowDialog();
                    StreamWriter sw = new StreamWriter(saveFileDialog.FileName);
                    object[] output = new object[reader.FieldCount];

                    for (int i = 0; i < reader.FieldCount; i++)
                        output[i] = reader.GetName(i);
                    sw.WriteLine(string.Join(",", output));
                    while (reader.Read())
                    {
                        reader.GetValues(output);
                        sw.WriteLine(string.Join(",", output));
                    }
                    sw.Close();
                    reader.Close();
                    MessageBox.Show("Успешный экспорт в CSV!");
                    Process.Start(saveFileDialog.FileName);
                }
                else
                {
                    MessageBox.Show("Выберите таблицу для экспорта данных в CSV!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            connection.Close();
        }

        private void sqlButtonExport_Click(object sender, RoutedEventArgs e)
        {
            Server srv = new Server();
            srv.ConnectionContext.ServerInstance = users.Connection.DataSource;
            srv.ConnectionContext.DatabaseName = users.Connection.Database;
            string dbName = users.Connection.Database;

            Database db = new Database();
            db = srv.Databases[dbName];

            StringBuilder sb = new StringBuilder();

            foreach (Table tbl in db.Tables)
            {
                if (tbl.Name == tableCombo.Text)
                {
                    ScriptingOptions options = new ScriptingOptions();
                    options.ClusteredIndexes = true;
                    options.Default = true;
                    options.DriAll = true;
                    options.Indexes = true;
                    options.IncludeHeaders = true;

                    StringCollection coll = tbl.Script(options);
                    foreach (string str in coll)
                    {
                        sb.Append(str);
                        sb.Append(Environment.NewLine);
                    }
                    StreamWriter fs = File.CreateText($"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\output.txt");
                    fs.Write(sb.ToString());
                    fs.Close();
                    MessageBox.Show("Скрипт таблицы успешно экспортирован!");
                }
            }
        }

        private void backupButton_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection sqlConnection = new SqlConnection(users.Connection.ConnectionString);
            sqlConnection.Open();
            try
            {
                SqlCommand command = new SqlCommand();
                command.Connection = sqlConnection;
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "BAK Files | *.bak";
                saveFileDialog.DefaultExt = "bak";
                saveFileDialog.ShowDialog();
                command.CommandText = $"Backup database {users.Connection.Database} to disk='{saveFileDialog.FileName}'";
                command.ExecuteNonQuery();
                MessageBox.Show($"Резервная копия БД успешно создана по пути {saveFileDialog.FileName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            sqlConnection.Close();
        }

        private void addToDia_Click(object sender, RoutedEventArgs e)
        {
            int selectedProductId = products.GetData().Where(x => x.Product_Name.Equals(productCombo.SelectedItem.ToString())).FirstOrDefault().ID_Product;
            var sumToAdd = queries.GetProductCountInOrders(selectedProductId);
            SeriesCollection = new SeriesCollection { };
            if (Labels.Contains(productCombo.SelectedItem.ToString()))
            {
                MessageBox.Show("Уже добавлено на диаграмму!");
            }
            else
            {
                Labels.Add(productCombo.SelectedItem.ToString());
                values.Add((double)sumToAdd);
                if (SeriesCollection.Count > 0)
                {
                    SeriesCollection.Clear();
                }
                SeriesCollection.Add(new LineSeries
                {
                    Title = "Кол-во в заказах",
                    Values = values
                });
                xAxis.Labels = Labels;
                chart.Series = SeriesCollection;
            }
        }
    }
}
