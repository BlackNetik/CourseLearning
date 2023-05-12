using CourseLearning.Classes;
using Microsoft.Win32;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
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
    /// Логика взаимодействия для ReadingCoursesPage.xaml
    /// </summary>
    public partial class ReadingCoursesPage : Page
    {
        string jsonPath = "";
        List<PageObject> pageObjects = new List<PageObject>();
        int iterator = 0;

        //Объект пользователя
        User CurentUser = null;

        //Строка для подключения к БД
        string ConnectionToBD = "Server=localhost; port=5432; user id=postgres; password=password; database=courselearning;";

        public ReadingCoursesPage(User user)
        {
            InitializeComponent();

            //Скрытые элементов отображения страницы
            PageReadingLayout.Visibility = Visibility.Collapsed;

            CurentUser= user;
        }

        private void NextPageReadingCoursesButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ResultTestReading(pageObjects[iterator])) //Проверка результата теста
            {
                MessageBox.Show($"Дан неверный вариант ответа на тест. Попробуй ещё раз");
            }
            else if (!ResultAnswerReading(pageObjects[iterator])) //Проверка ответа на вопрос
            {
                MessageBox.Show($"Дан неверный ответ на вопрос. Попробуй ещё раз");
            }
            else
            {
                //Переход по итератору на новую страницу
                iterator++;
                FillPageObjectsReading(pageObjects, iterator);


                //Сохранение прогресса
                AddOrUpdateCourseProgress(CurentUser.Id, pageObjects[0].header, iterator, pageObjects.Count);

                //Проверка на заполненность значений в тесте и наличие вопроса
                CheckStandartQuestionReading(pageObjects[iterator]);
                CheckTestPageReading(pageObjects[iterator]);

                //Снятие галочки с выбранных вариантов ответа в тесте
                FirstAnswerTestReading.IsChecked = false;
                SecondAnswerTestReading.IsChecked = false;
                ThirdAnswerTestReading.IsChecked = false;
                FourAnswerTestReading.IsChecked = false;
            }

        }

        private void FindFileButton_Click(object sender, RoutedEventArgs e)
        {
            //Открытие файла
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Json files (*.json)|*.json|All files (*.*)|*.*";

            
            if (openFileDialog.ShowDialog() == true)
            {

                //Получение пути файла
                jsonPath = openFileDialog.FileName;

                //Получение контекста
                string jsonContents = File.ReadAllText(jsonPath);

                JsonSerializerOptions jso = new JsonSerializerOptions();
                jso.Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic);

                //Считывания с файла в список объектов класса
                pageObjects = JsonSerializer.Deserialize<List<PageObject>>(jsonContents, jso);

                //Получение значения итератора
                iterator = GetCourseProgress(CurentUser.Id, pageObjects[0].header,pageObjects.Count);

                if(iterator < pageObjects.Count)
                {
                    //Скрытие кнопки открытия файла
                    PageReadingLayout.Visibility = Visibility.Visible;
                    PageLoadingLayout.Visibility = Visibility.Collapsed;

                    //Заполнение значений
                    FillPageObjectsReading(pageObjects, iterator);

                    CheckStandartQuestionReading(pageObjects[iterator]);
                    CheckTestPageReading(pageObjects[iterator]);

                    //Сохранение прогресса
                    AddOrUpdateCourseProgress(CurentUser.Id, pageObjects[0].header, iterator, pageObjects.Count);
                }
                else
                {
                    MessageBox.Show("Курс пройден! Поздравляю!");
                }

                
            }


        }

        

        //Функция, заполняющая элементы разметки список объекта по итератору
        public void FillPageObjectsReading(List<PageObject> pObjects, int iterator)
        {
            PageCountReading.Text = $"Страница {pObjects[iterator].page_number}/{pObjects.Count}";
            HeaderPageReading.Text = pObjects[iterator].header;
            TextPageReading.Text = pObjects[iterator].text;
            TestQuestionReading.Text = pObjects[iterator].standardized_test.question;

            FirstAnswerTestReading.Content = pObjects[iterator].standardized_test.answer_options[0];
            SecondAnswerTestReading.Content = pObjects[iterator].standardized_test.answer_options[1];
            ThirdAnswerTestReading.Content = pObjects[iterator].standardized_test.answer_options[2];
            FourAnswerTestReading.Content = pObjects[iterator].standardized_test.answer_options[3];

            StandartQuestionReading.Text = pObjects[iterator].question;
            StandartAnswerReading.Text = "";

            //Проверка на последнюю страницу
            if (CheckLastPage(pObjects, iterator))
            {
                NextPageReadingCoursesButton.Visibility = Visibility.Collapsed;
                if (!CheckTestAndStandartQuestionReading(pObjects[iterator]))
                {
                    LastPageReadingCoursesButton.Visibility = Visibility.Visible;
                }  

            }
        }

        //Функция, проверяющая последняя ли это страница в списке
        public bool CheckLastPage(List<PageObject> pObjects, int iterator)
        {
            return pObjects[iterator].page_number == pObjects.Count;
        }

        //Функция, которая скрывает тест, если он не заполнен
        public void CheckTestPageReading(PageObject pObject)
        {
            if(pObject.standardized_test.question == "")
            {
                TestQuestionReading.Visibility= Visibility.Collapsed;
                AnswersOnTestQuestionReading.Visibility = Visibility.Collapsed;
            }
            else
            {
                TestQuestionReading.Visibility = Visibility.Visible;
                AnswersOnTestQuestionReading.Visibility = Visibility.Visible;
            }
        }

        //Функция, которая скрывает стандартный вопрос, если он не заполнен
        public void CheckStandartQuestionReading(PageObject pObject)
        {
            if(pObject.question == "")
            {
                StandartQuestionReading.Visibility = Visibility.Collapsed;
                StandartAnswerReading.Visibility = Visibility.Collapsed;
            }
            else
            {
                StandartQuestionReading.Visibility = Visibility.Visible;
                StandartAnswerReading.Visibility = Visibility.Visible;
            }
        }

        //Функция, проверяющая на наличие теста или вопроса
        public bool CheckTestAndStandartQuestionReading(PageObject pObject)
        {
            return pObject.question == "" || pObject.standardized_test.question == "";
        }

        //Кнопка для последней страницы
        private void LastPageReadingCoursesButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ResultTestReading(pageObjects[iterator])) //Проверка результата теста
            {
                MessageBox.Show($"Дан неверный вариант ответа на тест. Попробуй ещё раз");
            }
            else if (!ResultAnswerReading(pageObjects[iterator])) //Проверка ответа на вопрос
            {
                MessageBox.Show($"Дан неверный ответ на вопрос. Попробуй ещё раз");
            }
            else
            {
                //Сохранение прогресса
                AddOrUpdateCourseProgress(CurentUser.Id, pageObjects[0].header, iterator + 1, pageObjects.Count);

                //переход в профиль
                NavigationService.Navigate(new ProfilePage(CurentUser));
                //contentFrame.Navigate(new ProfilePage(ProfileUser));
            }
        }

        //Функция, проверяющая ответ теста на правильность
        public bool ResultTestReading(PageObject pObject)
        {
            switch (pObject.standardized_test.correct_answer)
            {
                case 1:
                    return FirstAnswerTestReading.IsChecked == true;
                case 2:
                    return SecondAnswerTestReading.IsChecked == true;
                case 3:
                    return ThirdAnswerTestReading.IsChecked == true;
                case 4:
                    return FourAnswerTestReading.IsChecked == true;
            }
            return false;
            
        }

        //Функция, проверяющая ответ на вопрос
        public bool ResultAnswerReading(PageObject pObject)
        {
            return StandartAnswerReading.Text == pObject.correct_answer.ToString();
        }

        //Функция, сохраняющая прогресс прохождения курса в базе данных
        public void AddOrUpdateCourseProgress(int userId, string courseName, int currentPage, int totalPages)
        {
            var progress = (int)Math.Round((double)currentPage / totalPages * 100);

            using (var connection = new NpgsqlConnection(ConnectionToBD))
            {
                connection.Open();

                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT COUNT(*) FROM courseprogress WHERE id_user = @userId AND course_name = @courseName";
                    command.Parameters.AddWithValue("userId", userId);
                    command.Parameters.AddWithValue("courseName", courseName);

                    var count = (long)command.ExecuteScalar();

                    if (count == 0)
                    {
                        command.CommandText = "INSERT INTO courseprogress (id_user, course_name, progress, status_course) VALUES (@userId, @courseName, @progress, @status)";
                        command.Parameters.AddWithValue("progress", progress);
                        command.Parameters.AddWithValue("status", progress == 100 ? "Пройдено" : "Прохожу");
                    }
                    else
                    {
                        command.CommandText = "UPDATE courseprogress SET progress = @progress, status_course = @status WHERE id_user = @userId AND course_name = @courseName";
                        command.Parameters.AddWithValue("progress", progress);
                        command.Parameters.AddWithValue("status", progress == 100 ? "Пройдено" : "Прохожу");
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        // Функция, проверяющая прогресс прохождения курса в базе данных и возвращающая текущую страницу
        public int GetCourseProgress(int userId, string courseName, int totalPages)
        {
            using (var connection = new NpgsqlConnection(ConnectionToBD))
            {
                connection.Open();

                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT COUNT(*) FROM courseprogress WHERE id_user = @userId AND course_name = @courseName";
                    command.Parameters.AddWithValue("userId", userId);
                    command.Parameters.AddWithValue("courseName", courseName);

                    var count = (long)command.ExecuteScalar();

                    if (count == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        command.CommandText = "SELECT progress FROM courseprogress WHERE id_user = @userId AND course_name = @courseName";
                        var progress = (int)command.ExecuteScalar();

                        return (int)Math.Round((double)progress / 100 * totalPages);
                    }
                }
            }
        }

    }
}
