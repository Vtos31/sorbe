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
using Microsoft.Win32;
using sorbe.Utilities;

namespace sorbe.Pages
{
    /// <summary>
    /// Interaction logic for AddAlbumPage.xaml
    /// </summary>
    public partial class AddAlbumPage : Page
    {
        private Dictionary<string,object> genresSelect = new Dictionary<string, object>();
        private List<string> trackList = new List<string>();
        private string imageBase64 = string.Empty;
        public AddAlbumPage()
        {
            InitializeComponent();
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
            Label label = new Label() { Foreground = Brushes.White, FontSize=16  ,Content = SearchBox.Text, Margin = new Thickness(5, 0, 0, 0) };
            genresSelect.Add(SearchBox.Text,1);
            SelectedOptions.Children.Add(label);

            SearchBox.Text = string.Empty;
        }

        private void AddTrack_Click(object sender, RoutedEventArgs e)
        {
            trackList.Add(TrackBox.Text);
            Label label = new Label() { Foreground = Brushes.White, FontSize = 16, Content = TrackBox.Text, Margin = new Thickness(5, 0, 0, 0) };
            TrackList.Children.Add(label);
            TrackBox.Text = string.Empty;
        }

        private void AddAlbum_Click(object sender, RoutedEventArgs e)
        {
            string selectedText = string.Empty;
            if (TypeSelect.SelectedItem is ListBoxItem selectedItem)
            {
                 selectedText = selectedItem.Content.ToString();
            }
            Dictionary<string, object> dict = new Dictionary<string, object>()
            {
                {"artist",ArtistName.Text},
                {"commentcount",0},
                {"image",imageBase64},
                {"name",AlbumName.Text},
                {"rate",0},
                {"tags",genresSelect},
                {"tracklist",trackList},
                {"type",selectedText},
                {"year",Year.Text}
            };
            FireBaseController.Instance.AddData("projects",dict);
        }

        private void ChangeImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Виберіть фото",
                Filter = "Зображення (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                try
                {
                    BitmapImage bitmap = new BitmapImage(new Uri(selectedFilePath));
                    ImageBox.Source = bitmap;
                    imageBase64 = Tools.GetBase64FromImage(selectedFilePath);
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка завантаження зображення: {ex.Message}");
                }
            }
        }
    }
}
