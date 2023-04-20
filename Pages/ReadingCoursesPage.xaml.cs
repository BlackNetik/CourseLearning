using Microsoft.Win32;
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
    /// Логика взаимодействия для ReadingCoursesPage.xaml
    /// </summary>
    public partial class ReadingCoursesPage : Page
    {
        public ReadingCoursesPage()
        {
            InitializeComponent();

            //Скрытые элементов отображения страницы
            PageReadingLayout.Visibility = Visibility.Collapsed;
        }

        private void NextPageReadingCoursesButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FindFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Json files (*.json)|*.json|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                PageReadingLayout.Visibility = Visibility.Visible;
                PageLoadingLayout.Visibility = Visibility.Collapsed;
            }

        }
    }
}
