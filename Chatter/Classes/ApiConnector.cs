using Android.Graphics;
using Android.Media;
using Chatter.Model;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Chatter.Classes
{
    public class ApiConnector
    {
        public async Task<string> insertToPhoneRegister(string number,string code)
        {
            var client = new HttpClient();
            var form = new MultipartFormDataContent();
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StringContent(number), "phone_number");
            content.Add(new StringContent(code), "code");
            var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=register_number", content);
            request.EnsureSuccessStatusCode();
            var response = await request.Content.ReadAsStringAsync();
            return response;
        }
        public async Task<bool> checkCode(string number,string code)
        {
            try
            {
                using (var cl = new HttpClient())
                {
                    string urlstring = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=checkCode&number=" + number;
                    var request = await cl.GetAsync(urlstring);
                    request.EnsureSuccessStatusCode();
                    var response = await request.Content.ReadAsStringAsync();
                    if (response.Contains("Undefined"))
                    {
                        return false;
                    }
                    var looper = JsonConvert.DeserializeObject<List<RegisterNumberModel>>(response).ToList();
                    foreach (RegisterNumberModel model in looper)
                    {
                        if (model.code == code)
                            return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }
        public async Task<UserModel> getUserModel(string number)
        {
            try
            {
                UserModel user = new UserModel();
                using (var cl = new HttpClient())
                {
                    string urlstring = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_phonenumber&number=" + number;
                    var request = await cl.GetAsync(urlstring);
                    request.EnsureSuccessStatusCode();
                    var response = await request.Content.ReadAsStringAsync();
                    if (response.Contains("Undefined"))
                    {
                        return null;
                    }
                    var looper = JsonConvert.DeserializeObject<List<UserModel>>(response).ToList();
                    foreach (UserModel modeler in looper)
                    {
                        var webClient = new WebClient();
                        byte[] imageBytes = webClient.DownloadData(modeler.image);
                        string base64Image = Convert.ToBase64String(imageBytes);
                        modeler.image = base64Image;
                        user = modeler;
                    }
                }
                await saveToSqlite(user);
                await retrieveSearchReference();
                await retrieveGallery();
                await retrievInbox();
                await loadRecentMatches();
                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task saveToSqlite(UserModel userLogged)
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<UserModel>();
                conn.Insert(userLogged);
            }
        }
        private async Task saveSearchToSqlite(SearchRefenceModel userSearchReference)
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<SearchRefenceModel>();
                conn.Insert(userSearchReference);
            }
        }
        public async Task retrieveSearchReference()
        {
            using (var cl = new HttpClient())
            {
                string urlString = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_search_reference&id='" + Application.Current.Properties["Id"].ToString() + "'";
                var request = await cl.GetAsync(urlString);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                //await DisplayAlert("Erro!", response.ToString(), "Okay");
                if (response.ToString().Contains("Undefined"))
                {
                    return;
                }
                var looper = JsonConvert.DeserializeObject<List<SearchRefenceModel>>(response);
                foreach (SearchRefenceModel model in looper)
                {

                    await saveSearchToSqlite(model);
                    break;
                }
            }
        }

        private async Task saveGalleryToSqlite(GalleryModel model)
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<GalleryModel>();
                conn.Insert(model);
            }
        }
        public async Task retrieveGallery()
        {
            using (var cl = new HttpClient())
            {
                string urlString = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_gallery&user_id='" + Application.Current.Properties["Id"].ToString() + "'";
                var request = await cl.GetAsync(urlString);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                //await DisplayAlert("Error! Login_Input", response.ToString(), "Okay");
                if (response.ToString().Contains("Undefined"))
                {
                    return;
                }
                var modifString = response.Replace(@"\", "");
                var looper = JsonConvert.DeserializeObject<List<GalleryModel>>(modifString);
                foreach (GalleryModel model in looper)
                {
                    await saveGalleryToSqlite(model);
                }
            }
        }
        public async Task retrievInbox()
        {
            try
            {
                //Get the data for inbox list
                var client = new HttpClient();
                var form = new MultipartFormDataContent();
                string strurl = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_inbox&id='" + Application.Current.Properties["Id"].ToString() + "'";
                var request = await client.GetAsync(strurl);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                if (response.ToString().Contains("Undefined"))
                    return;
                var looper = JsonConvert.DeserializeObject<List<InboxModel>>(response.ToString());
                //await DisplayAlert("testlang","hahaha","okay");
                foreach (InboxModel messageContent in looper)
                {
                    var webClient = new WebClient();
                    byte[] imageBytes = webClient.DownloadData(messageContent.image);

                    Bitmap bitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    Bitmap resizedImage = Bitmap.CreateScaledBitmap(bitmap, 50, 50, false);
                    using (var stream = new MemoryStream())
                    {
                        resizedImage.Compress(Bitmap.CompressFormat.Png, 0, stream);
                        var bytes = stream.ToArray();
                        var str = Convert.ToBase64String(bytes);
                        messageContent.image = str;
                    }
                    await saveInbox(messageContent);
                    //     inboxModels.Add(messageContent);
                    // }
                }
                //InboxList.ItemsSource = inboxModels;
            }
            catch (Exception ex)
            {
            }
        }
        private async Task saveInbox(InboxModel model)
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<InboxModel>();
                conn.Insert(model);
            }
        }
        public async Task loadRecentMatches()
        {
            try
            {
                //Get the data for inbox list
                var client = new HttpClient();
                var form = new MultipartFormDataContent();
                string strurl = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_recentmatches&id='" + Application.Current.Properties["Id"].ToString() + "'";
                var request = await client.GetAsync(strurl);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                if (response.ToString().Contains("Undefined"))
                    return;
                var looper = JsonConvert.DeserializeObject<List<RecentMatchesModel>>(response.ToString());
                foreach (RecentMatchesModel matches in looper)
                {
                    var webClient = new WebClient();
                    byte[] imageBytes = webClient.DownloadData(matches.image);
                    Bitmap bitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    Bitmap resizedImage = Bitmap.CreateScaledBitmap(bitmap, 50, 50, false);
                    using (var stream = new MemoryStream())
                    {
                        resizedImage.Compress(Bitmap.CompressFormat.Png, 0, stream);
                        var bytes = stream.ToArray();
                        var str = Convert.ToBase64String(bytes);
                        matches.image = str;
                    }

                    saveRecentToLocalDb(matches);
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void saveRecentToLocalDb(RecentMatchesModel model)
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<RecentMatchesModel>();
                conn.InsertOrReplace(model);
            }
        }
        public async Task<UserModel> getSpeificUser(string id)
        {
            try
            {
                using (var cl = new HttpClient())
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    string urlstring = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_single&id=" + id;
                    var request = await cl.GetAsync(urlstring);
                    request.EnsureSuccessStatusCode();
                    var response = await request.Content.ReadAsStringAsync();
                    response = response.Replace(@"\","");
                    //response = response.Replace("null","\"\"");
                    if (response.Contains("Undefined"))
                    {
                        return null;
                    }
                    var looper = JsonConvert.DeserializeObject<UserModel>(response, settings);
                    return looper;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<GalleryModel>> otherUserImageList(string id)
        {
            try
            {
                using (var cl = new HttpClient())
                {
                    string urlString = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_gallery&user_id=" + id + "";
                    var request = await cl.GetAsync(urlString);
                    request.EnsureSuccessStatusCode();
                    var response = await request.Content.ReadAsStringAsync();
                    //await DisplayAlert("Error! Login_Input", response.ToString(), "Okay");
                    if (response.ToString().Contains("Undefined"))
                    {
                        return null;
                    }
                    var modifString = response.Replace(@"\", "");
                    var looper = JsonConvert.DeserializeObject<List<GalleryModel>>(modifString).ToList();
                    return looper;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task saveToDislikedUser(string user_id,string usertodislike)
        {
            var client = new HttpClient();
            var form = new MultipartFormDataContent();
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StringContent(user_id), "user_id");
            content.Add(new StringContent(usertodislike), "disliked_user");
            var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=insert_dislike", content);
            request.EnsureSuccessStatusCode();
            var response = await request.Content.ReadAsStringAsync();
        }
        public async Task updateProfilePicture(string user_id,string image)
        {
            var client = new HttpClient();
            var form = new MultipartFormDataContent();
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StringContent(user_id), "id");
            content.Add(new StringContent(image), "image");
            var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=update_profile_picture", content);
            request.EnsureSuccessStatusCode();
            var response = await request.Content.ReadAsStringAsync();
        }
        public async Task syncUserData(string id)
        {
                try
                {
                    using (var cl = new HttpClient())
                    {
                        string urlString = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_single&id=" + id + "";
                        var request = await cl.GetAsync(urlString);
                        request.EnsureSuccessStatusCode();
                        var response = await request.Content.ReadAsStringAsync();
                        //await DisplayAlert("Error! Login_Input", response.ToString(), "Okay");
                        if (response.ToString().Contains("Undefined"))
                        {
                            return;
                        }
                        var modifString = response.Replace(@"\", "");
                        var looper = JsonConvert.DeserializeObject<UserModel>(modifString);
                        looper.image = converttoBase64(looper.image);
                        syncUsertoSqlite(looper);
                    }
                }
                catch (Exception ex)
                {
                    return;
                }
        }
        private void syncUsertoSqlite(UserModel model)
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<UserModel>();
                conn.InsertOrReplace(model);
            }
        }
        private string converttoBase64(string imageUrl)
        {
            var webClient = new WebClient();
            byte[] imageBytes = webClient.DownloadData(imageUrl);
            Bitmap bitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
            using (var stream = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
                var bytes = stream.ToArray();
                var str = Convert.ToBase64String(bytes);
                return str;
            }
        }

    }
}
