using Android.Views.Animations;
using CarouselView.FormsPlugin.Abstractions;
using Chatter.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Chatter.Model;
using Xamd.ImageCarousel.Forms.Plugin.Abstractions;
using Plugin.Toast;
using Android.Renderscripts;
using SQLite;
using System.Runtime.InteropServices.WindowsRuntime;
using Android.Media;
using Xamarin.Essentials;
using Chatter.View;
using Rg.Plugins.Popup.Services;
using Chatter.Classes;

namespace Chatter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Discover : ContentPage
    {
        ObservableCollection<ImageStorage> imageSources = new ObservableCollection<ImageStorage>();
        private string currentUserIdSelected = "";
        ImageStorage currentItem;
        ApiConnector api = new ApiConnector();
        bool isLiked = false;
        private int liked_Id = 0;
        public string currentLocation = "", UserProfilePicture = "";
        public string distanceFilter = "";
        bool hasSearchReference = true;
        public Discover()
        {
            InitializeComponent();
            // BindingContext = new ImageStorage();
        }
        protected async override void OnAppearing()
        {
            try
            {
                string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
                System.IO.Directory.CreateDirectory(applicationFolderPath);
                string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
                using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
                {
                    conn.CreateTable<SearchRefenceModel>();
                    var sample = conn.Table<SearchRefenceModel>().ToList();
                    if (sample.Count == 0)
                    {
                        hasSearchReference = false;
                    }
                    //model = sample.Where(x => x.user_id == Application.Current.Properties["Id"].ToString().Replace("\"","")).ToList();
                    foreach (SearchRefenceModel iniModel in sample)
                    {
                        distanceFilter = iniModel.maximum_distance;
                        //       await DisplayAlert("Yes!!", "User ID:" + iniModel.user_id + " Maximum Distance:"+ iniModel.maximum_distance + " Age Range:" + iniModel.age_range, "Okay");
                    }
                    conn.CreateTable<UserModel>();
                    var sample1 = conn.Table<UserModel>().ToList();
                    foreach (UserModel iniModel in sample1)
                    {
                        currentLocation = iniModel.location;
                        UserProfilePicture = iniModel.image;
                    }
                    await loadData();
                }
            }
            catch(Exception e)
            {
                await DisplayAlert("Oops!",e.ToString(),"Okay");
            }
        }
        private async Task loadData()
        {
            try
            {
                using (var cl = new HttpClient())
                {
                    string urlstring = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_all&id='" + Application.Current.Properties["Id"].ToString() + "'";
                    var request = await cl.GetAsync(urlstring);
                    request.EnsureSuccessStatusCode();
                    var response = await request.Content.ReadAsStringAsync();
                    string sample = response.ToString().Replace(@"\", "");
                    //await DisplayAlert("Discover", response, "Okay");
                    var looper = JsonConvert.DeserializeObject<List<ImageStorage>>(sample);
                    if (sample.Contains("Undefined"))
                    {
                        await DisplayAlert("Discover", "No user to display", "Okay");
                        imageSources.Clear();
                        return;
                    }
                    foreach (ImageStorage imageStorage in looper)
                    {
                        if (!imageSources.Any(x => x.id == imageStorage.id))
                        {
                            if (hasSearchReference == true)
                            {
                                if (CheckSearching(imageStorage.location) == false)
                                {
                                    continue;
                                }
                            }
                            imageSources.Add(imageStorage);
                        }
                    }
                    carouselImage.ItemsSource = imageSources;
                }
            }
            catch(Exception ex)
            {
                imageSources.Clear();
                await DisplayAlert("Discover", "No user to display","Okay");
            }
        }

        private async void heartButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                await checkIfLiked();
                var client = new HttpClient();
                var form = new MultipartFormDataContent();
                MultipartFormDataContent content = new MultipartFormDataContent();
                //await DisplayAlert("Game",checkIfLiked().ToString(), "Okay");
                if (isLiked)
                {
                    content.Add(new StringContent(liked_Id.ToString()), "id");
                    content.Add(new StringContent("1"), "visible");
                    var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=updateVisible", content);
                    request.EnsureSuccessStatusCode();
                    var response = await request.Content.ReadAsStringAsync();
                    await PopupNavigation.Instance.PushAsync(new AnimateMatched(UserProfilePicture,currentItem.image));
                    //await DisplayAlert("MATCH FOUND", "You both liked each other! Hurry and send a message!", "Okay");
                    imageSources.Remove(currentItem);
                }
                else
                {

                    await DisplayAlert("Connection", Application.Current.Properties["Id"].ToString().Replace("\"", ""), "Okay");
                    content.Add(new StringContent(Application.Current.Properties["Id"].ToString().Replace("\"", "")), "user_id");
                    content.Add(new StringContent(currentUserIdSelected), "user_id_liked");
                    content.Add(new StringContent("0"), "visible");
                    var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=insert_liked", content);
                    request.EnsureSuccessStatusCode();
                    var response = await request.Content.ReadAsStringAsync();
                    var exec = await DisplayAlert("Discover", "You liked " + currentItem.username, null, "OK");
                    imageSources.Remove(currentItem);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Connection",ex.ToString(), "Okay");
            }
        }
        private void carouselImage_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            if (e.CurrentItem == null)
                return;

            currentItem = e.CurrentItem as ImageStorage;
            currentUserIdSelected = currentItem.id;
        }
        private async Task checkIfLiked()
        {
            string sample = Application.Current.Properties["Id"].ToString().Replace("\"","") + "," + currentUserIdSelected;
            string strurl = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_likeexists&userparam='" + sample + "'";
            //await DisplayAlert("Sample",strurl,"Okay");
            using (var cl = new HttpClient())
            {
                var request = await cl.GetAsync(strurl);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                if (response.Contains("Undefined"))
                {
                    isLiked = false;
                }
                else
                {
                    //await DisplayAlert("Game", strurl + " hayss" + response, "Okay");
                    liked_Id = Convert.ToInt32(response.Replace("\"", ""));
                    isLiked = true;
                }
            }
        }
        private bool CheckSearching(string model)
        {
            string[] currentLocArr = currentLocation.Split(',');
            string[] otherUserLocArr = model.Split(',');
            Location myLocation = new Location(Convert.ToDouble(currentLocArr[0]), Convert.ToDouble(currentLocArr[1]));
            Location otherLocation = new Location(Convert.ToDouble(otherUserLocArr[0]), Convert.ToDouble(otherUserLocArr[1]));
            double kmDistance = Location.CalculateDistance(myLocation,otherLocation,DistanceUnits.Miles);
            if (kmDistance <= Convert.ToDouble(distanceFilter))
                return true;

            return false;
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new ViewProfile(currentUserIdSelected));
            //await PopupNavigation.Instance.PushAsync();
        }

        private async void dislikeButton_Clicked(object sender, EventArgs e)
        {
            string user_id = Application.Current.Properties["Id"].ToString().Replace("\"", "");
            await api.saveToDislikedUser(user_id, currentUserIdSelected);
            imageSources.Remove(currentItem);
        }
        private async void reloadButton_Clicked(object sender, EventArgs e)
        {
            string user_id = Application.Current.Properties["Id"].ToString().Replace("\"", "");
            foreach (ImageStorage model in imageSources)
            {
                await api.saveToDislikedUser(user_id, model.id);
                imageSources.Remove(currentItem);
            }
            OnAppearing();
        }
    }
}