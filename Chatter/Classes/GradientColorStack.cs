using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
namespace Chatter.Classes
{
    public class GradientColorStack : Button
    {
        public enum GradientOrientation
        {
            Vertical,
            Horizontal
        }
        //properties
        public Color StartColor
        {
            get; set;
        }

        public Color EndColor
        {
            get; set;
        }

        public GradientOrientation GradientColorOrientation
        {
            get; set;
        }
    }
}
