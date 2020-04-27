using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinFirebase.Helper;

namespace Chatter.Model
{
    public class UserModelStorage
    {
        public int Id
        {
            get;
            set;
        }
        public string Image64 
        { 
            get; 
            set; 
        }
        public string UserName
        {
            get;
            set;
        }
        public string Password
        {
            get;
            set;
        }
        public string Email
        {
            get;
            set;
        }
        public string Birthday
        {
            get;
            set;
        }
        public string Gender
        {
            get;
            set;
        }
    }
}
