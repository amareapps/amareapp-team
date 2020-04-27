using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Chatter.Model;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Android.Media;
using System.Timers;
using SQLite;
using System.Net;
using System.IO;
using Android.Graphics;
using System.Text.RegularExpressions;

namespace Chatter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InboxMessaging : ContentPage
    {
        ObservableCollection<InboxModel> inboxModels = new ObservableCollection<InboxModel>();
        ObservableCollection<RecentMatchesModel> matchesModel = new ObservableCollection<RecentMatchesModel>();
        InboxModel modeler;
        Timer timer = new Timer();
        public InboxMessaging()
        {
            InitializeComponent();
          //  Task.Run(async () => { await retrieveAll(); });
        }
        private void SyncFromDb()
        {
             loadRecentMatchesLocal();
             loadDataFromLocalDb();
        }
        protected async override void OnAppearing()
        {
            activityIndicator.IsVisible = true;
            activityIndicator.IsRunning = true;
            activityIndicator.IsEnabled = true;
            /*
            await Task.Run(async () =>
            {
                // Run code here
                await loadData();
                await loadRecentMatches();
                Device.BeginInvokeOnMainThread(() =>
                {
                    SyncFromDb();
                });
            });
            */
            timer.Interval = 2000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }
        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
                // Run code here
            await loadData();
            await loadRecentMatches();

            Device.BeginInvokeOnMainThread(() =>
            {
                SyncFromDb();
                activityIndicator.IsVisible = false;
                activityIndicator.IsRunning = false;
                activityIndicator.IsEnabled = false;
            });
        }
        protected override void OnDisappearing()
        {
            //Navigation.PopModalAsync();
            timer.Stop();
        }
        private async Task loadData()
        {
            await dataList();
        }
        private async Task dataList()
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
                if (looper.Count == inboxModels.Count)
                    return;
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
                    if (!inboxModels.Any(x => x.session_id == messageContent.session_id))
                        saveToLocalDb(messageContent);
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

        private void InboxList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            modeler = e.Item as InboxModel;
            Messaging chatForm = new Messaging(modeler.user_id,modeler.session_id,modeler.username,modeler.image);
            Navigation.PushModalAsync(chatForm);
        }
        private async Task loadRecentMatches()
        {
            try {
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
                if (looper.Count == matchesModel.Count)
                    return;
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
                    if (!matchesModel.Any(x => x.user_id == matches.user_id))
                        saveRecentToLocalDb(matches);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "Okay");
            }
        }
        private void saveToLocalDb(InboxModel model)
        {
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<InboxModel>();
                conn.InsertOrReplace(model);
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
        private void loadDataFromLocalDb()
        {
            //Load Inbox Table
            try
            {
                string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
                System.IO.Directory.CreateDirectory(applicationFolderPath);
                string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
                using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
                {
                    conn.CreateTable<InboxModel>();
                    var table = conn.Table<InboxModel>().ToList();
                    foreach (InboxModel model in table)
                    {
                        if (!inboxModels.Any(x => x.session_id == model.session_id))
                        {
                            inboxModels.Add(model);
                        }
                    }
                }
                InboxList.ItemsSource = inboxModels;
            }
            catch (Exception ex)
            {
                 DisplayAlert("Error!",ex.ToString(),"Okay");
            }
        }
        private void loadRecentMatchesLocal()
        {
            //Load Recent Matches Table
            string applicationFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "databaseFolder");
            System.IO.Directory.CreateDirectory(applicationFolderPath);
            string databaseFileName = System.IO.Path.Combine(applicationFolderPath, "amera.db");
            using (SQLiteConnection conn = new SQLiteConnection(databaseFileName))
            {
                conn.CreateTable<RecentMatchesModel>();
                var table = conn.Table<RecentMatchesModel>().ToList();
                foreach (RecentMatchesModel model in table)
                {
                    if (!matchesModel.Any(x => x.user_id == model.user_id))
                        matchesModel.Add(model);
                }
            }
            BindableLayout.SetItemsSource(recentMatchesList, matchesModel);
        }
    }
}