using CourseLearning.Classes;
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
        List<PageObject> pageObjects = new List<PageObject> { new PageObject() };
        int iterator = 0;

        public ReadingCoursesPage()
        {
            InitializeComponent();

            //Скрытые элементов отображения страницы
            PageReadingLayout.Visibility = Visibility.Collapsed;
            this.DataContext = pageObjects[iterator];
        }

        private void NextPageReadingCoursesButton_Click(object sender, RoutedEventArgs e)
        {

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

                HeaderPageReading.Text = pageObjects[0].header.ToString();
                TextPageReading.Text = pageObjects[0].text.ToString();
                //MessageBox.Show($"{testpageObjects[0].text}");
            }



        }


    }
}
