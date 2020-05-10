using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chatter.Classes
{
    public class ImageOption
    {
        MediaFile file;
        public async Task<MediaFile> TakePhoto()
        {
            await CrossMedia.Current.Initialize();
            file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                CompressionQuality = 40,
                Name = "myimage.jpg",
                Directory = "sample"
            });
            if (file == null)
            {
                return null;
            }
            return file;
            // Convert file to byte array and set the resulting bitmap to imageview
            // byte[] imageArray = System.IO.File.ReadAllBytes(file.Path);
            //imageaRray = imageArray;
            //Bitmap bitmaper = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
//            chooseImageButton.Source = file.Path.ToString();
            //convertImagetoString(bitmaper);
        }
        public async Task<MediaFile> UploadPhoto()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                return null;
            }

            file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                CompressionQuality = 40
            });
            if (file == null)
            {
                return null;
            }
            // Convert file to byte array, to bitmap and set it to our ImageView

            // byte[] imageArray = System.IO.File.ReadAllBytes(file.Path);
            // Bitmap bitmap = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
            return file;
        }
    }
}
