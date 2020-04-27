using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
namespace Chatter.Classes
{
    class SmsSender
    {
        ApiConnector api = new ApiConnector();
        public async Task<bool> SendSms(string messageText, string recipient)  
        {  
            try  
            {
                object functionReturnValue = null;
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    System.Collections.Specialized.NameValueCollection parameter = new System.Collections.Specialized.NameValueCollection();
                    string url = "https://www.itexmo.com/php_api/api.php";
                    parameter.Add("1", recipient);
                    parameter.Add("2", "Your Amare code is : " + messageText);
                    parameter.Add("3", "TR-AMARE484804_AXPSX");
                    parameter.Add("passwd", "]lru2r7d##");
                    dynamic rpb = client.UploadValues(url, "POST", parameter);
                    functionReturnValue = (new System.Text.UTF8Encoding()).GetString(rpb);
                    await api.insertToPhoneRegister(recipient, messageText);
                }
            }  
            catch (FeatureNotSupportedException ex)  
            {
                return false;
            }  
            catch (Exception ex)  
            {
                return false;
            }
            return false;
        }
    }
}
