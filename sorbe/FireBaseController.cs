using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            Console.WriteLine("Connected to Firestore!");

        }

        public async Task<Dictionary<string,object>> ViewData()
        {
            DocumentReference docRef = db.Collection("music-serves-content").Document("projects");
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                Dictionary<string, object> data = snapshot.ToDictionary();
                return data;
            }
            else
            {
                Console.WriteLine("Document does not exist!");
            }
            return new Dictionary<string, object>();
        }
    }
}
