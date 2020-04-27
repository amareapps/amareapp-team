using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace Chatter.Classes
{
    class Base64toImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string base64Image = (string)value;
            if (base64Image == null)
                return null;
            byte[] Base64Stream = System.Convert.FromBase64String(base64Image);
            return ImageSource.FromStream(() =>  new MemoryStream(Base64Stream));
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
