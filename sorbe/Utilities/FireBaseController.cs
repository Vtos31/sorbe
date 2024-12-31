using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using Google.Cloud.Firestore;


namespace sorbe.Utilities
{
    internal class FireBaseController
    {
        private static FireBaseController _instance;
        private static readonly object _lock = new object();

        private FirestoreDb db;

        // Приватний конструктор для запобігання створення нових екземплярів
        private FireBaseController()
        {
            string credentialPath = @"C:\Users\Admin\source\repos\sorbe\sorbe\music-servis-firebase-adminsdk-b6st9-6c6990962f.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);

            db = FirestoreDb.Create("music-servis");
        }


        public static FireBaseController Instance
        {
            get
            {
                lock (_lock) 
                {
                    if (_instance == null)
                    {
                        _instance = new FireBaseController();
                    }
                    return _instance;
                }
            }
        }

        public async Task<List<Dictionary<string, object>>> ViewData(string collection)
        {
            Query query = db.Collection(collection);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            if (snapshot != null && snapshot.Documents.Count > 0)
            {
                return snapshot.Documents
                              .Select(doc => doc.ToDictionary())
                              .ToList();
            }
            return new List<Dictionary<string, object>>();
        }
    }

}

