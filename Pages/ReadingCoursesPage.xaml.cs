﻿using CourseLearning.Classes;
using Microsoft.Win32;
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

        public ReadingCoursesPage()
        {
            InitializeComponent();

            //Скрытые элементов отображения страницы
            PageReadingLayout.Visibility = Visibility.Collapsed;
        }

        private void NextPageReadingCoursesButton_Click(object sender, RoutedEventArgs e)
        {
            iterator++;
            FillPageObjectsReading(pageObjects, iterator);
        }

        private void FindFileButton_Click(object sender, RoutedEventArgs e)
        {
            //Открытие файла
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Json files (*.json)|*.json|All files (*.*)|*.*";

            
            if (openFileDialog.ShowDialog() == true)
            {
                //Скрытие кнопки открытия файла
                PageReadingLayout.Visibility = Visibility.Visible;
                PageLoadingLayout.Visibility = Visibility.Collapsed;

                //Получение пути файла
                jsonPath = openFileDialog.FileName;

                //Получение контекста
                string jsonContents = File.ReadAllText(jsonPath);

                JsonSerializerOptions jso = new JsonSerializerOptions();
                jso.Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic);

                //Считывания с файла в список объектов класса
                pageObjects = JsonSerializer.Deserialize<List<PageObject>>(jsonContents, jso);

                //Заполнение значений
                FillPageObjectsReading(pageObjects, iterator);
            }


        }

        

        //Функция, заполняющая элементы разметки список объекта по итератору
        public void FillPageObjectsReading(List<PageObject> pObjects, int iterator)
        {

            HeaderPageReading.Text = pObjects[iterator].header;
            TextPageReading.Text = pObjects[iterator].text;
            TestQuestionReading.Text = pObjects[iterator].standardized_test.question;

            FirstAnswerTestReading.Content = pObjects[iterator].standardized_test.answer_options[0];
            SecondAnswerTestReading.Content = pObjects[iterator].standardized_test.answer_options[1];
            ThirdAnswerTestReading.Content = pObjects[iterator].standardized_test.answer_options[2];
            FourAnswerTestReading.Content = pObjects[iterator].standardized_test.answer_options[3];

            StandartQuestionReading.Text = pObjects[iterator].question;

            //Изменить потом условие if
            if (pObjects[iterator].page_number== pObjects.Count)
            {
                NextPageReadingCoursesButton.Content = "Проверить ответы";
            }
            //StandartAnswerReading.Text = pObjects[iterator].correct_answer;
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

    }
}
