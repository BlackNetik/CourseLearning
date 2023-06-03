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
using CourseLearning.Classes;
using Npgsql;

namespace CourseLearning
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public User user = new User();

        //Строка для подключения к БД
        string ConnectionToBD = "Server=localhost; port=5432; user id=postgres; password=password; database=courselearning;";

        public RegistrationWindow()
        {
            InitializeComponent();
        }

        public void initializeUser()
        {
            user.Username = usernameUserReg.Text.ToString();
            user.Password = passwordUserReg.Text.ToString();
            user.FirstName = firstNameUser.Text.ToString();
            user.LastName = lastNameUser.Text.ToString();
        }

        private void registerNewAccountButton_Click(object sender, RoutedEventArgs e)
        {
            //Инициализация
            initializeUser();

            //Подключение к базе данных
            var conn = LoginWindow.GetConnection();
            conn.Open();

            //Проверка значений
            if (!checkPassword(passwordUserReg.Text.ToString(), correctPasswordUserReg.Text.ToString()))
                {
                    MessageBox.Show("Пароли не совпадают или не записаны");
                    passwordUserReg.Text = "";
                    correctPasswordUserReg.Text = "";
                    return;
                }
                //Проверка, не заняти ли кем-то по логин
            else if (checkUsernameExists(user.Username) == true)
                {
                    MessageBox.Show("Такой логин уже занят");
                    return;
                }
            else
                {
                //Отправка запроса на добавление пользователя в базу данных
                var cmd = new NpgsqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO Users (username, password, first_name, last_name) " +
                        "VALUES (@username, @password, @first_name, @last_name)";
                cmd.Parameters.AddWithValue("username", user.Username);
                cmd.Parameters.AddWithValue("password", user.Password);
                cmd.Parameters.AddWithValue("first_name", user.FirstName);
                cmd.Parameters.AddWithValue("last_name", user.LastName);

                // execute command
                cmd.ExecuteNonQuery();
                //Сообщение об успешном добавлении и очистка полей
                MessageBox.Show("Пользователь успешно добавлен!");
                clearAll();
                }
            
        }

        //Функция очистки полей
        private void clearAll()
        {
            usernameUserReg.Text = "";
            passwordUserReg.Text = "";
            correctPasswordUserReg.Text = "";
            firstNameUser.Text = "";
            lastNameUser.Text = "";
        }

        //Функция проверки паролей
        private bool checkPassword(string firstpassword, string secondpassword)
        {
            if (firstpassword == "" || secondpassword == "") return false;
            return firstpassword == secondpassword;
        }

        //Функция, перенапрявляющяя на окно авторизации после нажатия на отмену
        public void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow mainWindow = new LoginWindow() { WindowStartupLocation = WindowStartupLocation.CenterScreen };
            this.Close();
            mainWindow.Show();
        }

        private bool checkUsernameExists(string username) //Исправить
        {
            if (username == "")
            {
                return false;
            }
            //Подключение к базе данных
            using (var connection = new NpgsqlConnection(ConnectionToBD))
            {
                connection.Open();

               
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = "SELECT COUNT(*) FROM Users WHERE username = @username";
                    cmd.Parameters.AddWithValue("username", username);

                    // execute command and get count of users with the given username
                    //int count = (int)cmd.ExecuteScalar();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        int count = Convert.ToInt32(result);
                        return count > 0;
                    }
                    else
                    {
                        return false;
                    }
                }

            }
        }

    }
}
