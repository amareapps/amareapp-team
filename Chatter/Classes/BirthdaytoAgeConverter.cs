using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;
namespace Chatter.Classes
{
    class BirthdaytoAgeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var today = DateTime.Today;
            var birthdate = System.Convert.ToDateTime(value.ToString());
            // Calculate the age.
            var age = today.Year - birthdate.Year;
            return age;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
