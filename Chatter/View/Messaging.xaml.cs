using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using XamarinFirebase;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http;
using Android.Gestures;
using Newtonsoft.Json;
using Chatter.Model;
using System.Reactive.Linq;
using System.Collections.ObjectModel;
using Android.Hardware;
using System.Timers;
using Chatter.Classes;

namespace Chatter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Messaging : ContentPage
    {
        ObservableCollection<ChatModel> chatModels = new ObservableCollection<ChatModel>();
        private string Session_Id = "", Receiver_Id = "",Username = "",Image_Source="";
        Base64toImageConverter converters = new Base64toImageConverter();
        Timer timer;
        public Messaging(string receiver_id,string session_id,string username,string imagesource)
        {
            Session_Id = session_id;
            Receiver_Id = receiver_id;
            Username = username;
            Image_Source = imagesource;
            InitializeComponent();
            NavigationPage.SetHasBackButton(this,false);
            userImage.Source = Image_Source;
        }
        protected override void OnAppearing()
        {
            lblReceiver.Text = Username;
            timer = new Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 1000;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () => await loadData());
            throw new NotImplementedException();
        }
        protected override void OnDisappearing()
        {
            timer.Stop();
        }
        private async Task loadData()
        {
            await dataList();
            scrolltoBottom();
        }
        private async Task dataList()
        {
            try
            {
                var client = new HttpClient();
                var form = new MultipartFormDataContent();
                string sample = Application.Current.Properties["Id"].ToString().Replace("\"", "") + "," + Receiver_Id;
                string strurl = "http://" + ApiConnection.Url + "/apier/api/test_api.php?action=fetch_message&user_id='" + sample + "'";
                var request = await client.GetAsync(strurl);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                //await DisplayAlert("Error daw", strurl, "Okay");
                if (response.ToString().Contains("Undefined"))
                {
                    return;
                }
                var looper = JsonConvert.DeserializeObject<List<ChatModel>>(response.ToString());
                foreach (ChatModel messageContent in looper)
                {
                    if (!chatModels.Any(x => x.id == messageContent.id))
                    {
                        if (messageContent.sender_id == Application.Current.Properties["Id"].ToString().Replace("\"", ""))
                        {
                            messageContent.position = "1";
                            messageContent.spacingposition = "0";
                        }
                        else
                        {
                            messageContent.position = "0";
                            messageContent.spacingposition = "1";
                        }
                        messageContent.image = Image_Source;
                        //await DisplayAlert("Testing",messageContent.sender_id + " Position" + messageContent.position,"Okay");
                        chatModels.Add(messageContent);
                    }
                }
                ChatList.ItemsSource = chatModels;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error!",ex.ToString(),"Okay");
            }
        }

        private async void sendButton_Clicked(object sender, EventArgs e)
        {
            await sendMessage();
        }

        private void sendimageButton_Clicked(object sender, EventArgs e)
        {

        }

        private async Task sendMessage()
        {
            var client = new HttpClient();
            var form = new MultipartFormDataContent();
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StringContent(Application.Current.Properties["Id"].ToString().Replace("\"","")), "sender_id");
            content.Add(new StringContent(Application.Current.Properties["username"].ToString()), "sender_username");
            content.Add(new StringContent(Session_Id), "session_id");
            content.Add(new StringContent(Receiver_Id), "receiver_id");
            content.Add(new StringContent(messageEntry.Text), "message");
            var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=insert_message", content);
            request.EnsureSuccessStatusCode();
            var response = await request.Content.ReadAsStringAsync();
            //var exec = await DisplayAlert("Message", response.ToString() +  " Message Sent by: " + Application.Current.Properties["username"].ToString(), null, "OK");
            messageEntry.Text = string.Empty;
            messageEntry.Unfocus();
            scrolltoBottom();

        }

        private void backButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private void scrolltoBottom()
        {
            var messages = chatModels.ToArray();
            if (messages.Length == 0)
                return;
            var target = messages[messages.Length - 1];
            ChatList.ScrollTo(messages, ScrollToPosition.End, true);
        }
    }
}