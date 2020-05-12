using System;
using System.Net.Http;
using Chatter.Model;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using Chatter.Classes;
using System.Linq;

namespace Chatter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Settings : ContentPage
    {
        private string locationString;
        public Settings()
        {
            InitializeComponent();
            loadFromDatabase();
        }

        private void backButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private async void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (locationPicker.SelectedIndex == 0)
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    string sample = location.ToString();
              //      await DisplayAlert("", locationString, "Okay");
                }
                else
                {
                    await DisplayAlert("Error", "Location not found. Please turn on your GPS.", "Okay");
                }
            }
            else
            {

            }
        }

        private async void saveButton_Clicked(object sender, EventArgs e)
        {
            //Set to object
            SearchRefenceModel searchReference = new SearchRefenceModel()
            {
                user_id = Application.Current.Properties["Id"].ToString().Replace("\"", ""),
                maximum_distance = slider.Value.ToString("0"),
                age_range = ageslider.Value.ToString("0")
            };

            //Save to Remote Database
            var client = new HttpClient();
            var form = new MultipartFormDataContent();
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StringContent(searchReference.user_id), "user_id");
            content.Add(new StringContent(searchReference.maximum_distance), "maximum_distance");
            content.Add(new StringContent(searchReference.age_range), "age_range");
            var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=update_search_reference", content);
            request.EnsureSuccessStatusCode();
            var response = await request.Content.ReadAsStringAsync();

            //Save to Local Database
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<SearchRefenceModel>();
                conn.InsertOrReplace(searchReference);
            }
            await Navigation.PopModalAsync();
        }
        private void loadFromDatabase()
        {
            //Save to Local Database
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<SearchRefenceModel>();
                var table = conn.Table<SearchRefenceModel>().ToList();
                foreach (SearchRefenceModel model in table)
                {
                    slider.Value = Convert.ToInt32(model.maximum_distance);
                    ageslider.Value = Convert.ToInt32(model.age_range);
                }
            }
        }
        private void logoutButton_Clicked(object sender, EventArgs e)
        {

            deleteFromSqlite();
            var navigationPages = Navigation.NavigationStack.ToList();
            /*foreach (var page in navigationPages)
            {
                DisplayAlert("Check!",page.ToString(),"Okay");
            }
            */
            App.Current.MainPage = new NavigationPage(new Login());
            //Navigation.PushModalAsync();
            //Navigation.PopModalAsync();
        }
        private void deleteFromSqlite()
        {
            string _id =Application.Current.Properties["Id"].ToString().Replace("\"","");
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<UserModel>();
                var table = conn.Table<UserModel>().Delete(x => x.id == _id);
                conn.CreateTable<InboxModel>();
                var table1 = conn.Table<InboxModel>().Delete(x => x.user_id != "");
                conn.CreateTable<RecentMatchesModel>();
                var table3 = conn.Table<RecentMatchesModel>().Delete(x => x.user_id != "");
                conn.CreateTable<SearchRefenceModel>();
                var table4 = conn.Table<SearchRefenceModel>().Delete(x => x.user_id != "");
                conn.CreateTable<GalleryModel>();
                var table5 = conn.Table<GalleryModel>().Delete(x => x.user_id != "");
            }
            DependencyService.Get<IClearCookies>().Clear();
        }
    }
}