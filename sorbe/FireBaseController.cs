using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using Google.Cloud.Firestore;


namespace sorbe
{
    internal class FireBaseController
    {
        private FirestoreDb db;

        public FireBaseController()
        {

            string credentialPath = @"C:\Users\Admin\source\repos\sorbe\sorbe\music-servis-firebase-adminsdk-b6st9-6c6990962f.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);


             db = FirestoreDb.Create("music-servis");
        }

        public async Task<List<Dictionary<string, object>>> ViewData()
        {
            Query allAlbumQuery = db.Collection("projects");
            QuerySnapshot allAlbumQuerySnapshot = await allAlbumQuery.GetSnapshotAsync();
            if (allAlbumQuerySnapshot != null)
            {
                List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
                foreach (DocumentSnapshot documentSnapshot in allAlbumQuerySnapshot.Documents)
                {
                    Dictionary<string, object> albums = documentSnapshot.ToDictionary();
                    
                    list.Add(albums);
                }

                return list;
            }
            else
            {
            }
            return new List<Dictionary<string, object>>();
        }
    }
}
