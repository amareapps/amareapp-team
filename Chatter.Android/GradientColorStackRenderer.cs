using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Chatter.Classes;
using MyUIDemo.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(GradientColorStack), typeof(GradientButtonRenderer))]
namespace MyUIDemo.Droid.CustomRenderer
{
    public class GradientButtonRenderer : ButtonRenderer
    {
        public GradientButtonRenderer(Context context) : base(context)
        {
        }

        protected override void DispatchDraw(Canvas canvas)
        {

            LinearGradient gradient = null;

            // For Horizontal Gradient
            if (((GradientColorStack)Element).GradientColorOrientation == GradientColorStack.GradientOrientation.Horizontal)
            {
                gradient = new Android.Graphics.LinearGradient(0, 0, Width, 0,



                     ((GradientColorStack)Element).StartColor.ToAndroid(),
                     ((GradientColorStack)Element).EndColor.ToAndroid(),

                     Android.Graphics.Shader.TileMode.Mirror);

            }
            //For Veritical Gradient
            if (((GradientColorStack)Element).GradientColorOrientation == GradientColorStack.GradientOrientation.Vertical)
            {
                gradient = new Android.Graphics.LinearGradient(0, 0, 0, Height,



                     ((GradientColorStack)Element).StartColor.ToAndroid(),
                     ((GradientColorStack)Element).EndColor.ToAndroid(),

                     Android.Graphics.Shader.TileMode.Mirror);

            }

            var paint = new Android.Graphics.Paint()
            {
                Dither = true,
            };
            paint.SetShader(gradient);
            canvas.DrawPaint(paint);
            base.DispatchDraw(canvas);
        }

    }

}