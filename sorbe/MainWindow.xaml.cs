using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using sorbe.Pages;
using sorbe.Utilities;
using static Google.Apis.Requests.BatchRequest;

namespace sorbe
{

    public partial class MainWindow : Window
    {

        private List<string> genresSelect = new List<string>();
        public MainWindow()
        {
           
            InitializeComponent();
            _ = InitializeAppAsync();

        }
        private async Task InitializeAppAsync()
        {
           await FireBaseController.Instance.InitializeUidAsync(); 

            if (FireBaseController.Instance.Uid != null)
            {
                Profile.Visibility = Visibility.Visible;
                Exit.Visibility = Visibility.Visible;
                HomeButton.Visibility = Visibility.Visible;
                AddAlbumButton.Visibility = Visibility.Visible;
                SearchButton.Visibility = Visibility.Visible;
                SearchTextBox.Visibility = Visibility.Visible;
                LoadDataAsync();
            }
            else
            {

                ButtonLogInPanelOpen.Visibility = Visibility.Visible;
                ButtonSignInPanelOpen.Visibility = Visibility.Visible;
                AcountEnterPanel.Visibility = Visibility.Visible;
                SuggestionsListBox.Visibility = Visibility.Visible;
            }
            
        }
        private async void LoadDataAsync()
        {
            Random random = new Random();
            List<Dictionary<string, object>> dict = await FireBaseController.Instance.ViewData("projects");
            Dictionary<string, List<Dictionary<string, object>>> listToTopic = new Dictionary<string, List<Dictionary<string, object>>>(){ 
              {"Нові альбоми", new List<Dictionary<string, object>>() },
              {"Вам може сподобатись", new List<Dictionary<string, object>>() },
              {"Альбоми 2020-тих", new List<Dictionary<string, object>>() },
              {"Спробуйте щось нове", new List<Dictionary<string, object>>() },
              {"Всім подобається", new List<Dictionary<string, object>>() },
              {"Класика жанру", new List<Dictionary<string, object>>()  }
            };
            if(dict == null || dict.Count == 0)
                return;
            foreach (string item in TopicPreset.Topics)
            {
                switch (item)
                {
                    case "Нові альбоми":
                        {
                            foreach (Dictionary<string, object> topic in dict)
                                if (Convert.ToInt32(topic["year"]) == DateTime.Now.Year)
                                    listToTopic["Нові альбоми"].Add(topic);
                            break;
                        }
                        
                    case "Вам може сподобатись":
                        {
                            Dictionary<string, object> cleareUserGenre = await FireBaseController.Instance.ViewData("users", FireBaseController.Instance.Uid, new List<string> { "favgenre" });
                            Dictionary<string, List<string>> usergenre = cleareUserGenre.ToDictionary(x => x.Key, x => x.Value as List<string>);

                            foreach (Dictionary<string, object> topic in dict)
                            {
                                if (topic["tags"] is Dictionary<string, object> tags)
                                {
                                    if (usergenre.ContainsKey("favgenre") && usergenre["favgenre"].Any(fav => tags.ContainsKey(fav)))
                                    {
                                        listToTopic["Вам може сподобатись"].Add(topic);
                                    }
                                }
                            }
                            break;
                        }
                        
                    case "Альбоми 2020-тих":
                        foreach (Dictionary<string, object> topic in dict)
                            if (Convert.ToInt32(topic["year"]) > 2020)
                                listToTopic["Альбоми 2020-тих"].Add(topic);
                        break;
                    case "Спробуйте щось нове":
                        for(int i = 0; i < 10; i++)
                        {
                            
                            var d = dict[random.Next(0, dict.Count)];
                            if(!listToTopic["Спробуйте щось нове"].Contains(d))
                                listToTopic["Спробуйте щось нове"].Add(d);
                        }
                        break;
                    case "Всім подобається":
                        foreach (Dictionary<string, object> topic in dict)
                            if (Convert.ToInt32(topic["rate"]) > 70)
                                listToTopic["Всім подобається"].Add(topic);
                        break;
                    case "Класика жанру":
                        foreach (Dictionary<string, object> topic in dict)
                            if (Convert.ToInt32(topic["rate"]) > 90 && Convert.ToInt32(topic["commentcount"]) > 70)
                                listToTopic["Класика жанру"].Add(topic);
                        break;
                }

            }

  
            MainPage mainPage = new MainPage(listToTopic);
            Main.Content = mainPage;
        }
        private async Task<Dictionary<string, object>> LoadUserDataAsync()
        {
            Dictionary<string, object> user = await FireBaseController.Instance.ViewData("users", FireBaseController.Instance.Uid);
            return user;
        }

        private void ButtonSignIn_Click(object sender, RoutedEventArgs e)
        {
            AcountEnterPanel.Visibility = Visibility.Visible;
            AcountEnterTitle.Content = "Реєстрація";
            TextboxPasswordCheck.Visibility = Visibility.Visible;
            NonLogInText.Visibility = Visibility.Visible;
            DbLogButton.Visibility = Visibility.Collapsed;                                         
            DbSignButton.Visibility = Visibility.Visible;
            ButtonShow.Visibility = Visibility.Visible;
            Name.Visibility = Visibility.Visible;
            TextboxName.Visibility = Visibility.Visible;

            SuggestionsListBox.Visibility = Visibility.Visible;
            SelectedOptions.Visibility = Visibility.Visible;
            AddGenre.Visibility = Visibility.Visible;
            SearchBox.Visibility = Visibility.Visible;
            GenreLabel.Visibility = Visibility.Visible;

            TextboxPasswordCheckShow.Text = "";
            TextboxPasswordShow.Text = "";
            TextboxEmail.Text = "";
            TextboxPassword.Password = "";
            TextboxPasswordCheck.Password = "";
            TextboxName.Text = "";
        }


        private void ButtonLogIn_Click(object sender, RoutedEventArgs e)
        {
            AcountEnterPanel.Visibility = Visibility.Visible;
            AcountEnterTitle.Content = "Авторизація";
            TextboxPasswordCheck.Visibility = Visibility.Collapsed;
            NonLogInText.Visibility = Visibility.Collapsed;
            DbLogButton.Visibility = Visibility.Visible;
            DbSignButton.Visibility = Visibility.Collapsed;
            ButtonShow.Visibility = Visibility.Collapsed;
            Name.Visibility = Visibility.Collapsed;
            TextboxName.Visibility = Visibility.Collapsed;

            SuggestionsListBox.Visibility = Visibility.Collapsed;
            SelectedOptions.Visibility = Visibility.Collapsed;
            AddGenre.Visibility = Visibility.Collapsed;
            SearchBox.Visibility = Visibility.Collapsed;
            GenreLabel.Visibility = Visibility.Collapsed;

            TextboxPasswordCheckShow.Text = "";
            TextboxPasswordShow.Text = "";
            TextboxEmail.Text = "";
            TextboxPassword.Password = "";
            TextboxPasswordCheck.Password = "";
            
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            LoadDataAsync();
        }

        private async void Profile_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, object> user = await LoadUserDataAsync();
            ProfilePage profilePage = new ProfilePage(user);
            Main.Content = profilePage;

        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Visibility = Visibility.Collapsed;
            if (TextboxPasswordCheck.Password == TextboxPassword.Password)
            {
                FireBaseController.Instance.UserRegistration(TextboxEmail.Text, TextboxPassword.Password, TextboxName.Text,genresSelect);

            }
            else
            {
                ErrorLabel.Visibility = Visibility.Visible;
                ErrorLabel.Content = "Паролі не співпадають";
            }
        }

        private void ShowPassword_Click(object sender, RoutedEventArgs e)
        {
            if (TextboxPasswordShow.Visibility == Visibility.Visible)
            {
                TextboxPasswordShow.Visibility = Visibility.Collapsed;
                TextboxPassword.Password = TextboxPasswordShow.Text;
                TextboxPassword.Visibility = Visibility.Visible;
            }
            else
            {
                TextboxPassword.Visibility = Visibility.Collapsed;
                TextboxPasswordShow.Text = TextboxPassword.Password;
                TextboxPasswordShow.Visibility = Visibility.Visible;
            }
        }

        private void ShowPasswordCheck_Click(object sender, RoutedEventArgs e)
        {
            if (TextboxPasswordCheckShow.Visibility == Visibility.Visible)
            {
                TextboxPasswordCheckShow.Visibility = Visibility.Collapsed ;
                TextboxPasswordCheck.Password = TextboxPasswordCheckShow.Text;
                TextboxPasswordCheck.Visibility = Visibility.Visible;
            }
            else
            {
                TextboxPasswordCheck.Visibility = Visibility.Collapsed;
                TextboxPasswordCheckShow.Text = TextboxPasswordCheck.Password;
                TextboxPasswordCheckShow.Visibility = Visibility.Visible;
            }  
        }

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            if(TextboxPassword.Visibility == Visibility.Collapsed)
                FireBaseController.Instance.UserAuth(TextboxEmail.Text, TextboxPasswordShow.Text);
            else
                FireBaseController.Instance.UserAuth(TextboxEmail.Text, TextboxPassword.Password);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {

            string exePath = Process.GetCurrentProcess().MainModule.FileName;
            Application.Current.Shutdown();
            Process.Start(exePath);
            string relativePath = @"userdata.txt";
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            File.WriteAllText(path, string.Empty);
        }
        private async void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = SearchBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(query))
            {
                SuggestionsListBox.Visibility = Visibility.Collapsed;
                return;
            }

            var filteredGenres = await Task.Run(() =>
            {
                return MusicGenreList.Genres
                    .Where(g => g.StartsWith(query, StringComparison.OrdinalIgnoreCase))
                    .Take(100)
                    .ToList();
            });

            SuggestionsListBox.ItemsSource = filteredGenres;
            SuggestionsListBox.Visibility = filteredGenres.Any() ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SuggestionsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SuggestionsListBox.SelectedItem is string selectedGenre)
            {
                SearchBox.Text = selectedGenre;
                SuggestionsListBox.Visibility = Visibility.Collapsed;
            }
        }

        private void AddGenre_Click(object sender, RoutedEventArgs e)
        {
            if(genresSelect.Count == 5)
            {
                MessageBox.Show("Ви вже вибрали 5 жанрів");
                return;
            }
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                return;
            }
            Label label = new Label() { Foreground = Brushes.White, Content = SearchBox.Text, Margin = new Thickness(5, 0, 0, 0) };
            genresSelect.Add(SearchBox.Text);
            SelectedOptions.Children.Add(label);

            SearchBox.Text = string.Empty;
        }

        private void AddAlbumButton_Click(object sender, RoutedEventArgs e)
        {
            AddAlbumPage addAlbumPage = new AddAlbumPage();
            Main.Content = addAlbumPage;
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            SearchPage searchPage = new SearchPage(SearchTextBox.Text);
            Main.Content = searchPage;
        }
    }

}