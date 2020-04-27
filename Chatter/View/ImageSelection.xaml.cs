using Android.Graphics;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Chatter.Model;
using Android.Util;
using System.Net.Http;
using Firebase.Storage;
using Plugin.Media.Abstractions;
using Xamarin.Essentials;

namespace Chatter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageSelection : ContentPage
    {
        string imageString;
        string locationString = "";
        MediaFile file;
        public ImageSelection()
        {
            InitializeComponent();
            BindingContext = new UserModelStorage();
        }
        async void TakePhoto()
        {
            await CrossMedia.Current.Initialize();

            file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small,
                CompressionQuality = 10,
                Name = "myimage.jpg",
                Directory = "sample"
            });
            if (file == null)
            {
                return;
            }
            // Convert file to byte array and set the resulting bitmap to imageview
           // byte[] imageArray = System.IO.File.ReadAllBytes(file.Path);
            //imageaRray = imageArray;
            //Bitmap bitmaper = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
            ProfileImage.Source = file.Path.ToString();
            //convertImagetoString(bitmaper);
        }
        async void UploadPhoto()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Upload not supported on this device", "sdadsa", "Okay");
                return;
            }

            file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small,
                CompressionQuality = 10
            });
            // Convert file to byte array, to bitmap and set it to our ImageView

           // byte[] imageArray = System.IO.File.ReadAllBytes(file.Path);
           // Bitmap bitmap = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
            ProfileImage.Source = file.Path.ToString();
        }
        private async Task uploadtoServer()
        {
            await StoreImages(file.GetStream());
        }
        private void takePhotoButton_Clicked(object sender, EventArgs e)
        {
            TakePhoto();
        }
        private void choosePhotoButton_Clicked(object sender, EventArgs e)
        {
            UploadPhoto();
        }
        private async void doneButton_Clicked(object sender, EventArgs e)
        {
            var request = new GeolocationRequest(GeolocationAccuracy.High);
            var location = await Geolocation.GetLocationAsync(request);
            
            if (location == null)
            {
                return;
            }
            locationString = location.Latitude.ToString() + "," + location.Longitude.ToString();
            loadingActivity.IsRunning = true;
            if (ProfileImage.Source.ToString() == "no_image.jpg")
          {
               await DisplayAlert("Image Selection", "Picture is required", "Okay");
               return;
           }
            await uploadtoServer();
            await sampless();
            loadingActivity.IsRunning = false;
        }
        private async Task sampless()
        {
            var client = new HttpClient();
            var form = new MultipartFormDataContent();
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StringContent(Application.Current.Properties["Email"].ToString()), "email");
            content.Add(new StringContent(Application.Current.Properties["Password"].ToString()), "password");
            content.Add(new StringContent(Application.Current.Properties["Name"].ToString()), "username");
            content.Add(new StringContent(Application.Current.Properties["Gender"].ToString()), "gender");
            content.Add(new StringContent(locationString),"location");
            content.Add(new StringContent(imageString), "image");
            var request = await client.PostAsync("http://" + ApiConnection.Url + "/apier/api/test_api.php?action=insert", content);
            request.EnsureSuccessStatusCode();
            var response = await request.Content.ReadAsStringAsync();
            var exec = await DisplayAlert("Congratulations!","You are succesfully registered",null,"OK");
            if (exec == false)
            {
                await Navigation.PushModalAsync(new Login());
            }
        }
        public async Task StoreImages(Stream imageStream)
        {
            var stroageImage = await new FirebaseStorage("chatter-7b8e4.appspot.com")
                .Child("UserImages")
                .Child(Application.Current.Properties["Email"].ToString().Replace(".","") + ".jpg")
                .PutAsync(imageStream);
            string imgurl = stroageImage;
            imageString = imgurl;
        }
    }
}