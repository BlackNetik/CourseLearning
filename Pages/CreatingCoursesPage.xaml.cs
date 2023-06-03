using CourseLearning.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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
using System.Xml.Linq;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.Win32;
using CourseLearning.Pages;
using System.Data.SqlClient;
using Npgsql;

namespace CourseLearning.Pages
{
    /// <summary>
    /// Логика взаимодействия для CreatingCoursesPage.xaml
    /// </summary>
    public partial class CreatingCoursesPage : Page
    {
        //Лист страниц, необходимый для дальнейшего сохранения в JSON файле
        List<PageObject> pageObjects = new List<PageObject>();

        //Итератор для страниц
        int actualIterator = 1;

        //Строка для подключения к БД
        string ConnectionToBD = "Server=localhost; port=5432; user id=postgres; password=password; database=courselearning;";

        //Пользователь
        User CurrentUser { get; set; }

        public CreatingCoursesPage(User user)
        {
            InitializeComponent();
            CurrentUser= user;
        }


        private PageObject ExtractPageObjectFromMarkup()
        {
            PageObject pageObject = new PageObject();

            // Extract page number
            string pageNumberText = PageNumber.Text;
            string[] pageNumberParts = pageNumberText.Split(':');
            int pageNumber = int.Parse(pageNumberParts[1].Trim());
            pageObject.page_number = pageNumber;

            // Extract header
            pageObject.header = PageHeader.Text;

            // Extract text
            pageObject.text = PageText.Text;

            // Extract standardized test
            TestObject testObject = new TestObject();
            testObject.question = TestQuestion.Text;
            testObject.answer_options = new string[]
            {
                AnswerOption1.Text,
                AnswerOption2.Text,
                AnswerOption3.Text,
                AnswerOption4.Text
            };
            testObject.correct_answer = CorrectAnswer.SelectedIndex + 1;
            pageObject.standardized_test = testObject;

            // Extract regular question with written answer
            pageObject.question = RegularQuestion.Text;
            pageObject.correct_answer = CorrectAnswerText.Text;

            return pageObject;
        }


        private void SaveButtonCreating_Click(object sender, RoutedEventArgs e)
        {
            //Добавление объекта в список объектов
            PageObject result = ExtractPageObjectFromMarkup();
            pageObjects.Add(result);

            // Serialize the modified list of PageObject objects back into a JSON string
            string newJsonString = JsonSerializer.Serialize(pageObjects, new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic) });

            //Путь к файлу
            var filePath = string.Empty;

            //Фильтр для сохранения
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Json files (*.json)|*.json|All files (*.*)|*.*"
            };

            //Открытие диалогового окна для выбора пути и названия файла
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != string.Empty)
            {
                filePath = saveFileDialog.FileName;
            }


            // Write the new JSON string back to the file, overwriting the existing data
            File.WriteAllText(filePath, newJsonString);

            //Добавление курса в базу данных
            AddOrUpdateCourse(CurrentUser.Id, pageObjects[0].header,"");

            MessageBox.Show("Курс сохранен!");
        }

        private void NextPageCreating_Click(object sender, RoutedEventArgs e)
        {
            if (CheckHeaderAndText())
            {
                return;
            }

            //Добавление объекта в список объектов
            if(actualIterator > pageObjects.Count)
            {
                PageObject result = ExtractPageObjectFromMarkup();
                pageObjects.Add(result);
            }
            else
            {
                PageObject result = ExtractPageObjectFromMarkup();
                pageObjects[actualIterator-1] = result;
            }

            //Вывод кнопки перехода на прошлую страницу
            PreviousPageCreating.Visibility = Visibility.Visible;


            //Смена итератора
            actualIterator += 1;
            FillPageObjectsCourseCreating(pageObjects, actualIterator);

            


        }

        private void PreviousPageCreating_Click(object sender, RoutedEventArgs e)
        {
            if (CheckHeaderAndText())
            {
                return;
            }

            if (actualIterator == 2) PreviousPageCreating.Visibility = Visibility.Collapsed;

            //Добавление объекта в список объектов
            if (actualIterator > pageObjects.Count)
            {
                PageObject result = ExtractPageObjectFromMarkup();
                pageObjects.Add(result);
            }
            else
            {
                PageObject result = ExtractPageObjectFromMarkup();
                pageObjects[actualIterator-1] = result;
            }


            //Cмена итератора
            actualIterator -= 1;
            FillPageObjectsCourseCreating(pageObjects, actualIterator);

            
        }

        //Функция, которая заполняет значения полей разметки
        public void FillPageObjectsCourseCreating(List<PageObject> pObjects, int iterator)
        {
            if(iterator > pageObjects.Count)
            {
                //Если страница новая - выводим пустую разметку
                ClearTextBoxes();
            }
            else
            {
                //Отнимаем еденицу от итератора, поскольку он не совпадает с индексом
                iterator -= 1;
                PageNumber.Text = $"Страница: {actualIterator}";
                PageHeader.Text = pObjects[iterator].header;
                PageText.Text = pObjects[iterator].text;
                TestQuestion.Text = pObjects[iterator].standardized_test.question;
                AnswerOption1.Text = pObjects[iterator].standardized_test.answer_options[0];
                AnswerOption2.Text = pObjects[iterator].standardized_test.answer_options[1];
                AnswerOption3.Text = pObjects[iterator].standardized_test.answer_options[2];
                AnswerOption4.Text = pObjects[iterator].standardized_test.answer_options[3];
                CorrectAnswer.SelectedIndex = pObjects[iterator].standardized_test.correct_answer - 1;

                RegularQuestion.Text = pObjects[iterator].question;
                CorrectAnswerText.Text = pObjects[iterator].correct_answer;
            }
        }

            //Функция, которая очищает значения полей разметки
            private void ClearTextBoxes()
        {
            PageHeader.Text = "";
            PageText.Text = "";
            TestQuestion.Text = "";
            AnswerOption1.Text = "";
            AnswerOption2.Text = "";
            AnswerOption3.Text = "";
            AnswerOption4.Text = "";
            CorrectAnswer.SelectedIndex = 0;
            RegularQuestion.Text = "";
            CorrectAnswerText.Text = "";
        }

        //Проверка, пустой ли заголовок и текст страницы
        private bool CheckHeaderAndText()
        {
            return PageHeader.Text == "" || PageText.Text == "";
        }

        //Функция, которая добавляет курс в базу данных
        public void AddOrUpdateCourse(int userId, string name, string url)
        {
            using (var connection = new NpgsqlConnection(ConnectionToBD))
            {
                connection.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = "SELECT COUNT(*) FROM Courses WHERE id_user = @userId AND name = @name";
                    cmd.Parameters.AddWithValue("userId", userId);
                    cmd.Parameters.AddWithValue("name", name);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    if (count == 0)
                    {
                        cmd.CommandText = "INSERT INTO Courses (id_user, name, url) VALUES (@userId, @name, @url)";
                        cmd.Parameters.AddWithValue("url", url);

                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        cmd.CommandText = "UPDATE Courses SET url = @url WHERE id_user = @userId AND name = @name";
                        cmd.Parameters.AddWithValue("url", url);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }


    }
}
