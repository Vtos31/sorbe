﻿
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.IO;
using Google.Cloud.Firestore;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin.Auth;
using Auth0.ManagementApi.Models;
using Firebase.Auth;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Windows;



namespace sorbe.Utilities
{
    internal class FireBaseController
    {
        private static FireBaseController _instance;
        private static readonly object _lock = new object();

        private FirestoreDb db;


        private FireBaseController()
        {
            string credentialPath = @"C:\Users\Admin\source\repos\sorbe\sorbe\mistery\music-servis-firebase-adminsdk-b6st9-53a16acc08.jsongi";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);
            db = FirestoreDb.Create("music-servis");

            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(credentialPath),
            });

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
        
       
        public async Task UserRegistration(string email,string password,string username)
        {
            try
            {
                var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs()
                {
                    Email = email,
                    Password = password,
                    DisplayName = username
                });
            }
            catch (Exception ex)
            {

            }
        }
    }


}

