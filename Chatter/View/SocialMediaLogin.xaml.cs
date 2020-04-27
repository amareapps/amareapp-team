using Chatter.Classes;
using Chatter.Model;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Chatter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SocialMediaLogin : ContentPage
    {
        private UserModel userModel = new UserModel();
        public string client_ID = "249917976134115";
        private string imageUrl;
        public class SocialMediaPlatform {
            public static readonly int Facebook = 0;
            public static readonly int Instagram = 1;
            public static readonly int Google = 2;
        }

        public SocialMediaLogin(int platform)
        {
            InitializeComponent();
            var apiRequest = "";
            if (platform == SocialMediaPlatform.Facebook)
            {
                  apiRequest = "https://www.facebook.com/dialog/oauth?client_id="
                  + client_ID
                  + "&display=popup&response_type=token&redirect_uri=" +
                  "https://www.facebook.com/connect/login_success.html";
            }
            if (platform == SocialMediaPlatform.Instagram)
            {
                apiRequest = "https://api.instagram.com/oauth/authorize/?client_id"
                + client_ID
                + "&redirect_uri=https://www.instagram.com/" + "&response_type=token";
            }
            var webView = new WebView
            {
                Source = apiRequest,
                HeightRequest = 1
            };
            webView.Navigated += WebView_Navigated;

            Content = webView;
        }

        private async void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            var ewan = e.Url;
            var accessToken = ExtractAccessTokenFromUrl(e.Url);
            if (accessToken != "")
            {
                await getFacebookProfileAsync(accessToken);

            }
        }
        public async Task getFacebookProfileAsync(string accessToken)
        {
            try
            {
                using (var cl = new HttpClient())
                {
                    var request = await cl.GetAsync("https://graph.facebook.com/v6.0/me/?fields=name,picture.width(800),gender&access_token=" + accessToken);
                    request.EnsureSuccessStatusCode();
                    var response = await request.Content.ReadAsStringAsync();
                    var profile = JsonConvert.DeserializeObject<FacebookProfile>(response);
                    //var requestLoc = new GeolocationRequest(GeolocationAccuracy.High);
                   // var location = await Geolocation.GetLocationAsync(requestLoc);
                    userModel.username = profile.Name;
                    userModel.gender = profile.Gender;
                    userModel.id = profile.Id;
                    var webClient = new WebClient();
                    byte[] imageBytes = webClient.DownloadData(profile.Picture.Data.Url);
                    imageUrl = profile.Picture.Data.Url;
                    
                    string base64Image = Convert.ToBase64String(imageBytes);
                    userModel.image = base64Image;
                    userModel.location = "70.30" + "," + "30,20";
                    await sampless();
                    await saveDataSqlite();
                }
            }
            catch(Exception ex)
            {
                await DisplayAlert("Error!",ex.ToString(),"Okay");
            }
        }
        private string ExtractAccessTokenFromUrl(string url)
        {
            if (url.Contains("access_token") && url.Contains("&expires_in="))
            {
                var at = url.Replace("https://www.facebook.com/connect/login_success.html#access_token=", "");

                var accessToken = at.Remove(at.IndexOf("&expires_in="));

                return accessToken;
            }

            return string.Empty;
        }
        private async Task saveDataSqlite()
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<UserModel>();
                conn.Insert(userModel);
            }
            await loadMainPage();
        }
        private async Task loadMainPage()
        {
            await Navigation.PushModalAsync(new MainPage());
        }
        private async Task sampless()
        {
            var client = new HttpClient();
            var form = new MultipartFormDataContent();
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StringContent(userModel.id), "id");
            content.Add(new StringContent(""), "email");
            content.Add(new StringContent(""), "password");
            content.Add(new StringContent(userModel.username), "username");
            content.Add(new StringContent(userModel.gender), "gender");
            content.Add(new StringContent(userModel.location), "location");
            content.Add(new StringContent(imageUrl), "image");
            var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=insert_fb_user", content);
            request.EnsureSuccessStatusCode(); 
            var response = await request.Content.ReadAsStringAsync();
            var exec = await DisplayAlert("Congratulations!", "You are successfully logged in", null, "OK");
        }
    }
}