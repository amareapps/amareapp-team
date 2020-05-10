using Android.Widget;
using Plugin.Toast;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Chatter.Model;
using SQLite;
using Android.Hardware;
using Newtonsoft.Json;
using Android.Media;
using Image = Xamarin.Forms.Image;
using System.IO;
using Android.Util;
using System.Net;
using Android.Graphics;

namespace Chatter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login_Input
    {
        UserModel userLogged = new UserModel();
        SearchRefenceModel userSearchReference = new SearchRefenceModel();
        List<GalleryModel> galleryModel = new List<GalleryModel>();
        public Login_Input()
        {
            InitializeComponent();
        }

        private async void loginButton_Clicked(object sender, EventArgs e)
        {
            activityIndicator.IsRunning = true;
            var context = Android.App.Application.Context;
            string sample = emailEntry.Text + "," + passEntry.Text;
            if (emailEntry.Text == string.Empty || passEntry.Text == string.Empty)
            {
                await DisplayAlert("Login Failed", "Please check credentials", "Okay");
                activityIndicator.IsRunning = false;
                return;
            }
            try
            {
                using (var cl = new HttpClient())
                {
                    var request = await cl.GetAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_userexists&email='" + sample + "'");
                    request.EnsureSuccessStatusCode();
                    var response = await request.Content.ReadAsStringAsync();
                    if (response.ToString().Contains("Undefined")) 
                    {
                        await DisplayAlert("Login Failed", "Please check credentials", "Okay");
                        activityIndicator.IsRunning = false;
                        return;
                    }
                    response = response.Replace(@"\", "");
                    var looper = JsonConvert.DeserializeObject<List<UserModel>>(response); 
                    foreach (UserModel model in looper)
                    {
                        var webClient = new WebClient();
                        byte[] imageBytes = webClient.DownloadData(model.image);
                        string base64Image = Convert.ToBase64String(imageBytes);
                        model.image = base64Image;
                        userLogged = model;
                    }
                    Application.Current.Properties["Id"] = "\""+ userLogged.id + "\"";
                    CrossToastPopUp.Current.ShowToastMessage("Welcome " + userLogged.username);
                }
                await retrieveSearchReference();
                await saveToSqlite();
                await retrieveGallery();
                await retrievInbox();
                await loadRecentMatches();
                App.Current.MainPage = new MainPage();
                //await Navigation.PushModalAsync(new MainPage());
                activityIndicator.IsRunning = false;
                await PopupNavigation.Instance.PopAsync(true);

            }
            catch (Exception ex)
            {
                activityIndicator.IsRunning = false;
                await DisplayAlert("Error",ex.ToString(),"Okay");
            }
        }
        private async Task saveToSqlite()
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
        private async Task saveSearchToSqlite()
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
        private async Task retrieveSearchReference()
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
                    userSearchReference = model;
                    break;
                }
                await saveSearchToSqlite();
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
        private async Task retrieveGallery()
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
                var modifString = response.Replace(@"\","");
                var looper = JsonConvert.DeserializeObject<List<GalleryModel>>(modifString);
                foreach (GalleryModel model in looper)
                {
                    await saveGalleryToSqlite(model);
                }
            }
        }
        private async Task retrievInbox()
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
                    saveInbox(messageContent);
                        //     inboxModels.Add(messageContent);
                        // }
                }
                    //InboxList.ItemsSource = inboxModels;
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.ToString(), "Okay");
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
        private async Task loadRecentMatches()
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
                await DisplayAlert("Error", ex.ToString(), "Okay");
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

        private void activityIndicator_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (activityIndicator.IsRunning)
            {
                loginButton.IsEnabled = false;
            }
            else
            {
                loginButton.IsEnabled = true;
            }
        }
    }
}