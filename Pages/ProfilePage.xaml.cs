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
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;

namespace CourseLearning.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : System.Windows.Controls.Page
    {
        //private ProfilePageViewModel viewModel;
        string ConnectionToBD = "Server=localhost; port=5432; user id=postgres; password=password; database=courselearning;";
        string imagePath = @"C:\Users\User\Desktop\c#\CourseLearning\Images\OutputWord\CourseLearning.jpg";
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
                CourseProgress course = new CourseProgress()
                {
                    Id = 0, //Int32.Parse(row["id"].ToString())
                    UserId = ProfileUser.Id,
                    CourseName = row["course_name"].ToString(),
                    Progress = Int32.Parse(row["progress"].ToString()),
                    Status = row["status_course"].ToString()
                };

                CreateCertificate(ProfileUser, course, imagePath);
            }
        }


        public void CreateCertificate(User user, CourseProgress courseProgress, string imagePath)
        {
            // Открываем проводник и предлагаем выбрать путь для Word файла
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Word Documents (*.docx)|*.docx";
            saveFileDialog.DefaultExt = "docx";
            saveFileDialog.AddExtension = true;
            saveFileDialog.FileName = $"Сертификат '{courseProgress.CourseName}' {user.FirstName} {user.LastName}.docx";
            if (saveFileDialog.ShowDialog() != true)
            {
                return;
            }

            //string fileName = $"Сертификат_{user.FirstName}_{user.LastName}_{courseProgress.CourseName}.docx";
            using (WordprocessingDocument document = WordprocessingDocument.Create(saveFileDialog.FileName, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = document.AddMainDocumentPart();
                mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
                Body body = mainPart.Document.AppendChild(new Body());

                // Добавление иконки приложения
                ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Png);
                using (FileStream stream = new FileStream(imagePath, FileMode.Open))
                {
                    imagePart.FeedData(stream);
                }

                string imageId = mainPart.GetIdOfPart(imagePart);

                // Добавление содержимого сертификата
                DocumentFormat.OpenXml.Wordprocessing.Paragraph titleParagraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text("Сертификат о прохождении курса")));
                titleParagraph.ParagraphProperties = new DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties(new DocumentFormat.OpenXml.Wordprocessing.Justification() { Val = DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Center });
                body.AppendChild(titleParagraph);

                DocumentFormat.OpenXml.Wordprocessing.Paragraph courseNameParagraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text($"Курс: {courseProgress.CourseName}")));
                body.AppendChild(courseNameParagraph);

                DocumentFormat.OpenXml.Wordprocessing.Paragraph userNameParagraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text($"Имя: {user.FirstName} {user.LastName}")));
                body.AppendChild(userNameParagraph);

                DocumentFormat.OpenXml.Wordprocessing.Paragraph progressParagraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text($"Процент прохождения: {courseProgress.Progress}%")));
                body.AppendChild(progressParagraph);


                mainPart.Document.Save();
                
            }

            // Добавление изображения
            AddImageToFile(saveFileDialog, imagePath);

            MessageBox.Show("Готово!");
        }

        public void AddImageToFile(SaveFileDialog saveFile, string imagePath)
        {
            // Создаем новое приложение Word
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();

            // Открываем документ
            Microsoft.Office.Interop.Word.Document doc = wordApp.Documents.Open(saveFile.FileName);
            // Добавляем новый параграф в конец документа
            doc.Paragraphs.Add();

            // Добавляем изображение на новую строку
            object oMissed = doc.Paragraphs[doc.Paragraphs.Count].Range;
            object oLinkToFile = false;
            object oSaveWithDocument = true;
            object oRange = doc.Paragraphs[doc.Paragraphs.Count].Range;
            Microsoft.Office.Interop.Word.InlineShape inlineShape = doc.InlineShapes.AddPicture(imagePath, ref oLinkToFile, ref oSaveWithDocument, ref oRange);
            inlineShape.Width = 100;
            inlineShape.Height = 100;

            // Выравниваем изображение по центру
            inlineShape.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;

            // Сохраняем документ
            doc.Save();

            // Закрываем документ и приложение Word
            ((_Document)doc).Close();
            ((_Application)wordApp).Quit();
        }

        
    }
}

