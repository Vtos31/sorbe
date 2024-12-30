﻿using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace sorbe
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FireBaseController fireBaseController = new FireBaseController();
            LoadDataAsync(fireBaseController);


        }
        private async void LoadDataAsync(FireBaseController fireBaseController)
        {
            List<Dictionary<string, object>> list = await fireBaseController.ViewData();
            MainPage mainPage = new MainPage(list);
            Main.Content = mainPage;
        }
        public void EnterToInfoPage_Click(object sender, RoutedEventArgs e)
        {
            InfoPage InfoPage = new InfoPage();
            Main.Content = InfoPage;
        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            AcountEnterPanel.Visibility = Visibility.Visible;
            AcountEnterTitle.Content = "Реєстрація";
            NonLogInPole.Visibility = Visibility.Visible;
            NonLogInText.Visibility = Visibility.Visible;
        }

        private void AcountEnterPanelExit_Click(object sender, RoutedEventArgs e)
        {
            AcountEnterPanel.Visibility = Visibility.Collapsed;
        }

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            AcountEnterPanel.Visibility = Visibility.Visible;
            AcountEnterTitle.Content = "Авторизація";
            NonLogInPole.Visibility = Visibility.Collapsed;
            NonLogInText.Visibility = Visibility.Collapsed;
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            FireBaseController fireBaseController = new FireBaseController();
            LoadDataAsync(fireBaseController);

        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {

            ProfilePage profilePage = new ProfilePage();
            Main.Content = profilePage;
        }
    }

}