using CourseLearning.Classes;
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
using System.Xml.Linq;

namespace CourseLearning.Pages
{
    /// <summary>
    /// Логика взаимодействия для CreatingCoursesPage.xaml
    /// </summary>
    public partial class CreatingCoursesPage : Page
    {
        //Лист страниц, необходимый для дальнейшего сохранения в JSON файле
        List<PageObject> pageObjects = new List<PageObject>();
        public CreatingCoursesPage()
        {
            InitializeComponent();
        }


        //Функция для добавления нового нового объекта в список страниц
        public void AddPageObject(int pageNumber, string header, string text, TestObject testObject, string question, string correctAnswer)
        {
            PageObject newPage = new PageObject();
            newPage.page_number = pageNumber;
            newPage.header = header;
            newPage.text = text;
            newPage.standardized_test = testObject;
            newPage.question = question;
            newPage.correct_answer = correctAnswer;

            pageObjects.Add(newPage);
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
            //Тестирование
            PageObject result = ExtractPageObjectFromMarkup();
            MessageBox.Show($"{result.text}, {result.standardized_test.correct_answer}");
        }

        private void NextPageCreating_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
