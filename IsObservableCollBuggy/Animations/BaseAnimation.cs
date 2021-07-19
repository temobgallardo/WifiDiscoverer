using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace IsObservableCollBuggy.Animations
{
    public abstract class BaseAnimation : BindableObject
    {
        private CancellationTokenSource _animateTimerCancellationTokenSource;

        public static readonly BindableProperty TargetProperty =
            BindableProperty.Create(nameof(Target), typeof(VisualElement), typeof(BaseAnimation), null,
                BindingMode.TwoWay, null);

        public VisualElement Target
        {
            get { return (VisualElement)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly BindableProperty DurationProperty =
            BindableProperty.Create(nameof(Duration), typeof(string), typeof(BaseAnimation), "1000",
                BindingMode.TwoWay, null);

        public string Duration
        {
            get { return (string)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly BindableProperty EasingProperty =
            BindableProperty.Create(nameof(Easing), typeof(Easing), typeof(BaseAnimation), Easing.Linear,
                BindingMode.TwoWay, null);

        public Easing Easing
        {
            get { return (Easing)GetValue(EasingProperty); }
            set { SetValue(EasingProperty, value); }
        }

        public static readonly BindableProperty DelayProperty =
          BindableProperty.Create("Delay", typeof(int), typeof(BaseAnimation), 0, propertyChanged: (bindable, oldValue, newValue) =>
              ((BaseAnimation)bindable).Delay = (int)newValue);

        public int Delay
        {
            get { return (int)GetValue(DelayProperty); }
            set { SetValue(DelayProperty, value); }
        }

        public static readonly BindableProperty RepeatForeverProperty =
          BindableProperty.Create("RepeatForever", typeof(bool), typeof(BaseAnimation), false, propertyChanged: (bindable, oldValue, newValue) =>
              ((BaseAnimation)bindable).RepeatForever = (bool)newValue);

        public bool RepeatForever
        {
            get { return (bool)GetValue(RepeatForeverProperty); }
            set { SetValue(RepeatForeverProperty, value); }
        }

        protected abstract Task BeginAnimation();

        public async Task Begin()
        {
            if (Delay > 0)
            {
                await Task.Delay(Delay);
            }

            if (!RepeatForever)
            {
                await BeginAnimation();
            }
            else
            {
                RepeatAnimation(new CancellationTokenSource());
            }
        }

        public void End()
        {
            ViewExtensions.CancelAnimations(Target);

            if (_animateTimerCancellationTokenSource != null)
            {
                _animateTimerCancellationTokenSource.Cancel();
            }
        }

        internal void RepeatAnimation(CancellationTokenSource tokenSource)
        {
            _animateTimerCancellationTokenSource = tokenSource;

            Device.BeginInvokeOnMainThread(async () =>
            {
                if (!_animateTimerCancellationTokenSource.IsCancellationRequested)
                {
                    await BeginAnimation();

                    RepeatAnimation(_animateTimerCancellationTokenSource);
                }
            });
        }
    }
}
