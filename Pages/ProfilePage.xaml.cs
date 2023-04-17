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

    /*
    public class ProfilePageViewModel : INotifyPropertyChanged
    {
        

        // Define the properties for user information and course progress
        private string username;
        private string password;
        private string firstName;
        private string lastName;
        private ObservableCollection<CourseProgress> courseProgress;

        public string Username
        {
            get { return username; }
            set { username = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get { return password; }
            set { password = value; OnPropertyChanged(); }
        }

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; OnPropertyChanged(); }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; OnPropertyChanged(); }
        }

        public ObservableCollection<CourseProgress> CourseProgress
        {
            get { return courseProgress; }
            set { courseProgress = value; OnPropertyChanged(); }
        }

        // Load the course progress for the current user
        public void LoadCourseProgress()
        {
            // Get the user ID for the current user
            int userId = GetCurrentUser().Id;

            // Query the course progress table for this user
            string ConnectionToBD = "Server=localhost; port=5432; user id=postgres; password=password; database=courselearning;";
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnectionToBD))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT course_name, progress, status_course " +
                                      "FROM CourseProgress " +
                                      "WHERE id_user = @userId";
                    cmd.Parameters.AddWithValue("userId", userId);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Create a new collection to hold the course progress data
                        ObservableCollection<CourseProgress> progress = new ObservableCollection<CourseProgress>();

                        // Read each row of data and add it to the collection
                        while (reader.Read())
                        {
                            CourseProgress item = new CourseProgress();
                            item.CourseName = reader.GetString(0);
                            item.Progress = reader.GetInt32(1);
                            item.Status = reader.GetString(2);
                            progress.Add(item);
                        }

                        // Set the CourseProgress property to the new collection
                        CourseProgress = progress;
                    }
                }
            }
        }

        // Get the user object for the current user (assumed to be logged in)
        private User GetCurrentUser()
        {
            return new User { Id = 1 };
            // TODO: implement this method to get the user object for the current user
        }

        // INotifyPropertyChanged implementation for updating UI when properties change
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    */
}
