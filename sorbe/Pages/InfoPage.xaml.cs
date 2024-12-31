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

            var tracks = firestoreObject["tracklist"] as List<object>;
            foreach (var item in tracks)
            {
                Label track = new Label() { Content = item.ToString(), FontSize = 20, Foreground = Brushes.White };
                ProjecTrackList.Children.Add(track);
            }
        }
    }
}
