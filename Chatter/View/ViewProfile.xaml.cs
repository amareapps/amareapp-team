using Android.Database;
using Chatter.Classes;
using Chatter.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Chatter.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewProfile 
    {
        UserModel userModel = new UserModel();
        ApiConnector api = new ApiConnector();
        ObservableCollection<List<GalleryModel>> galleryModel = new ObservableCollection<List<GalleryModel>>();
        string userId;
        public ViewProfile(string id)
        {
            InitializeComponent();
            userId = id;
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await loadUser();
        }
        async Task loadUser()
        {
            try
            {
                var user = await api.getSpeificUser(userId);
                var list = await api.otherUserImageList(userId);
                BindingContext = user;
                galleryModel.Add(list);
                galleryView.ItemsSource = list;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error",ex.ToString(),"Okay");
            }
        }   
    }
}