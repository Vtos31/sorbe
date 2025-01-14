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
using Auth0.ManagementApi.Models.Keys;
using sorbe.Utilities;

namespace sorbe.Pages
{
    /// <summary>
    /// Interaction logic for SearchPage.xaml
    /// </summary>
    public partial class SearchPage : Page
    {
        public SearchPage(string stringSearch)
        {
            InitializeComponent();
            SearchAlbum(stringSearch);
        }
        private async Task SearchAlbum(string stringSearch)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            list = await FireBaseController.Instance.ViewParamData("projects", stringSearch);
            int k = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (k == list.Count - 1)
                {
                    k = 0;
                }
                StackPanel stackPanel = new StackPanel
                {
                    Margin = new Thickness(10, 0, 0, 0),
                    Orientation = Orientation.Vertical,
                    VerticalAlignment = VerticalAlignment.Stretch
                };
                Button button = new Button
                {
                    Width = 200,
                    Height = 200,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Tag = list[i],
                    Content = new Image { Source = Tools.CreateImageFromBase64(list[i]["image"].ToString()), Stretch = Stretch.Fill }
                };
                button.Click += EnterToInfoPage_Click;
                stackPanel.Children.Add(button);
                Label AlbumLabel = new Label
                {
                    FontSize = 18,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Foreground = Brushes.White,
                    Height = 33,
                    Content = list[i]["name"]
                };

                stackPanel.Children.Add(AlbumLabel);
                Label ArtistLabel = new Label
                {
                    FontSize = 10,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Height = 23,
                    Foreground = Brushes.White,
                    Content = list[i]["artist"]
                };
                stackPanel.Children.Add(ArtistLabel);
                SearchPlatform.Children.Add(stackPanel);
                k++;
            }
           
        }
        private void EnterToInfoPage_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            InfoPage infoPage = new InfoPage((Dictionary<string, object>)button.Tag);
            NavigationService?.Navigate(infoPage);
        }
    }
}
