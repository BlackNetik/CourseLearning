using CourseLearning.Classes;
using Microsoft.Win32;
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
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using Microsoft.Office.Interop.Word;

namespace CourseLearning.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : System.Windows.Controls.Page
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

                        using (var reader = cmd.ExecuteReader())
                        {
                            System.Data.DataTable dt = new System.Data.DataTable();
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

        private void profileExportButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the row that contains the button that was clicked
            var button = sender as Button;
            var row = button?.DataContext as DataRowView;

            // Check if the row is not null and display the ID in a MessageBox
            if (row != null)
            {
                /*
                string courseName = row["course_name"].ToString();
                MessageBox.Show($"The selected course is {courseName}");
                */
                AddImageAndTextToWordDocument();
            }
        }

        public void AddImageAndTextToWordDocument()
        {
            // Открываем проводник и предлагаем выбрать путь для Word файла
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Word Documents (*.docx)|*.docx";
            saveFileDialog.DefaultExt = "docx";
            saveFileDialog.AddExtension = true;
            if (saveFileDialog.ShowDialog() != true)
            {
                return;
            }

            // Создаем новое приложение Word
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            wordApp.Visible = true;

            // Добавляем новый документ
            Microsoft.Office.Interop.Word.Document doc = wordApp.Documents.Add();

            // Добавляем изображение
            string imagePath = @"C:\Users\User\Desktop\c#\CourseLearning\Images\OutputWord\CourseLearning.jpg";
            //"/Images/OutputWord/CourseLearning.jpg"
            //@"C:\Users\User\Desktop\c#\CourseLearning\Images\OutputWord\CourseLearning.jpg"
            object oMissed = doc.Paragraphs[1].Range;
            object oLinkToFile = false;
            object oSaveWithDocument = true;
            object oRange = doc.Paragraphs[1].Range;
            Microsoft.Office.Interop.Word.InlineShape inlineShape = doc.InlineShapes.AddPicture(imagePath, ref oLinkToFile, ref oSaveWithDocument, ref oRange);
            inlineShape.Width = 100;
            inlineShape.Height = 100;

            // Добавляем текст
            /*
            Microsoft.Office.Interop.Word.Range range = doc.Paragraphs.Add().Range;
            range.Text = "Привет Word";
            */
            // Add text between the pictures
            object oCollapseEnd = WdCollapseDirection.wdCollapseEnd;
            Range rng = doc.Paragraphs[1].Range;
            rng.Collapse(ref oCollapseEnd);
            rng.Text = "This is the text between the pictures.";
            rng.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify; // Align the text to the width



            // Сохраняем документ
            object oFileName = saveFileDialog.FileName;
            object oFileFormat = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatDocumentDefault;
            doc.SaveAs2(ref oFileName, ref oFileFormat);

            // Закрываем документ и приложение Word
            ((_Document)doc).Close();
            ((_Application)wordApp).Quit();

            MessageBox.Show("Документ успешно сохранен.");
        }
    }
}

