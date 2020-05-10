using Android.App;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Application = Xamarin.Forms.Application;

namespace Chatter.Model
{
    public class ChatModel
    {
        public string id { get; set; }
        public string session_id { get; set; }
        public string sender_id { get; set; }
        public string receiver_id { get; set; }
        public string sender_username { get; set; }
        public string message { get; set; }
        public string image { get; set; } = "";
        public string datetime { get; set; }
    }
}
