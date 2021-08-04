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
        private Android.Views.View _view => Container ?? Control;
        private GradientDrawable _gradient;
        private Drawable _orgDrawable;
        private RippleDrawable _ripple;
        private ObjectAnimator _animator;
        private Android.Graphics.Color _color;
        private byte _alpha;

        protected bool IsSupportedByApi => Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop;

        public const string TAG = nameof(GradientPlatformEffect);

        protected override void OnAttached()
        {
            if (Control is ListView || Control is ScrollView)
            {
                return;
            }

            /// Those two properties ensure the view to not react to clicks outside its boundaries.
            _view.Clickable = true;
            _view.LongClickable = true;
            _gradient = new GradientDrawable();
            _orgDrawable = _view.Background;

            UpdateGradient();

            if (!(_view is AButton))
                TouchCollector.Add(_view, OnTouch);

            System.Diagnostics.Debug.WriteLine($"{GetType().FullName} - {nameof(OnAttached)} - Attached completely");
        }

        protected override void OnDetached()
        {
            _view.Background = _orgDrawable;
            _gradient?.Dispose();
            _gradient = null;

            if (IsSupportedByApi)
            {
                _ripple?.Dispose();
                _ripple = null;
            }

            TouchCollector.Delete(_view, OnTouch);
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
                UpdateGradient();
            }
            else if (args.PropertyName == Gradient.IsEnableProperty.PropertyName)
            {
                UpdateDisabledGradient();
            }
        }

        void UpdateGradient()
        {
            System.Diagnostics.Debug.WriteLine($"{GetType().FullName} - {nameof(UpdateGradient)}");
            var colors = Gradient.GetColors(Element);
            if (colors == null) return;

            _gradient.SetColors(colors.Select(x => (int)x.ToAndroid()).ToArray());
            _gradient.SetOrientation(ConvertOrientation());
            _gradient.SetAlpha(GetAlpha());
            _gradient.SetCornerRadius(Gradient.GetCornerRadius(Element));

            _view.ClipToOutline = true; //not to overflow children

            var rippleColor = Gradient.GetTouchColor(Element);
            if (IsSupportedByApi && rippleColor != default)
            {
                System.Diagnostics.Debug.WriteLine($"{GetType().FullName} - {nameof(UpdateGradient)} - Setting ripple");
                SetTouchEffectColor(rippleColor);
                _ripple = new RippleDrawable(GetPressedColorSelector(_color), content: _gradient, null);
                _view.Background = _ripple;
                //_view.InvalidateDrawable(_ripple);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"{GetType().FullName} - {nameof(UpdateGradient)} - Setting drawable");
                _view.Background = _gradient;
                //_view.InvalidateDrawable(_ripple);
            }

            //_view.Background?.InvalidateSelf();
            //_view.RefreshDrawableState();
        }

        void UpdateDisabledGradient()
        {
            UpdateGradient();

            var enabled = Gradient.GetIsEnable(Element);
            if (enabled) return;

            switch (_view)
            {
                case Android.Widget.Button button:
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

        int GetAlpha()
        {
            var enabled = Gradient.GetIsEnable(Element);

            if (enabled) return 250;

            return 90;
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

        void SetTouchEffectColor(XFColor color)
        {
            if (color == XFColor.Default) return;

            _color = color.ToAndroid();
            _alpha = _color.A == 255 ? (byte)80 : _color.A;
        }

        void OnTouch(Android.Views.View.TouchEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine($"{GetType().FullName} - {nameof(OnTouch)}");

            switch (args.Event.Action)
            {
                case MotionEventActions.Down:
                    System.Diagnostics.Debug.WriteLine($"{GetType().FullName} - {nameof(OnTouch)} - Down motion action");

                    if (IsSupportedByApi)
                        ForceStartRipple(args.Event.GetX(), args.Event.GetY());
                    else
                        BringLayer();

                    break;
                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                    System.Diagnostics.Debug.WriteLine($"{GetType().FullName} - {nameof(OnTouch)} - Up or Cancel motion action");

                    if (IsSupportedByApi)
                        ForceEndRipple();
                    else
                        TapAnimation(250, _alpha, 0);

                    break;
            }
        }

        #region ripple
        RippleDrawable CreateRipple(Android.Graphics.Color color)
        {
            if (Element is Xamarin.Forms.Layout)
            {
                var mask = new ColorDrawable(Android.Graphics.Color.White);
                return _ripple = new RippleDrawable(GetPressedColorSelector(color), null, mask);
            }

            var back = _view.Background;
            if (back == null)
            {
                var mask = new ColorDrawable(Android.Graphics.Color.White);
                return _ripple = new RippleDrawable(GetPressedColorSelector(color), null, mask);
            }

            if (back is RippleDrawable)
            {
                _ripple = (RippleDrawable)back.GetConstantState().NewDrawable();
                _ripple.SetColor(GetPressedColorSelector(color));

                return _ripple;
            }

            return _ripple = new RippleDrawable(GetPressedColorSelector(color), back, null);
        }

        static ColorStateList GetPressedColorSelector(int pressedColor)
        {
            return new ColorStateList(
                new[] { new int[] { } },
                new[] { pressedColor, });
        }

        void ForceStartRipple(float x, float y)
        {
            if (!(_view.Background is RippleDrawable bc)) return;

            bc.SetHotspot(x, y);
            _view.Pressed = true;
        }

        void ForceEndRipple()
        {
            _view.Pressed = false;
        }
        #endregion

        #region Overlay

        void BringLayer()
        {
            ClearAnimation();

            var color = _color;
            color.A = _alpha;
            _view.SetBackgroundColor(color);
        }

        void TapAnimation(long duration, byte startAlpha, byte endAlpha)
        {
            var start = _color;
            var end = _color;
            start.A = startAlpha;
            end.A = endAlpha;

            ClearAnimation();
            _animator = ObjectAnimator.OfObject(_view,
                "BackgroundColor",
                new ArgbEvaluator(),
                start.ToArgb(),
                end.ToArgb());
            _animator.SetDuration(duration);
            _animator.RepeatCount = 0;
            _animator.RepeatMode = ValueAnimatorRepeatMode.Restart;
            _animator.Start();
            _animator.AnimationEnd += AnimationOnAnimationEnd;
        }

        void AnimationOnAnimationEnd(object sender, EventArgs eventArgs)
        {
            ClearAnimation();
        }

        void ClearAnimation()
        {
            if (_animator == null) return;
            _animator.AnimationEnd -= AnimationOnAnimationEnd;
            _animator.Cancel();
            _animator.Dispose();
            _animator = null;
        }

        #endregion
    }
}