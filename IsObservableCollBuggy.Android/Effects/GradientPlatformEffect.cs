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
            //if (!IsDisposed)
            //{
            //    try
            //    {
            //        _view.Background = _orgDrawable;
            //        _view.ClipToOutline = false;
            //    }
            //    catch (ObjectDisposedException e)
            //    {
            //        System.Diagnostics.Debug.WriteLine($"{this.GetType().FullName} Detached Disposing");
            //    }
            //    System.Diagnostics.Debug.WriteLine($"{this.GetType().FullName} Detached Disposing");
            //}

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

            switch (orientation)
            {
                case GradientOrientation.LeftRight:
                    return GradientDrawable.Orientation.LeftRight;
                case GradientOrientation.BlTr:
                    return GradientDrawable.Orientation.BlTr;
                case GradientOrientation.BottomTop:
                    return GradientDrawable.Orientation.BottomTop;
                case GradientOrientation.BrTl:
                    return GradientDrawable.Orientation.BrTl;
                case GradientOrientation.RightLeft:
                    return GradientDrawable.Orientation.RightLeft;
                case GradientOrientation.TrBl:
                    return GradientDrawable.Orientation.TrBl;
                case GradientOrientation.TopBottom:
                    return GradientDrawable.Orientation.TopBottom;
                case GradientOrientation.TlBr:
                    return GradientDrawable.Orientation.TlBr;
                default:
                    return GradientDrawable.Orientation.LeftRight;
            }
        }
    }
}