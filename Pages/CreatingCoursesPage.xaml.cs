﻿using CourseLearning.Classes;
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

            // Write the new JSON string back to the file, overwriting the existing data
            File.WriteAllText("example.json", newJsonString);

            MessageBox.Show("Курс сохранен!");
        }

        private void NextPageCreating_Click(object sender, RoutedEventArgs e)
        {
            //Добавление объекта в список объектов
            PageObject result = ExtractPageObjectFromMarkup();
            pageObjects.Add(result);

            //Очистка значений в разметке
            ClearTextBoxes();

            //Запись новой страницы
            string pageNumberText = PageNumber.Text;
            string[] pageNumberParts = pageNumberText.Split(':');
            int pageNumber = int.Parse(pageNumberParts[1].Trim());
            PageNumber.Text = $"Страница: {pageNumber+=1}";


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

    }
}
