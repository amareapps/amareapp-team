using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Permissions;
using Android.Graphics;
using Chatter.Model;
using Android.Media;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using Color = Xamarin.Forms.Color;
using Plugin.Media.Abstractions;
using Stream = System.IO.Stream;
using Firebase.Storage;
using Xamarin.Essentials;
using Chatter.Classes;

namespace Chatter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class ProfileMaintenance : CarouselPage
    {
        bool _isInsert = true;
        string gender = "",interestIn="";
        string locationString = "";
        string imageString;
        string number = "";
        MediaFile file;
        ApiConnector api = new ApiConnector();
        private byte[] imageaRray;

        public ProfileMaintenance(string _number)
        {
            InitializeComponent();
            number = _number;

            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.FromHex("98000b");
            ((NavigationPage)Application.Current.MainPage).BarTextColor = Color.FromHex("fffcf8");
            BindingContext = new UserModelStorage();
        }
        private void clearFields()
        {
            userNameEntry.Text = string.Empty;
            passwordEntry.Text = string.Empty;
        }

        private async void continueButton_Clicked(object sender, EventArgs e)
        {
            if (userNameEntry.Text == string.Empty || passwordEntry.Text == string.Empty || 
                emailEntry.Text == string.Empty || gender == string.Empty || imageString == string.Empty || interestIn == string.Empty 
                || universityEntry.Text == string.Empty)
            {
                await DisplayAlert("Oops!", "Incomplete credentials! Please fill the required fields.", "Okay");
                return;
            }
            activityIndicator.IsRunning = true;
            Application.Current.Properties["Name"] = userNameEntry.Text;
            Application.Current.Properties["Password"] = passwordEntry.Text;
            Application.Current.Properties["Email"] = emailEntry.Text;
            Application.Current.Properties["Gender"] = gender;
            Application.Current.Properties["Birthday"] = birthdatePicker.ToString();
            var request = new GeolocationRequest(GeolocationAccuracy.High);
            var location = await Geolocation.GetLocationAsync(request);

            if (location == null)
            {
                return;
            }
            locationString = location.Latitude.ToString() + "," + location.Longitude.ToString();

            if (imageString == string.Empty)
            {
                await DisplayAlert("Image Selection", "Image required.", "Okay");
                return;
            }
            await uploadtoServer();
            await sampless();
            activityIndicator.IsRunning = false;
            //await Navigation.PushAsync(new ImageSelection());
            //await DisplayAlert("Image Selection", string.IsNullOrEmpty(number).ToString(), "Okay");
            if (string.IsNullOrEmpty(number))
            {
                await Navigation.PopToRootAsync();
            }
            else
            {
                var value = await api.getUserModel(number);
                if (value == null) {
                    await DisplayAlert("Oops!",value.username,"Okay");
                }
                Application.Current.Properties["Id"] = "\"" + value.id + "\"";
                await Navigation.PushModalAsync(new MainPage());
                await Navigation.PopToRootAsync();
            }
        }
        private async Task sampless()
        {
            try
            {
                var client = new HttpClient();
                var form = new MultipartFormDataContent();
                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Add(new StringContent(Application.Current.Properties["Email"].ToString()), "email");
                content.Add(new StringContent(Application.Current.Properties["Password"].ToString()), "password");
                content.Add(new StringContent(Application.Current.Properties["Name"].ToString()), "username");
                content.Add(new StringContent(Application.Current.Properties["Gender"].ToString()), "gender");
                content.Add(new StringContent(locationString), "location");
                content.Add(new StringContent(imageString), "image");
                content.Add(new StringContent(number), "phone_number");
                content.Add(new StringContent(birthdatePicker.Date.ToString()), "birthdate");
                content.Add(new StringContent(interestIn), "interest");
                content.Add(new StringContent(universityEntry.Text), "school");

                var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=insert", content);
                request.EnsureSuccessStatusCode();
                var response = await request.Content.ReadAsStringAsync();
                var exec = await DisplayAlert("Congratulations!", "You have successfully registered!", null, "Okay");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Oops!",ex.ToString(),"Okay");
            }
        }
        private async Task uploadtoServer()
        {
            await StoreImages(file.GetStream());
        }
        public async Task StoreImages(Stream imageStream)
        {
            var stroageImage = await new FirebaseStorage("chatter-7b8e4.appspot.com")
                .Child("UserImages")
                .Child(Application.Current.Properties["Email"].ToString().Replace(".", "") + ".jpg")
                .PutAsync(imageStream);
            string imgurl = stroageImage;
            imageString = imgurl;
        }
        private void Button_Clicked(object sender, EventArgs e)
        {
            var looper = iamGrid.Children.Where(x => x is Button);
            foreach (Button btn in looper)
            {
                btn.BackgroundColor = Color.FromHex("#fffcf8");
            }
            Button btne = (Button)sender;
            btne.BackgroundColor = Color.FromHex("#adadad");
            gender = btne.Text;
        }
        private void Button_Interest(object sender, EventArgs e)
        {
            var looper = gridInterest.Children.Where(x => x is Button);
            foreach(Button btn in looper)
            {
                btn.BackgroundColor = Color.FromHex("#fffcf8");
            }
            Button btne = (Button)sender;
            btne.BackgroundColor = Color.FromHex("#adadad");
            interestIn = btne.Text;
        }
        private void nextContent(object sender, EventArgs e)
        {
            if (this.CurrentPage == emailContent)
            {
                this.CurrentPage = passwordContent;
            }
            else if (this.CurrentPage == passwordContent)
            {
                this.CurrentPage = nameContent;
            }
            else if (this.CurrentPage == nameContent)
            {
                this.CurrentPage = birthdayContent;
            }
            else if (this.CurrentPage == birthdayContent)
            {
                this.CurrentPage = schoolContent;
            }
            else if (this.CurrentPage == schoolContent)
            {
                this.CurrentPage = genderContent;
            }
            else if (this.CurrentPage == genderContent)
            {
                this.CurrentPage = interestContent;
            }
            else if (this.CurrentPage == interestContent) {
                this.CurrentPage = pictureContent;
            }
        }

        private void chooseImageButton_Clicked(object sender, EventArgs e)
        {
            var monkeyList = new List<string>();
            monkeyList.Add("Take Photo");
            monkeyList.Add("Choose from Gallery");
            imagePicker.Title = "Select Image";
            imagePicker.ItemsSource = monkeyList;
            imagePicker.Focus();
        }
        async Task TakePhoto()
        {
            await CrossMedia.Current.Initialize();

            file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                CompressionQuality = 50,
                Name = "myimage.jpg",
                Directory = "sample"
            });
            if (file == null)
            {
                return;
            }
            // Convert file to byte array and set the resulting bitmap to imageview
            //byte[] imageArray = File.ReadAllBytes(file.Path.ToString());
           // imageaRray = imageArray;
            //Bitmap bitmaper = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
            chooseImageButton.Source = file.Path.ToString();
            //convertImagetoString(bitmaper);
        }
        async Task UploadPhoto()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Oops!", "Image is not supported on this device. Please try again.", "Okay");
                return;
            }

            file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Full,
                CompressionQuality = 60
            });
            if (file == null)
            {
                return;
            }
            // Convert file to byte array, to bitmap and set it to our ImageView

            // byte[] imageArray = System.IO.File.ReadAllBytes(file.Path);
            // Bitmap bitmap = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
            chooseImageButton.Source = file.Path.ToString();
        }

        private void universityEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue == string.Empty)
                btnUniversity.Text = "SKIP";
            else
            {
                btnUniversity.Text = "CONTINUE";
            }
        }

        private async void imagePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (imagePicker.SelectedIndex == 0)
            {
                await TakePhoto();
            }
            else if (imagePicker.SelectedIndex == 1)
            {
                await UploadPhoto();
            }
        }
    }
}