using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chatter.Classes;
using Chatter.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(NotificationHelper))]
namespace Chatter.iOS
{
    class NotificationHelper : INotification
    {
        public void CreateNotification(string title, string message)
        {
            new NotificationDelegate().RegisterNotification(title, message);
        }
    }
}