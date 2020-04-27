using Chatter.iOS;
using Xamarin.Forms;
using System.Net;
using Foundation;
using Chatter.Classes;

[assembly: Dependency(typeof(IClearCookiesImplementation))]
namespace Chatter.iOS
{
    public class IClearCookiesImplementation : IClearCookies
    {
        public void Clear()
        {
            NSHttpCookieStorage CookieStorage = NSHttpCookieStorage.SharedStorage;
            foreach (var cookie in CookieStorage.Cookies)
                CookieStorage.DeleteCookie(cookie);
        }
    }
}