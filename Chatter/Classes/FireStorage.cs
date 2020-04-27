using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Chatter.Classes
{
    class FireStorage
    {
        public async Task<string> StoreImages(Stream imageStream,string name)
        {
            var stroageImage = await new FirebaseStorage("chatter-7b8e4.appspot.com")
                .Child("UserImages")
                .Child(name+".png")
                .PutAsync(imageStream);
            string imgurl = stroageImage;
            return imgurl;
        }
    }
}
