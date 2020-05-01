using Android.Graphics;
using SQLite;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Chatter.Model
{
    public class UserModel
    {
        [PrimaryKey]
        public string id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string gender { get; set; }
        public string location { get; set; }
        public string image { 
            get;
            set;
        } //base64
        public string about { get; set; } = "";
        public string job_title { get; set; } = "";
        public string company { get; set; } = "";
        public string school { get; set; } = "";
        public string city { get; set; } = "";
        public string show_age { get; set; } = "";
        public string show_distance { get; set; } = "";
        public string phone_number { get; set; } = "";
        public string birthdate
        { get; set; } = "";
    }   
}
