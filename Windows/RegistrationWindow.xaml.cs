﻿using System;
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
            var conn = MainWindow.GetConnection();
            conn.Open();


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

        private void clearAll()
        {
            usernameUserReg.Text = "";
            passwordUserReg.Text = "";
            firstNameUser.Text = "";
            lastNameUser.Text = "";
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow= new MainWindow() { WindowStartupLocation = WindowStartupLocation.CenterScreen };
            this.Close();
            mainWindow.Show();
        }
    }
}
