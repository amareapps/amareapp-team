using Android.Graphics;
using Chatter.Classes;
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
using Xamarin.Forms.Xaml;
using Color = Xamarin.Forms.Color;

namespace Chatter.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OtpAuthentication : ContentPage
    {
        ApiConnector api = new ApiConnector();
        private string number;
        public OtpAuthentication(string _number)
        {
            InitializeComponent();
            number = _number;
        }

        private async void confirmButton_Clicked(object sender,     EventArgs e)
        {
            if (!await api.checkCode(number, codeEntry.Text))
            {
                await DisplayAlert("Code", "Code mismatch!. Please try again", "Okay");
                return;
            }
            var user = await api.getUserModel(number);
            if(user == null)
            {
                await Navigation.PushAsync(new ProfileMaintenance(number), true);
            }
            else
            {
                Application.Current.Properties["Id"] = "\"" +user.id + "\"";
                await Navigation.PushModalAsync(new MainPage());
                await Navigation.PopToRootAsync(false);
            }
        }

        private void codeEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            Entry _entry = sender as Entry;
            if (string.IsNullOrEmpty(_entry.Text))
            {
                confirmButton.IsEnabled = false;
                confirmButton.BackgroundColor = Color.Gray;
            }
            else
            {
                confirmButton.IsEnabled = true;
                confirmButton.BackgroundColor = Color.DarkRed;
            }
        }

    }
}