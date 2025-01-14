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
            if(ProjectName.Content.ToString().Length > 9)
            {
                ProjectName.FontSize = 40;
            }
            ProjectCreator.Content = firestoreObject["artist"].ToString();
            ProjectType.Content += firestoreObject["type"].ToString();
            ProjectYearRelease.Content += firestoreObject["year"].ToString();
            var tags = firestoreObject["tags"] as Dictionary<string,object>; 
            foreach(var item in tags )
            {
                Label label = new Label() { Content = "     "+item.Key+" "+item.Value.ToString(), FontSize = 20, Foreground = Brushes.White };
                Tags.Children.Add(label);
            }
            WantListenButton.Tag = firestoreObject["id"].ToString();
            var tracks = firestoreObject["tracklist"] as List<object>;
            projid = firestoreObject["id"].ToString();
            foreach (var item in tracks)
            {
                Label track = new Label() { Content = item.ToString(), FontSize = 20, Foreground = Brushes.White };
                ProjecTrackList.Children.Add(track);
            }
            
             GetUser(projid,firestoreObject);
             getComment();

        }

        private async Task getComment()
        {
            
            List<Dictionary<string, object>> d = await FireBaseController.Instance.ViewData("comments", "projectId", projid.ToString(),5);
            List < Dictionary<string, object>> dcheck = d.Where(d => d["useruid"].ToString() == FireBaseController.Instance.Uid).ToList();
            if (dcheck.Count > 0)
            {
                CreateComment.Visibility = Visibility.Collapsed;
            }
            foreach (var item in d)
            {
                Dictionary<string, object> user = await FireBaseController.Instance.ViewData("users", item["useruid"].ToString());
                Comments.Children.Add(CreateCustomBorder(item,user));
            }

        }
        private async void GetUser(string id, Dictionary<string, object> firebaseObject)
        {
            Random random = new Random();
            Dictionary<string,object> user = await FireBaseController.Instance.ViewData("users", FireBaseController.Instance.Uid);
            string bestGenre = "";
            if (firebaseObject["tags"] is Dictionary<string, object> tags)
            {
                bestGenre = tags
            .Where(t => int.TryParse(t.Value.ToString(), out _))
            .OrderByDescending(t => Convert.ToInt32(t.Value))
            .FirstOrDefault().Key;
            }
            if ( user["favgenre"] is List<object> favgenre)
            {
                if(bestGenre != null && !favgenre.Contains(bestGenre))
                {
                    favgenre[random.Next(0, favgenre.Count)] = bestGenre;
                    await FireBaseController.Instance.UpdateData("users", FireBaseController.Instance.Uid, new Dictionary<string, object> { { "favgenre", favgenre } });
                }

            }

            userimage = user["image"].ToString();

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
                wantlistenproj = wantlistenproj;
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
            FireBaseController.Instance.UpdateData("users",FireBaseController.Instance.Uid,new Dictionary<string, object> { { "wantlistenproj", new object[] { button.Tag } } });
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
            if (genresSelect.Count == 5)
            {
                MessageBox.Show("Ви вже вибрали 5 жанрів");
                return;
            }
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                return;
            }
            Label label = new Label() { Foreground = Brushes.White,Content=SearchBox.Text , Margin = new Thickness(5,0,0,0)};
            genresSelect.Add(SearchBox.Text);   
            SelectedOptions.Children.Add(label);

            SearchBox.Text = string.Empty;
        }

        private async void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Comment.Text) ||
             string.IsNullOrWhiteSpace(CommentRate.Text) ||
             !int.TryParse(CommentRate.Text, out int rate) || rate < 0 || rate > 100 ||
             genresSelect == null || genresSelect.Count == 0)
            {
                CommentError.Visibility = Visibility.Visible;
                CommentError.Content = "Ви заповнили не усі поля або заповнили їх не коректно";
                return;
            }


            FireBaseController.Instance.AddData("comments", new Dictionary<string, object> { { "projectId", projid }, { "useruid",FireBaseController.Instance.Uid}, { "comment", Comment.Text }, { "genres", genresSelect }, { "rate", CommentRate.Text } });
            FireBaseController.Instance.UpdateData("users", FireBaseController.Instance.Uid, new Dictionary<string, object> { { "rateproj", new object[] { projid } } });

            foreach (var item in genresSelect)
            {
               await FireBaseController.Instance.UpdateGenreAsync(projid,item);
            }

            FireBaseController.Instance.UpdateProjectRate("projects", projid, Convert.ToInt32(CommentRate.Text));
            Comment.Text = string.Empty;
            CommentRate.Text = string.Empty;
            genresSelect.Clear();
            CommentError.Visibility = Visibility.Collapsed;
            SelectedOptions.Children.Clear();

            InfoPage infoPage = new InfoPage(await FireBaseController.Instance.ViewData("projects", projid));
            NavigationService.Navigate(infoPage);
        }

        public static Border CreateCustomBorder(Dictionary<string,object> comment, Dictionary<string, object> user)
        {
            
            var border = new Border
            { 
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Background = new SolidColorBrush(Color.FromRgb(26, 26, 26)),
                Width = 1000,
                Margin = new Thickness(0, 10, 0, 0),
                VerticalAlignment = VerticalAlignment.Stretch,  
            };

            
            var mainStackPanel = new StackPanel();

           
            var grid = new Grid();

            
            var image = new Image
            {
                Source = Tools.CreateImageFromBase64(user["image"].ToString()),
                Margin = new Thickness(11, 0, 0, 0),
                Width = 95,
                Height = 95,
                Stretch = Stretch.Fill,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            grid.Children.Add(image);

            
            var textStackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(106, 10, 0, 0),
                VerticalAlignment = VerticalAlignment.Top
            };

            var nameLabel = new Label
            {
                Content = user["name"].ToString(),
                FontSize = 50,
                Foreground = Brushes.White,
                Height = 70
            };

            var scoreLabel = new Label
            {
                Content = comment["rate"] +"/100",
                FontSize = 36,
                Foreground = Brushes.White,
                Height = 55,
                Width = 146,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center
            };

            textStackPanel.Children.Add(nameLabel);
            textStackPanel.Children.Add(scoreLabel);
            grid.Children.Add(textStackPanel);


            mainStackPanel.Children.Add(grid);

            var textBlockBorder = new Border
            {
                BorderBrush = new SolidColorBrush(Color.FromRgb(86, 86, 86)),
                BorderThickness = new Thickness(1),
                Background = new SolidColorBrush(Color.FromRgb(28, 28, 28)),
                Margin = new Thickness(0, 10, 0, 0),
                Width = 955,
                Height = 132
            };

            var textBlock = new TextBlock
            {
                Text = comment["comment"].ToString(),
                TextWrapping = TextWrapping.Wrap,
                FontSize = 24,
                Foreground = Brushes.White,
                Padding = new Thickness(5)
            };

            textBlockBorder.Child = textBlock;
            mainStackPanel.Children.Add(textBlockBorder);

            var genreStackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(10,0, 0, 0)
            };
            foreach (var genre in comment["genres"] as List<object>)
            {
                var genreLabel = new Label
                {
                    Content = genre.ToString(),
                    FontSize = 18,
                    Foreground = Brushes.White,
                    Margin = new Thickness(5, 0, 0, 0)
                };
                genreStackPanel.Children.Add(genreLabel);
            }
            mainStackPanel.Children.Add(genreStackPanel);






            border.Child = mainStackPanel;

            return border;
        }
    }
}
