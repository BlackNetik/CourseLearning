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
using CourseLearning.Classes;
using Npgsql;

namespace CourseLearning
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string ConnectionToBD = "Server=localhost; port=5432; user id=postgres; password=password; database=courselearning;";


        public MainWindow()
        {
            InitializeComponent();
        }
        


        private void joinButton_Click(object sender, RoutedEventArgs e)
        {
            //TestConnection();
            
            var conn = new NpgsqlConnection(ConnectionToBD);
            conn.Open();
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            cmd.CommandText = "SELECT id, username, password, first_name, last_name " +
                              "FROM Users " +
                              $"WHERE username = @username AND password = @password";


            cmd.Parameters.AddWithValue("username", loginTextBox.Text);
            cmd.Parameters.AddWithValue("password", passwordPasswordBox.Password.ToString());


            //var reader = cmd.ExecuteScalar().ToString();
            //MessageBox.Show(reader);

            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                
                
                var user = new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Password = reader.GetString(2),
                    FirstName = reader.GetString(3),
                    LastName = reader.GetString(4)
                };
                MessageBox.Show(user.Username.ToString());
                //return user;

            }
            else
            {
                MessageBox.Show("Ошибка");
            }
            
        }

        private void registrationButton_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow() { WindowStartupLocation = WindowStartupLocation.CenterScreen };
            this.Close();
            registrationWindow.Show();
        }

        private static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection("Server=localhost; port=5432; user id=postgres; password=password; database=courselearning;");
        }

        private void TestConnection()
        {
            
            using (var conn = new NpgsqlConnection(ConnectionToBD))
            {
                // Open the database connection
                conn.Open();

                // Create a new NpgsqlCommand object with your query and the NpgsqlConnection object
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                {
                    // Execute the query and store the result in a variable
                    var result = cmd.ExecuteScalar();

                    // Check if the result is not null and is equal to 1
                    if (result != null && result.Equals(1))
                    {
                        // Connection is successful
                        //Console.WriteLine("Database connection successful!");
                        MessageBox.Show("Отлично");
                    }
                    else
                    {
                        // Connection failed
                        //Console.WriteLine("Database connection failed!");
                        MessageBox.Show("Ошибка");
                    }
                }
            }
            /*
            using (NpgsqlConnection connection = GetConnection())
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    MessageBox.Show("Connected");
                }
                else
                {
                    MessageBox.Show("Disconnected");
                }
            }
            */

        }
    }
}
