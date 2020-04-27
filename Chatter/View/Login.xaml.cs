using Android.Widget;
using Chatter.Classes;
using Chatter.Model;
using Chatter.View;
using Java.Sql;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using eliteKit;

namespace Chatter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {

        SmsSender smsSender = new SmsSender();
        public class SocialMediaPlatform
        {
            public static readonly int Facebook = 0;
            public static readonly int Instagram = 1;
            public static readonly int Google = 2;
        }
        public Login()
        {
            InitializeComponent();

        }
        private void registerButton_Clicked(object sender, EventArgs e)
        {
            var edit = new ProfileMaintenance("");
            Navigation.PushAsync(edit);
        }

        private async void LoginButton_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new Login_Input());
        }

        private async void fbLoginButton_Clicked(object sender, EventArgs e)
        {
            /*
             var checker = await smsSender.SendSms("Hi!!","09750484804");
             if (checker)
                 await DisplayAlert("Yehey!","Successfully sent","Nice");
             else
                 await DisplayAlert("Nyek!", "May error", "Okay");

             */
            await Navigation.PushModalAsync(new SocialMediaLogin(SocialMediaPlatform.Facebook));
        }

        private async void phoneRegister_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NumberLogin(),true);
        }
    }
}