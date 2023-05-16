using CourseLearning.Classes;
using Npgsql;
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

namespace CourseLearning.Pages
{
    /// <summary>
    /// Логика взаимодействия для ListCoursesPage.xaml
    /// </summary>
    public partial class ListCoursesPage : Page
    {
        private string ConnectionToBD = "Server=localhost; port=5432; user id=postgres; password=password; database=courselearning;";

        private List<Course> courses = new List<Course>();

        public ListCoursesPage()
        {
            InitializeComponent();

            // Получаем данные из таблицы курсов и отображаем их в DataGrid
            LoadCoursesData();
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем текст из поля поиска
            string searchText = searchTextBox.Text;

            // Фильтруем список курсов по названию, содержащему searchText (без учета регистра)
            List<Course> filteredCourses = courses.Where(c => c.Name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();


            // Устанавливаем источник данных для DataGrid
            coursesDataGrid.ItemsSource = filteredCourses;
        }

        private void LoadCoursesData()
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionToBD))
                {
                    connection.Open();

                    // Запрос для получения данных из таблицы курсов и связанных с ними пользователей
                    string query = "SELECT courses.id, courses.name, courses.url, users.first_name, users.last_name " +
                                   "FROM courses " +
                                   "INNER JOIN users ON courses.id_user = users.id";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            // Создаем список объектов Course для хранения данных из таблицы курсов
                            List<Course> courses = new List<Course>();

                            while (reader.Read())
                            {
                                // Создаем объект Course и заполняем его данными из текущей строки результата запроса
                                Course course = new Course
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Url = reader.GetString(2),
                                    User = new User
                                    {
                                        FirstName = reader.GetString(3),
                                        LastName = reader.GetString(4)
                                    }
                                };

                                // Добавляем объект Course в список courses
                                courses.Add(course);
                            }

                            // Устанавливаем источник данных для DataGrid
                            coursesDataGrid.ItemsSource = courses;

                            // Сохраняем список курсов в поле класса для использования в функции searchButton_Click
                            this.courses = courses;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }
    }
}
