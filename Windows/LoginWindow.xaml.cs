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
using CourseLearning.Windows;
using Npgsql;

namespace CourseLearning
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        string ConnectionToBD = "Server=localhost; port=5432; user id=postgres; password=password; database=courselearning;";


        public LoginWindow()
        {
            InitializeComponent();

            //Заполняем данные чтобы каждый раз не вводить по новой. После завершения работы не забыть удалить
            loginTextBox.Text = "admin";
            passwordPasswordBox.Password = "admin";
        }
        


        private void joinButton_Click(object sender, RoutedEventArgs e)
        {
            //Подключение к базе данных
            var conn = GetConnection();
            conn.Open();
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            //Отправка запроса на поиск пользователя в базе данных
            cmd.CommandText = "SELECT id, username, password, first_name, last_name " +
                              "FROM Users " +
                              $"WHERE username = @username AND password = @password";

            //Добавление сторонних данных в запрос
            cmd.Parameters.AddWithValue("username", loginTextBox.Text);
            cmd.Parameters.AddWithValue("password", passwordPasswordBox.Password.ToString());

            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {

                //Создание экземпляра пользователя для дальнейшего использования в программе
                var user = new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Password = reader.GetString(2),
                    FirstName = reader.GetString(3),
                    LastName = reader.GetString(4)
                };
                MessageBox.Show(user.Username.ToString() + " — Существующий пользовательский логин");
                //return user;

                //Переход на новое окно
                changeToMainWindow();

            }
            else
            {
                MessageBox.Show("Ошибка");
            }
            
        }

        //Функция, используемая для перехода на окно регистрации
        private void registrationButton_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow() { WindowStartupLocation = WindowStartupLocation.CenterScreen };
            this.Close();
            registrationWindow.Show();
        }

        //Функция, используемая для перехода на основное окно после авторизации или регистрации
        public void changeToMainWindow()
        {
            MainWindow mainWindow = new MainWindow() { WindowStartupLocation = WindowStartupLocation.CenterScreen };
            this.Close();
            mainWindow.Show();
        }

        //Функция, возвращающая подключение к БД
        public static NpgsqlConnection GetConnection()
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
                        MessageBox.Show("Database connection successful!");
                    }
                    else
                    {
                        // Connection failed
                        MessageBox.Show("Database connection failed!");
                    }
                }
            }
            

        }
    }
}
