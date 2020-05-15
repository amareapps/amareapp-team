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
using System.Net.WebSockets;
using Chatter.Classes;
using Google.Protobuf.WellKnownTypes;
using Android.OS;
using System.Threading;
using Android.Media;
using Plugin.Media.Abstractions;
using Java.Sql;

namespace Chatter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Messaging : ContentPage
    {
        ObservableCollection<ChatModel> chatModels = new ObservableCollection<ChatModel>();
        private string Session_Id = "", Receiver_Id = "",Username = "",Image_Source="",Emoji = "";
        ImageOption imageOpt = new ImageOption();
        Base64toImageConverter converters = new Base64toImageConverter();
        string userLoggedIn = Application.Current.Properties["Id"].ToString().Replace("\"", "");
        ClientWebSocket wsClient = new ClientWebSocket();
        FireStorage fireStorage = new FireStorage();
        //System.Timers.Timer timer;
        public Messaging(string receiver_id,string session_id,string username,string imagesource,string emoji)
        {
            Session_Id = session_id;
            Receiver_Id = receiver_id;
            Username = username;
            Emoji = emoji;
            Image_Source = imagesource;
            InitializeComponent();
            NavigationPage.SetHasBackButton(this,false);
            userImage.Source = Image_Source;

            lblEmoji.Text = emoji;
        }

        async Task ConnectToServerAsync()
        {
            await wsClient.ConnectAsync(new Uri("ws://"+ApiConnection.Url+":8088"), CancellationToken.None);
        }
        protected async override void OnAppearing()
        {
            await loadData();
            //scrollToBottom();
            ChatList.ItemsSource = chatModels.OrderByDescending(entry => entry.datetime);
            lblReceiver.Text = Username;
            //timer = new Timer();
            //timer.Elapsed += Timer_Elapsed;
            //timer.Interval = 1000;
            //timer.Start();
            await ConnectToServerAsync();
            while (wsClient.State == WebSocketState.Open)
            {
                await ReadMessage();
            }
        }
        private void scrollToBottom()
        {
            var target = chatModels[chatModels.Count - 1];
            ChatList.ScrollTo(target, ScrollToPosition.End, true);
        }
        async Task ReadMessage()
        {
            WebSocketReceiveResult result;
            var message = new ArraySegment<byte>(new byte[4096]);
            string receivedMessage;
            do
            {
                result = await wsClient.ReceiveAsync(message, CancellationToken.None);
                var messageBytes = message.Skip(message.Offset).Take(result.Count).ToArray();
                receivedMessage = System.Text.Encoding.UTF8.GetString(messageBytes);
                var resultModel = JsonConvert.DeserializeObject<ChatModel>(receivedMessage);
                if ((resultModel.sender_id == userLoggedIn && resultModel.receiver_id == Receiver_Id) ||
                    (resultModel.sender_id == Receiver_Id && resultModel.receiver_id == userLoggedIn))
                {
                    //await DisplayAlert("Anayre", userLoggedIn + resultModel.sender_id + resultModel.receiver_id, "Okay");
                    resultModel.image = Image_Source;
                    chatModels.Add(resultModel);
                    ChatList.ItemsSource = chatModels.OrderByDescending(entry => entry.datetime);
                }
            }
            while (!result.EndOfMessage);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () => await loadData());
            throw new NotImplementedException();
        }
        protected override void OnDisappearing()
        {
            //timer.Stop();
        }
        private async Task loadData()
        {
            await dataList();
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
            await sendMessage(messageEntry.Text);
        }

        private void ChatList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ChatModel model = e.SelectedItem as ChatModel;
            if (model.isVisible == "false")
                model.isVisible = "true";
            else
                model.isVisible = "false";

            var oldItem = chatModels.FirstOrDefault(i => i.id == model.id);
            var oldIndex = chatModels.IndexOf(oldItem);
            chatModels[oldIndex] = model;
            //chatModels[e.SelectedItemIndex].isVisible = "true";
        }

        private void messageEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue == string.Empty)
            {
                lblEmoji.IsVisible = true;
                sendButton.IsVisible = false;
            }
            else
            {
                lblEmoji.IsVisible = false;
                sendButton.IsVisible = true;
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await sendMessage(lblEmoji.Text);
        }

        private async void ChatList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            await DisplayAlert("Ito yun","sana","Okay");
        }
        private async void imagePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker picker = sender as Picker;
            string userId = Application.Current.Properties["Id"].ToString().Replace("\"", "");
            if (picker.SelectedIndex == -1)
                return;
            ImageOption imageOption = new ImageOption();
            MediaFile imagePath = null;
            if (picker.SelectedIndex == 0)
            {
                imagePath = await imageOpt.TakePhoto();
            }
            else if (picker.SelectedIndex == 1)
            {
                imagePath = await imageOpt.UploadPhoto();
            }
            string imageLink = await fireStorage.StoreImages(imagePath.GetStream(), Session_Id + userId + Receiver_Id + DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss_fff"));
            await sendMessage(imageLink);
        }

        private void sendimageButton_Clicked(object sender, EventArgs e)
        {
            imagePicker.Focus();
        }

        private async Task sendMessage(string message)
        {
            ChatModel modeler = new ChatModel {
                id = "1",
                sender_id = Application.Current.Properties["Id"].ToString().Replace("\"", ""),
                sender_username = Application.Current.Properties["username"].ToString(),
                session_id = Session_Id,
                receiver_id = Receiver_Id,
                message = message,
                datetime = DateTime.Now.ToString()
            };
            string val = JsonConvert.SerializeObject(modeler);
            //await DisplayAlert("Test", val, "Okay");
            var byteMessage = System.Text.Encoding.UTF8.GetBytes(val);
            var segmnet = new ArraySegment<byte>(byteMessage);
            await wsClient.SendAsync(segmnet, WebSocketMessageType.Text, true, CancellationToken.None);
            messageEntry.Text = string.Empty;
            messageEntry.Unfocus();
            /*
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
            */
        }

        private void backButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}