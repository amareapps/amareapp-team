using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Chatter.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnimateMatched
    {
        public string ImageOne, ImageTwo;
        public AnimateMatched(string userImage,string likedUserImage)
        {
            InitializeComponent();
            ImageOne = userImage;
            ImageTwo = likedUserImage;
        }
        protected async override void OnAppearing()
        {
            yourImage.Source = ImageOne;
            othersImage.Source = ImageTwo;
            await Task.Delay(1000);
            yourImage.RotateTo(360,1500);
            othersImage.RotateTo(-360, 1500);
            yourImage.TranslateTo(100,0, 1500);
            othersImage.TranslateTo(-100, 0, 1500);
            yourImage.ScaleTo(0, 1500);
            othersImage.ScaleTo(0, 1500);
            heartImage.IsVisible = true;
            heartImage.FadeTo(1, 2000);
            await heartImage.ScaleTo(0,100);
            await heartImage.ScaleTo(1,2000);

        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

    }
}