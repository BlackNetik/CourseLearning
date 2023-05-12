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

            //Подключение к базе данных
            var conn = GetConnection();
            var cmd = new NpgsqlCommand();

            using (conn)
            {
                // Открытие подключения
                conn.Open();

                // Проверка наличия базы данных
                using (var testCmd = new NpgsqlCommand("SELECT COUNT(*) FROM pg_database WHERE datname = 'courselearning'", conn))
                {
                    object resultObj = testCmd.ExecuteScalar();
                    int result = 0;
                    if (resultObj != null && resultObj != DBNull.Value)
                    {
                        result = Convert.ToInt32(resultObj);
                    }

                    // Создание базы данных, если она не существует
                    if (result == 0)
                    {
                        using (var createCmd = new NpgsqlCommand("CREATE DATABASE courselearning", conn))
                        {
                            createCmd.ExecuteNonQuery();
                        }

                        // Проверка наличия таблицы Users
                        using (var usersCmd = new NpgsqlCommand("SELECT to_regclass('public.users')", conn))
                        {
                            bool tableExists = (usersCmd.ExecuteScalar() != DBNull.Value);

                            // Создание таблицы Users, если она не существует
                            if (!tableExists)
                            {
                                using (var createCmd = new NpgsqlCommand(
                                    "CREATE TABLE Users (" +
                                        "id SERIAL PRIMARY KEY, " +
                                        "username VARCHAR(50) UNIQUE NOT NULL, " +
                                        "password VARCHAR(255) NOT NULL, " +
                                        "first_name VARCHAR(50) NOT NULL, " +
                                        "last_name VARCHAR(50) NOT NULL" +
                                    ");", conn))
                                {
                                    createCmd.ExecuteNonQuery();
                                }
                            }
                        }

                        // Проверка наличия таблицы CourseProgress
                        using (var progressCmd = new NpgsqlCommand("SELECT to_regclass('public.courseprogress')", conn))
                        {
                            bool tableExists = (progressCmd.ExecuteScalar() != DBNull.Value);

                            // Создание таблицы CourseProgress, если она не существует
                            if (!tableExists)
                            {
                                using (var createCmd = new NpgsqlCommand(
                                    "CREATE TABLE CourseProgress (" +
                                        "id SERIAL PRIMARY KEY, " +
                                        "id_user INTEGER NOT NULL, " +
                                        "course_name TEXT, " +
                                        "progress INTEGER NOT NULL, " +
                                        "status_course TEXT, " +
                                        "FOREIGN KEY (id_user) REFERENCES Users(id)" +
                                    ");", conn))
                                {
                                    createCmd.ExecuteNonQuery();
                                }
                            }
                        }

                        // Проверка наличия таблицы Courses
                        using (var coursesCmd = new NpgsqlCommand("SELECT to_regclass('public.courses')", conn))
                        {
                            bool tableExists = (coursesCmd.ExecuteScalar() != DBNull.Value);

                            // Создание таблицы Courses, если она не существует
                            if (!tableExists)
                            {
                                using (var createCmd = new NpgsqlCommand(
                                    "CREATE TABLE Courses (" +
                                        "Id SERIAL PRIMARY KEY, " +
                                        "id_user INTEGER, " +
                                        "name TEXT, " +
                                        "url TEXT" +
                                    ");", conn))
                                {
                                    createCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }

            }
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

                //MessageBox.Show(user.Username.ToString() + " — Существующий пользовательский логин");
                //return user;

                //Переход на новое окно
                changeToMainWindow(user);

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
        public void changeToMainWindow(User transferUser)
        {
            MainWindow mainWindow = new MainWindow(transferUser) { WindowStartupLocation = WindowStartupLocation.CenterScreen };
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
