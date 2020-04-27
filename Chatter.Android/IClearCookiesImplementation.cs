using Chatter.Droid;
using Xamarin.Forms;
using System.Net;
using Android.Webkit;
using Chatter.Classes;

[assembly: Dependency(typeof(IClearCookiesImplementation))]
namespace Chatter.Droid
{
    public class IClearCookiesImplementation : IClearCookies
    {
        public void Clear()
        {
            var cookieManager = CookieManager.Instance;
            cookieManager.RemoveAllCookie();
        }
    }
}