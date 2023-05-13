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
                /*
                string courseName = row["course_name"].ToString();
                MessageBox.Show($"The selected course is {courseName}");
                */
                //AddImageAndTextToWordDocument();
                CourseProgress course = new CourseProgress()
                {
                    Id = 0, //Int32.Parse(row["id"].ToString())
                    UserId = ProfileUser.Id,
                    CourseName = row["course_name"].ToString(),
                    Progress = Int32.Parse(row["progress"].ToString()),
                    Status = row["status_course"].ToString()
                };

                CreateCertificate(ProfileUser, course,imagePath);
            }
        }
        
        

        public void CreateCertificate(User user, CourseProgress courseProgress, string imagePath)
        {
            string fileName = $"Certificate_{user.FirstName}_{user.LastName}_{courseProgress.CourseName}.docx";
            using (WordprocessingDocument document = WordprocessingDocument.Create(fileName, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = document.AddMainDocumentPart();
                mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
                Body body = mainPart.Document.AppendChild(new Body());

                // Добавление иконки приложения
                ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);
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


                // Добавление изображения
                //AddImageToBody(body, mainPart, imageId);

                mainPart.Document.Save();
            }
        }

        private void AddImageToBody(Body body, MainDocumentPart mainPart, string imageId)
        {
            DocumentFormat.OpenXml.Wordprocessing.Run run = new DocumentFormat.OpenXml.Wordprocessing.Run();
            RunProperties runProperties = new RunProperties();
            DocumentFormat.OpenXml.Wordprocessing.Drawing drawing = new DocumentFormat.OpenXml.Wordprocessing.Drawing();

            DocumentFormat.OpenXml.Drawing.Wordprocessing.Inline inline = new DocumentFormat.OpenXml.Drawing.Wordprocessing.Inline();
            inline.AnchorId = "anchorId";
            inline.DocProperties = new DocProperties() { Id = 1, Name = "Picture" };
            inline.Extent = new Extent() { Cx = 990000L, Cy = 792000L };

            DocumentFormat.OpenXml.Drawing.Graphic graphic = new DocumentFormat.OpenXml.Drawing.Graphic();
            graphic.GraphicData = new DocumentFormat.OpenXml.Drawing.GraphicData() { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" };

            DocumentFormat.OpenXml.Drawing.Pictures.Picture picture = new DocumentFormat.OpenXml.Drawing.Pictures.Picture();
            picture.BlipFill = new DocumentFormat.OpenXml.Drawing.Pictures.BlipFill();
            picture.BlipFill.Blip = new DocumentFormat.OpenXml.Drawing.Blip() { Embed = imageId };
            picture.BlipFill.SourceRectangle = new DocumentFormat.OpenXml.Drawing.SourceRectangle();
            //picture.BlipFill.Blip.Scratch = new DocumentFormat.OpenXml.Drawing.Stretch();
            //picture.BlipFill.Blip.Stretch.FillRectangle = new DocumentFormat.OpenXml.Drawing.FillRectangle();

            picture.ShapeProperties = new DocumentFormat.OpenXml.Drawing.Pictures.ShapeProperties();
            picture.ShapeProperties.Transform2D = new DocumentFormat.OpenXml.Drawing.Transform2D();
            picture.ShapeProperties.Transform2D.Extents = new DocumentFormat.OpenXml.Drawing.Extents() { Cx = 990000L, Cy = 792000L };
            picture.ShapeProperties.Transform2D.Offset = new DocumentFormat.OpenXml.Drawing.Offset() { X = 0L, Y = 0L };
            //picture.ShapeProperties.PresetGeometry = new DocumentFormat.OpenXml.Drawing.Pictures.PresetGeometry() { Preset = DocumentFormat.OpenXml.Drawing.ShapeTypeValues.Rectangle };
            //picture.ShapeProperties.PresetGeometry.AdjustValueList = new DocumentFormat.OpenXml.Drawing.AdjustValueList();



            graphic.GraphicData.Append(picture);
            //graphic.Append(graphic.GraphicData);
            drawing.Append(inline);
            run.Append(runProperties);
            run.Append(drawing);

            DocumentFormat.OpenXml.Wordprocessing.Paragraph imageParagraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(run);
            imageParagraph.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
            body.AppendChild(imageParagraph);
        }

    }
}

