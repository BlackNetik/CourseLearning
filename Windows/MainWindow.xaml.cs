using CourseLearning.Pages;
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
using System.Windows.Shapes;

namespace CourseLearning.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void sideBarProfile_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the profile page
            contentFrame.Navigate(new ProfilePage());
        }

        private void sideBarCreatingCourses_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the creating courses page
            contentFrame.Navigate(new CreatingCoursesPage());
        }

        private void sideBarReadingCourses_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the read courses page
            contentFrame.Navigate(new ReadingCoursesPage());
        }

        //Выход из профиля
        private void sideBarLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow mainWindow = new LoginWindow() { WindowStartupLocation = WindowStartupLocation.CenterScreen };
            this.Close();
            mainWindow.Show();
        }
    }
}
