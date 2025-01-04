
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
using System.Security.Cryptography;
using System.Text.Json;
using static Google.Rpc.Context.AttributeContext.Types;
using Newtonsoft.Json.Linq;



namespace sorbe.Utilities
{
    internal class FireBaseController
    {
        private static readonly string apiKey = "AIzaSyCOZ6S6Wnd7t3_Pc_YN7p3q1W7I7oppqtA";
        private static readonly string signInUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=" + apiKey;
        private string signUpUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=" + apiKey;

        private static FireBaseController _instance;
        private static readonly object _lock = new object();
        private string _uid;
        private FirestoreDb db;

        public string Uid
        {
            get => _uid;
            private set => _uid = value; 
        }

        public async Task InitializeUidAsync()
        {
            Uid = await AutoAuth();
        }
        private FireBaseController()
        {
            string credentialPath = @"C:\Users\Admin\source\repos\sorbe\sorbe\music-servis-firebase-adminsdk-b6st9-53a16acc08.json";
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

        public async Task UserAuth(string email, string password)
        {
            using (var client = new HttpClient())
            {
                var data = new
                {
                    email = email,
                    password = password,
                    returnSecureToken = true
                };

                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(signInUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {

                    MessageBox.Show(responseString);
                    File.WriteAllText(@"C:\Users\Admin\source\repos\sorbe\sorbe\userdata.txt", string.Empty);
                    var jsonResponse = JsonConvert.DeserializeObject<SignInClass>(responseString);
                    File.WriteAllText(@"C:\Users\Admin\source\repos\sorbe\sorbe\userdata.txt", jsonResponse.idToken);
                }
                else
                {
                    Console.WriteLine("Error: " + responseString);
                }
            }
        }
        public async Task<string> AutoAuth()
        {
            string idToken = File.ReadAllText("C:\\Users\\Admin\\source\\repos\\sorbe\\sorbe\\userdata.txt");
            if(idToken == null)
            {
                MessageBox.Show("User not authenticated!");
                return null;
            }

            try
            {
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);

               
                string uid = decodedToken.Uid;
                MessageBox.Show("You just got work done, hit me when church done\r\nThis ain't big pharmacy but fuckin' Yeezy got perks, huh?\r\nYou do what you want now, like you Lil Uzi Vert or some'\r\nShe givin' me FaceTime, even when we in person\r\nI want you dressed up, so I can undress ya\r\nThe money ain't small, but we still at the Webster\r\nShe got some girlfriends, we fucked on the best one\r\nI got a confession, I'm on another run\r\n\r\n[Chorus: Both, Kanye West & Talmadge Armstrong]\r\n(See)\r\nOoh-wee\r\nOoh-wee (So I can see)\r\nOoh-wee\r\nCan see (Ooh-wee, so I can see)\r\nOoh-wee\r\nOoh-wee (So I can see)\r\nOoh-wee, ooh—\r\n\r\n[Refrain: Talmadge Armstrong & Ty Dolla $ign]\r\nSlip off your dress, baby, so I can s—\r\nSlip off your dress, baby, so I can s— (Run it, r");
                return uid;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            return null;
        }
        public async Task UserRegistration(string email, string password,string name)
        {
            using (var client = new HttpClient())
            {
                var data = new
                {
                    email = email,
                    password = password,
                    returnSecureToken = true
                };

                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(signUpUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {


                    var jsonResponse = JsonConvert.DeserializeObject<SignInClass>(responseString);
                    File.WriteAllText(@"C:\Users\Admin\source\repos\sorbe\sorbe\userdata.txt", string.Empty);
                    File.WriteAllText(@"C:\Users\Admin\source\repos\sorbe\sorbe\userdata.txt", jsonResponse.idToken);
                    MessageBox.Show("User registered successfully!");
                    FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(jsonResponse.idToken);
                    string uid = decodedToken.Uid;
                    Google.Cloud.Firestore.DocumentReference docRef = db.Collection("users").Document(uid);
                    Dictionary<string, object> city = new Dictionary<string, object>
                    {
                        { "email", email },
                        { "name", name },
                        { "image", " " },
                        { "rateproj", new string[]{""} },
                        { "wantlistenproj", new string[]{""} },
                        { "favgenre", "" }

                    };
                    await docRef.SetAsync(city);
                }
                else
                {
                    Console.WriteLine("Error: " + responseString);
                    MessageBox.Show($"Error: {responseString}");
                }
            }
        }


    }
}




