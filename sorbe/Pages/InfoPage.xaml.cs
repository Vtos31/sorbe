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
using Google.Cloud.Firestore.V1;
using sorbe.Utilities;

namespace sorbe
{
    
    public partial class InfoPage : Page
    {
        private List<string> genresSelect = new List<string>();
        private string userimage;
        private string projid;
        public InfoPage(Dictionary<string, object> firestoreObject)
        {
            InitializeComponent();


            ProjectCover.Source = Tools.CreateImageFromBase64(firestoreObject["image"].ToString());
            ProjectName.Content = firestoreObject["name"].ToString();
            ProjectCreator.Content = firestoreObject["artist"].ToString();
            ProjectType.Content += firestoreObject["type"].ToString();
            ProjectYearRelease.Content += firestoreObject["year"].ToString();
            var tags = firestoreObject["tags"] as List<object>; 
            foreach(var item in tags )
            {
                ProjectGenre.Text += item.ToString() + ",";
            }
            WantListenButton.Tag = firestoreObject["id"].ToString();
            var tracks = firestoreObject["tracklist"] as List<object>;
            userimage = firestoreObject["image"].ToString();
            projid = firestoreObject["id"].ToString();
            foreach (var item in tracks)
            {
                Label track = new Label() { Content = item.ToString(), FontSize = 20, Foreground = Brushes.White };
                ProjecTrackList.Children.Add(track);
            }

           
             GetUser(FireBaseController.Instance.Uid);


            
        }
        private async void GetUser(string id)
        {
            Dictionary<string,object> user = await FireBaseController.Instance.ViewData("users", FireBaseController.Instance.Uid);
            ShowUserOnCommentData(id,user);
            ChangeWantListenButton(id,user);
        }
        private async void ShowUserOnCommentData(string id, Dictionary<string, object> user)
        {
           CommentUserName.Content = user["name"].ToString();
            CommentUserImage.Source = Tools.CreateImageFromBase64(user["image"].ToString());
        }

        private async void ChangeWantListenButton(string id, Dictionary<string, object> user)
        {

            if (user.ContainsKey("wantlistenproj"))
            {
                var wantlistenproj = user["wantlistenproj"] as List<object>;
                if (wantlistenproj.Contains(id))
                {
                    WantListenButton.Click -= WantListenButton_Click;
                    WantListenButton.Click += WantListenDeleteButton_Click;
                    WantListenButton.Content = "Видалити";
                }
                else
                {
                    WantListenButton.Click += WantListenButton_Click;
                    WantListenButton.Click -= WantListenDeleteButton_Click;
                    WantListenButton.Content = "Хочу послухати";
                }
            }
        }
        private void WantListenButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            WantListenButton.Click -= WantListenButton_Click;
            WantListenButton.Click += WantListenDeleteButton_Click;
            WantListenButton.Content = "Видалити";
            FireBaseController.Instance.UpdateUserData("users",FireBaseController.Instance.Uid,new Dictionary<string, object> { { "wantlistenproj", new object[] { button.Tag } } });
        }
        private void WantListenDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            WantListenButton.Click += WantListenButton_Click;
            WantListenButton.Click -= WantListenDeleteButton_Click;
            WantListenButton.Content = "Хочу послухати";
            FireBaseController.Instance.DeleteUserData("users", FireBaseController.Instance.Uid, new Dictionary<string, object> { { "wantlistenproj", new object[] { button.Tag } } });
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
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                return;
            }
            Label label = new Label() { Foreground = Brushes.White,Content=SearchBox.Text , Margin = new Thickness(5,0,0,0)};
            genresSelect.Add(SearchBox.Text);   
            SelectedOptions.Children.Add(label);

            SearchBox.Text = string.Empty;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Comment.Text) ||
             string.IsNullOrWhiteSpace(CommentRate.Text) ||
             !int.TryParse(CommentRate.Text, out int rate) || rate < 0 || rate > 100 ||
             genresSelect == null || genresSelect.Count == 0)
            {
                CommentError.Visibility = Visibility.Visible;
                CommentError.Content = "Ви не заповнили усі поля або заповнили їх не коректно";
                return;
            }
            FireBaseController.Instance.AddData("comments", new Dictionary<string, object> { { "projectId", projid }, { "username", CommentUserName.Content }, { "userimage", userimage}, { "comment", Comment.Text }, { "genres", genresSelect }, { "rate",CommentRate.Text } });
            Comment.Text = string.Empty;
            CommentRate.Text = string.Empty;
            genresSelect.Clear();
            CommentError.Visibility = Visibility.Collapsed;
            SelectedOptions.Children.Clear();
        }
    }
}
