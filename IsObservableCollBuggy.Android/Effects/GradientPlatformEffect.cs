using Android.Graphics.Drawables;
using IsObservableCollBuggy.Droid.Effects;
using IsObservableCollBuggy.Effects;
using System;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(GradientPlatformEffect), nameof(GradientRoutingEffect))]
namespace IsObservableCollBuggy.Droid.Effects
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class GradientPlatformEffect : BasePlatformEffect
    {
        private Android.Views.View _view;
        private GradientDrawable _gradient;
        private Drawable _orgDrawable;

        public const string TAG = nameof(GradientPlatformEffect);

        protected override void OnAttachedOverride()
        {
            _view = Container ?? Control;

            _gradient = new GradientDrawable();
            _orgDrawable = _view.Background;

            UpdateGradient();
        }

        protected override void OnDetachedOverride()
        {
            _gradient?.Dispose();
            _gradient = null;
            _view = null;
            System.Diagnostics.Debug.WriteLine($"{this.GetType().FullName} Detached completely");
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if (!IsSupportedByApi)
                return;

            if (IsDisposed)
            {
                return;
            }

            if (args.PropertyName == Gradient.ColorsProperty.PropertyName ||
                args.PropertyName == Gradient.OrientationProperty.PropertyName ||
                args.PropertyName == Gradient.CornerRadiusProperty.PropertyName)
            {
                UpdateGradient();
            }
            else if (args.PropertyName == Gradient.IsEnableProperty.PropertyName)
            {
                UpdateDisabledGradient();
            }
        }

        void UpdateGradient()
        {
            var colors = Gradient.GetColors(Element);
            if (colors == null) return;

            _gradient.SetColors(colors.Select(x => (int)x.ToAndroid()).ToArray());
            _gradient.SetOrientation(ConvertOrientation());
            _gradient.SetAlpha(GetAlpha());
            _gradient.SetCornerRadius(Gradient.GetCornerRadius(Element));

            _view.ClipToOutline = true; //not to overflow children
            _view.SetBackground(_gradient);
        }
        void UpdateDisabledGradient()
        {
            UpdateGradient();

            var enabled = Gradient.GetIsEnable(Element);
            if (enabled) return;

            switch (_view)
            {
                case Android.Widget.Button button:
                    button.SetTextColor(Color.White.ToAndroid());
                    break;
                case Android.Widget.EditText editView:
                    editView.SetTextColor(Color.White.ToAndroid());
                    break;
                case Android.Widget.TextView textView:
                    textView.SetTextColor(Color.White.ToAndroid());
                    break;
                default:
                    break;
            }
        }

        int GetAlpha()
        {
            var enabled = Gradient.GetIsEnable(Element);

            if (enabled) return 250;//200;

            return 90;//280;
        }

        GradientDrawable.Orientation ConvertOrientation()
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
    }
}