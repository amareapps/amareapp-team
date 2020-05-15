using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace Chatter.Model
{
    public class InboxModel
    {
        [PrimaryKey]
        public string session_id { get; set; }
        public string user_id { get; set; } //user id
        public string username { get; set; }
        public string message { get; set; }
        public string image { get; set; }
        public string datetime { get; set; }
        public string emoji { get; set; }
    }
}
