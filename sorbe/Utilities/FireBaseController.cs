
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
using DocumentReference = Google.Cloud.Firestore.DocumentReference;
using Google.Apis.Auth.OAuth2.Responses;
using System.ComponentModel;



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
                                 .Select(doc =>
                                 {
                                     var data = doc.ToDictionary();
                                     data["id"] = doc.Id;
                                     return data;
                                 })
                                 .ToList();
            }
            return new List<Dictionary<string, object>>();
        }
        public async Task<Dictionary<string, object>> ViewData(string collection,string document)
        {
            try
            {
                DocumentSnapshot snapshot = await db.Collection(collection).Document(document).GetSnapshotAsync();

                if (snapshot != null && snapshot.Exists)
                {
                    var data = snapshot.ToDictionary();
                    data["id"] = snapshot.Id; 
                    return data;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
            return new Dictionary<string, object>();
        }
       
       
        public async Task<Dictionary<string, object>> ViewData(string collection, string document,List<string> values)
        {
            DocumentReference docRef = db.Collection(collection).Document(document);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                for (int i = 0; i < values.Count; i++)
                {
                    try
                    {
                        data.Add(values[i], snapshot.GetValue<string>(values[i]));
                    }
                    catch (Exception ex)
                    {
                        data.Add(values[i], snapshot.GetValue<List<string>>(values[i]));
                    }
                }
                return data;

            }
            else
            {
                return new Dictionary<string, object>();
            }
        }
        public async Task<List<Dictionary<string, object>>> ViewData(string collectionName, string fieldName, string valueToSearch,int maxCount)
        {
            try
            {
                CollectionReference collection = db.Collection(collectionName);
                Query query = collection.WhereEqualTo(fieldName, valueToSearch);
                List<Dictionary<string, object>> L = new List<Dictionary<string, object>>();
                QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

                if (querySnapshot.Documents.Count > 0)
                {
                    foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
                    {
                        if(L.Count < maxCount)
                        {
                            L.Add(documentSnapshot.ToDictionary());
                        }
                        else
                        {
                            return L;
                        }
                    }
                    return L;
                }
                else
                {
                    return new List<Dictionary<string, object>>();
                }
            }
            catch (Exception e)
            {
                return new List<Dictionary<string, object>>();
            }
        }
        public async Task UpdateGenreAsync(string document, string newGenre)
        {
            DocumentReference docRef = db.Collection("projects").Document(document);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                return;
            }

            Dictionary<string, object> data = snapshot.ToDictionary();
            Dictionary<string, object> tags = data.ContainsKey("tags") && data["tags"] is Dictionary<string, object> existingTags
                ? existingTags
                : new Dictionary<string, object>();

            
            if (tags.ContainsKey(newGenre))
            {
                int i = Convert.ToInt32(tags[newGenre].ToString());
                tags[newGenre] = i + 1;
                await docRef.UpdateAsync(new Dictionary<string, object> { { "tags", tags } });
            }
            else
            {
                tags.Add(newGenre,1);
                await docRef.UpdateAsync(new Dictionary<string, object> { { "tags", tags } });
            }

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
                    File.WriteAllText(@"C:\Users\Admin\source\repos\sorbe\sorbe\userdata.txt", jsonResponse.refreshToken);
                }
                else
                {
                    MessageBox.Show("Error: " + responseString);
                }
            }
        }
        public async Task<string> AutoAuth()
        {
            HttpClient client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", File.ReadAllText(@"C:\Users\Admin\source\repos\sorbe\sorbe\userdata.txt"))
            });

            HttpResponseMessage response = await client.PostAsync("https://securetoken.googleapis.com/v1/token?key="+apiKey, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenData = JsonConvert.DeserializeObject<TokenResponse>(responseContent);
            try
            {
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(tokenData.IdToken);

                string uid = decodedToken.Uid;
                return uid;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            return null;
        }
        public async Task UpdateData(string collection, string document, Dictionary<string, object> data)
        {
            DocumentReference docRef = db.Collection(collection).Document(document);
            if ((data.ContainsKey("wantlistenproj") && data["wantlistenproj"] is IEnumerable<object> newValues))
            {
                await docRef.UpdateAsync(new Dictionary<string, object>
                {
                    { "wantlistenproj", FieldValue.ArrayUnion(newValues.ToArray()) }
                });

                data.Remove("wantlistenproj");

                await docRef.UpdateAsync(data);
            }
            if(data.ContainsKey("rateproj") && data["rateproj"] is IEnumerable<object> newValue)
            {
                await docRef.UpdateAsync(new Dictionary<string, object>
                {
                    { "rateproj", FieldValue.ArrayUnion(newValue.ToArray()) }
                });

                data.Remove("rateproj");
                await docRef.UpdateAsync(data);
            }
            else
            {
               await docRef.UpdateAsync(data);
            }
        }
        public async Task UpdateProjectRate(string collection, string document, int rate)
        {
            DocumentReference docRef = db.Collection(collection).Document(document);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            Dictionary<string,object> d = snapshot.ToDictionary();
            if (d.ContainsKey("rate"))
            {
                int i = Convert.ToInt32(d["rate"].ToString());
                int ic = Convert.ToInt32(d["commentcount"].ToString());
                int ik = ((i*ic) + rate);
                d["rate"] = ik / (ic + 1);
                d["commentcount"] = ic + 1;
                await docRef.UpdateAsync(d);
            }
            else
            {
                return;
            }   
           
        }

        public async Task DeleteUserData(string collection, string document, Dictionary<string, object> data)
        {
            DocumentReference docRef = db.Collection(collection).Document(document);

            if (data.ContainsKey("wantlistenproj") && data["wantlistenproj"] is IEnumerable<object> valuesToRemove)
            {
                await docRef.UpdateAsync(new Dictionary<string, object>
                {
                    { "wantlistenproj", FieldValue.ArrayRemove(valuesToRemove.ToArray()) }
                });

                data.Remove("wantlistenproj");

                if (data.Count > 0)
                {
                    await docRef.UpdateAsync(data);
                }
            }
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
                    File.WriteAllText(@"C:\Users\Admin\source\repos\sorbe\sorbe\userdata.txt", jsonResponse.refreshToken);
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
        public async Task AddData(string collection, Dictionary<string, object> data)
        {
            DocumentReference docRef = await db.Collection(collection).AddAsync(data);
        }

    }
}




