
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
using System.Diagnostics;
using System.Buffers;



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
            string relativePath = @"music-servis-firebase-adminsdk-b6st9-53a16acc08.json";
            string credentialPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
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
        public async Task<List<Dictionary<string, object>>> ViewParamData(string collection, string value)
        {
            Query query = db.Collection(collection);

            
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            if (snapshot != null && snapshot.Documents.Count > 0)
            {
                return snapshot.Documents
                                 .Select(doc =>
                                 {
                                     var data = doc.ToDictionary();
                                     string artist = data.ContainsKey("artist") ? data["artist"].ToString() : string.Empty;
                                     string name = data.ContainsKey("name") ? data["name"].ToString() : string.Empty;

                                     if (artist.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                                         name.Contains(value, StringComparison.OrdinalIgnoreCase))
                                     {
                                         data["id"] = doc.Id;
                                         return data;
                                     }

                                     return null;
                                 })
                                 .Where(doc => doc != null) 
                                 .ToList();
            }

            return new List<Dictionary<string, object>>();

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
                    string relativePath = @"userdata.txt";
                    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

                    File.WriteAllText(path, string.Empty);
                    var jsonResponse = JsonConvert.DeserializeObject<SignInClass>(responseString);
                    File.WriteAllText(path, jsonResponse.refreshToken);

                    string exePath = Process.GetCurrentProcess().MainModule.FileName;
                    Application.Current.Shutdown();
                    Process.Start(exePath);
                }
                else
                {
                    MessageBox.Show("Помилка:" + responseString);
                }
            }
        }
        public async Task<string> AutoAuth()
        {
            HttpClient client = new HttpClient();
            string relativePath = @"userdata.txt";
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", File.ReadAllText(path))
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

                File.WriteAllText(path, string.Empty);
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
        public async Task UserRegistration(string email, string password,string name,List<string> genre)
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

                    string relativePath = @"userdata.txt";
                    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
                    var jsonResponse = JsonConvert.DeserializeObject<SignInClass>(responseString);
                    File.WriteAllText(path, string.Empty);
                    File.WriteAllText(path, jsonResponse.refreshToken);
                    MessageBox.Show("Ви успішно зареєстувалися!");
                    FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(jsonResponse.idToken);
                    string uid = decodedToken.Uid;
                    Google.Cloud.Firestore.DocumentReference docRef = db.Collection("users").Document(uid);
                    Dictionary<string, object> city = new Dictionary<string, object>
                    {
                        { "email", email },
                        { "name", name },
                        { "image", "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" },
                        { "rateproj", new string[]{""} },
                        { "wantlistenproj", new string[]{""} },
                        { "favgenre", genre}

                    };
                    await docRef.SetAsync(city);

                    string exePath = Process.GetCurrentProcess().MainModule.FileName;
                    Application.Current.Shutdown();
                    Process.Start(exePath);
                }
                else
                {
                    MessageBox.Show($"Помилка: {responseString}");
                }
            }
        }
        public async Task AddData(string collection, Dictionary<string, object> data)
        {
            DocumentReference docRef = await db.Collection(collection).AddAsync(data);
        }

    }
}




