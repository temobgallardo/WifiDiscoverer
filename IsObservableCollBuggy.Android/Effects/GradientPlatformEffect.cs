using Android.Graphics.Drawables;
using IsObservableCollBuggy.Droid.Effects;
using IsObservableCollBuggy.Effects;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(GradientPlatformEffect), nameof(GradientRoutingEffect))]
namespace IsObservableCollBuggy.Droid.Effects
{
    public class GradientPlatformEffect : BaseEffect
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
            if (!IsDisposed)
            {
                _view.Background = _orgDrawable;
                _view.ClipToOutline = false;
                System.Diagnostics.Debug.WriteLine($"{this.GetType().FullName} Detached Disposing");
            }

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
                args.PropertyName == Gradient.IsEnableProperty.PropertyName)
            {
                UpdateGradient();
            }
        }

        void UpdateGradient()
        {
            var colors = Gradient.GetColors(Element);
            if (colors == null) return;

            _gradient.SetColors(colors.Select(x => (int)x.ToAndroid()).ToArray());
            _gradient.SetOrientation(ConvertOrientation());
            _gradient.SetAlpha(GetAlpha());

            _view.ClipToOutline = true; //not to overflow children
            _view.SetBackground(_gradient);
        }

        int GetAlpha()
        {
            var enabled = Gradient.GetIsEnable(Element);

            if (enabled) return 200;

            return 280;
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