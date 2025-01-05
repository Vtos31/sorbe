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
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class InfoPage : Page
    {
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
            foreach (var item in tracks)
            {
                Label track = new Label() { Content = item.ToString(), FontSize = 20, Foreground = Brushes.White };
                ProjecTrackList.Children.Add(track);
            }

           //ChangeWantListenButton(firestoreObject["id"].ToString());
        }

        private async void ChangeWantListenButton(string id)
        {
            var user = await FireBaseController.Instance.ViewData("users", FireBaseController.Instance.Uid);
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
            FireBaseController.Instance.UpdateUserData("users",FireBaseController.Instance.Uid,new Dictionary<string, object> { { "wantlistenproj", new object[] { button.Tag } } });
        }
        private void WantListenDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            FireBaseController.Instance.DeleteUserData("users", FireBaseController.Instance.Uid, new Dictionary<string, object> { { "wantlistenproj", new object[] { button.Tag } } });
        }

    }
}
