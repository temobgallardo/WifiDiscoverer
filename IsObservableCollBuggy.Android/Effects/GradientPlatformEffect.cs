using Android.Animation;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Views;
using IsObservableCollBuggy.Droid.Effects;
using IsObservableCollBuggy.Droid.Effects.GestureCollectors;
using IsObservableCollBuggy.Effects;
using System;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AButton = Android.Widget.Button;
using XFColor = Xamarin.Forms.Color;
using ListView = Android.Widget.ListView;
using ScrollView = Android.Widget.ScrollView;

[assembly: ExportEffect(typeof(GradientPlatformEffect), nameof(GradientRoutingEffect))]
namespace IsObservableCollBuggy.Droid.Effects
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class GradientPlatformEffect : PlatformEffect
    {
        private Android.Views.View View => Container ?? Control;
        private GradientDrawable _gradient;
        private Drawable _orgDrawable;

        protected bool IsSupportedByApi => Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop;

        public const string TAG = nameof(GradientPlatformEffect);

        protected override void OnAttached()
        {
            if (Control is ListView || Control is ScrollView) return;

            _gradient = new GradientDrawable();
            _orgDrawable = View.Background;
            View.Background = GetBackground();

            System.Diagnostics.Debug.WriteLine($"{GetType().FullName} - {nameof(OnAttached)} - Attached completely");
        }

        protected override void OnDetached()
        {
            View.Background = _orgDrawable;
            _gradient?.Dispose();
            _gradient = null;

            System.Diagnostics.Debug.WriteLine($"{GetType().FullName} Detached completely");
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine($"{GetType().FullName} - {nameof(OnElementPropertyChanged)}");
            base.OnElementPropertyChanged(args);

            if (!IsSupportedByApi)
                return;

            if (args.PropertyName == Gradient.ColorsProperty.PropertyName ||
                args.PropertyName == Gradient.OrientationProperty.PropertyName ||
                args.PropertyName == Gradient.CornerRadiusProperty.PropertyName ||
                args.PropertyName == Gradient.TouchColorProperty.PropertyName)
            {
                View.Background = GetBackground();
            }
            else if (args.PropertyName == Gradient.IsEnableProperty.PropertyName)
            {
                View.Background = GetBackgrounWithTextColor();
            }
        }

        private void UpdateGradient()
        {
            System.Diagnostics.Debug.WriteLine($"{GetType().FullName} - {nameof(UpdateGradient)}");
            var colors = Gradient.GetColors(Element);
            if (colors == null) return;

            _gradient.SetColors(colors.Select(x => (int)x.ToAndroid()).ToArray());
            _gradient.SetOrientation(ConvertOrientation());
            _gradient.SetAlpha(GetAlpha());
            _gradient.SetCornerRadius(Gradient.GetCornerRadius(Element));

            View.ClipToOutline = true; //not to overflow children
        }

        private Drawable GetBackgrounWithTextColor()
        {
            UpdateGradient();
            UpdateDisabledGradient();
            return CreateDrawable();
        }

        private void UpdateDisabledGradient()
        {
            var enabled = Gradient.GetIsEnable(Element);
            if (enabled) return;

            switch (View)
            {
                case AButton button:
                    button.SetTextColor(XFColor.White.ToAndroid());
                    break;
                case Android.Widget.EditText editView:
                    editView.SetTextColor(XFColor.White.ToAndroid());
                    break;
                case Android.Widget.TextView textView:
                    textView.SetTextColor(XFColor.White.ToAndroid());
                    break;
                default:
                    break;
            }
        }

        private int GetAlpha()
        {
            var enabled = Gradient.GetIsEnable(Element);

            if (enabled) return 250;

            return 90;
        }

        private GradientDrawable.Orientation ConvertOrientation()
        {
            var orientation = Gradient.GetOrientation(Element);

            return orientation switch
            {
                GradientOrientation.LeftRight => GradientDrawable.Orientation.LeftRight,
                GradientOrientation.BlTr => GradientDrawable.Orientation.BlTr,
                GradientOrientation.BottomTop => GradientDrawable.Orientation.BottomTop,
                GradientOrientation.BrTl => GradientDrawable.Orientation.BrTl,
                GradientOrientation.RightLeft => GradientDrawable.Orientation.RightLeft,
                GradientOrientation.TrBl => GradientDrawable.Orientation.TrBl,
                GradientOrientation.TopBottom => GradientDrawable.Orientation.TopBottom,
                GradientOrientation.TlBr => GradientDrawable.Orientation.TlBr,
                _ => GradientDrawable.Orientation.LeftRight,
            };
        }


        private Drawable GetBackground()
        {
            UpdateGradient();

            return CreateDrawable();
        }

        private Drawable CreateDrawable()
        {
            var back = View.Background;

            if (back is RippleDrawable)
                return new RippleDrawable(GetPressedColorSelector(Gradient.GetTouchColor(Element).ToAndroid()), _gradient, null);

            return _gradient;
        }

        private static ColorStateList GetPressedColorSelector(int pressedColor)
        {
            return new ColorStateList(
                new[] { new int[] { } },
                new[] { pressedColor, });
        }
    }
}