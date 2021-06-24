using Android.Graphics.Drawables;
using Android.Util;
using Android.Views;
using IsObservableCollBuggy.Effects;
using System;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Android.Views.View;

[assembly: ExportEffect(typeof(IsObservableCollBuggy.Droid.Effects.GradientBackgroundColorEffect), nameof(GradientBackgroundColorEffect))]
namespace IsObservableCollBuggy.Droid.Effects
{
    public class GradientBackgroundColorEffect : PlatformEffect
    {
        public const string TAG = nameof(GradientBackgroundColorEffect);
        private GradientDrawable _gradient;

        protected override void OnAttached()
        {
            try
            {
                var element = (IsObservableCollBuggy.Effects.GradientBackgroundColorEffect)Element.Effects.FirstOrDefault(element => element is IsObservableCollBuggy.Effects.GradientBackgroundColorEffect);
                if (element != null)
                {
                    _gradient = GetGradient(element.GradientColors);
                    Control.SetBackground(_gradient);
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, $"{nameof(OnAttached)} - {e.Message}. Trace: {e.StackTrace}");
            }
        }

        protected override void OnDetached()
        {
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            try
            {
                if (args.PropertyName == "GradientColors")
                {
                    var element = (IsObservableCollBuggy.Effects.GradientBackgroundColorEffect)Element.Effects.FirstOrDefault(element => element is IsObservableCollBuggy.Effects.GradientBackgroundColorEffect);
                    if (element == null) return;

                    var gradients = element.GradientColors;
                    if (gradients == null) return;

                    _gradient = GetGradient(gradients);
                    Control.SetBackground(_gradient);
                }

                if (args.PropertyName == nameof(GradientEffect.IsEnableProperty))
                {
                    UpdateAlpha();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot set property on attached control. Error: ", ex.Message);
            }
        }

        /// <summary>
        /// Draw the gradient with the correct oppacity
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UpdateAlpha()
        {
            if (GradientEffect.GetIsEnable(Element))
            {
                _gradient.Alpha = 200;
                Control.SetBackground(_gradient);
            }
            else
            {
                _gradient.Alpha = 255;
                Control.SetBackground(_gradient);
            }
        }

        GradientDrawable GetGradient(GradientColors colors)
        {
            if (colors == null)
            {
                return null;
            }

            var gradient = new GradientDrawable(GradientDrawable.Orientation.LeftRight, colors.Select(c => (int)c.ToAndroid()).ToArray());

            return gradient;
        }
    }
}