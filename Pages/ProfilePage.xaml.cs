using CourseLearning.Classes;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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

namespace CourseLearning.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        //private ProfilePageViewModel viewModel;
        string ConnectionToBD = "Server=localhost; port=5432; user id=postgres; password=password; database=courselearning;";
        User ProfileUser { get; set; }

        public ProfilePage(User user)
        {
            InitializeComponent();

            this.ProfileUser = user;
            this.DataContext = user; // set user object as DataContext for data binding
            LoadCourseProgress(); // load course progress data for the user
        }
        private void LoadCourseProgress()
        {
            try
            {
                using (var conn = new NpgsqlConnection(ConnectionToBD))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT course_name, progress, status_course FROM CourseProgress WHERE id_user = @userId";
                        cmd.Parameters.AddWithValue("userId", ProfileUser.Id);
                        //По какой-то причине не выводит полученную информацию
                        using (var reader = cmd.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);
                            courseProgressDataGrid.ItemsSource = dt.DefaultView;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }

}
