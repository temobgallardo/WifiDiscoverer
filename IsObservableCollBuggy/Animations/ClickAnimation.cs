using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace IsObservableCollBuggy.Animations
{
    public class ClickAnimation : BaseAnimation
    {
        public float Scale
        {
            get => (float)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }
        public static BindableProperty ScaleProperty = BindableProperty.Create(
            propertyName: "ScaleProperty",
            returnType: typeof(float),
            declaringType: typeof(ClickAnimation),
            defaultValue: 1.0f);

        protected override Task BeginAnimation()
        {
            if (Target == null)
            {
                throw new NullReferenceException("Null Target property.");
            }

            return Task.Run(() =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Target.Animate("BounceIn", ScaleUpAndDown(), 16, Convert.ToUInt32(Duration));
                });
            });
        }

        internal Animation ScaleUpAndDown()
        {
            var animate = new Animation();
            var scaleUp = new Animation(v => Target.Scale = v, 1, Scale);
            var scaleDown = new Animation(v => Target.Scale = v, Scale, 1);

            animate.Add(0, 0.5, scaleUp);
            animate.Add(0.5, 1, scaleDown);

            return animate;
        }
    }
}
