using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;

namespace sorbe
{
    internal class FireBaseController
    {
        public FireBaseController() {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("C:\\Users\\Admin\\source\\repos\\sorbe\\sorbe\\music-servis-firebase-adminsdk-b6st9-09d7bba446.json")
            });

            FirestoreDb db = FirestoreDb.Create("music-servis");
        }
    }
}