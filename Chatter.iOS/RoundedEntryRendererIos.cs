using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Chatter;
using Chatter.iOS;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;


[assembly: ExportRenderer(typeof(RoundedEntry), typeof(RoundedEntryRendererIos))]
namespace Chatter.iOS
{
    class RoundedEntryRendererIos : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                Control.Layer.CornerRadius = 20;
                Control.Layer.BorderWidth = 3f;
                Control.Layer.BorderColor = Xamarin.Forms.Color.DeepPink.ToCGColor();
                Control.Layer.BackgroundColor = Xamarin.Forms.Color.LightGray.ToCGColor();

                Control.LeftView = new UIKit.UIView(new CGRect(0, 0, 10, 0));
                Control.LeftViewMode = UIKit.UITextFieldViewMode.Always;
            }

            Control.TintColor = UIColor.FromRGB(152, 0, 11);
        }
    }
}