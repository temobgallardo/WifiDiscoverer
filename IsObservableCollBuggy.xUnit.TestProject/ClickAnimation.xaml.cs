using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xunit;
using System.Linq;
using IsObservableCollBuggy.Triggers;
using IsObservableCollBuggy.Animations;

namespace IsObservableCollBuggy.xUnit.Tests
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClickAnimationTest : ContentPage
    {
        public ClickAnimationTest()
        {
            InitializeComponent();
        }

        public class Test : IDisposable
        {
            private ClickAnimationTest _page;
            private Label _animatedLabel;

            public Test()
            {
                _page = new ClickAnimationTest();
                _animatedLabel = _page.AnimatedLabel;
            }

            [Fact]
            public void ShouldHaveDifferentPropertyValues()
            {
                _animatedLabel.Triggers.RemoveAt(0);

                ClickAnimation _clickAnimation = new ClickAnimation();
                _clickAnimation.Scale = 1.04f;
                _clickAnimation.Target = _animatedLabel;
                _clickAnimation.Delay = 12;
                _clickAnimation.Duration = "123";
                _clickAnimation.Easing = Easing.CubicInOut;
                _clickAnimation.RepeatForever = true;

                BeginAnimation _beginAnimation = new BeginAnimation();
                _beginAnimation.Animation = _clickAnimation;

                EventTrigger _eventTrigger = new EventTrigger();
                _eventTrigger.Actions.Add(_beginAnimation);
                _eventTrigger.Event = "Clicked";
                _animatedLabel.Triggers.Add(_eventTrigger);

                TriggerBase triggerBase = _animatedLabel.Triggers.FirstOrDefault(t => t is TriggerBase);
                var et = (EventTrigger)triggerBase;
                var ba = (BeginAnimation)et.Actions.FirstOrDefault(a => a is BeginAnimation);

                Assert.NotNull(ba.Animation);

                var ca = (ClickAnimation) ba.Animation;

                Assert.Equal(ca.Target, _animatedLabel);
                Assert.NotEqual(ca.Delay, BaseAnimation.DelayProperty.DefaultValue);
                Assert.NotEqual(ca.Duration, BaseAnimation.DurationProperty.DefaultValue);
                Assert.NotEqual(ca.Easing, BaseAnimation.EasingProperty.DefaultValue);
                Assert.NotEqual(ca.RepeatForever, BaseAnimation.RepeatForeverProperty.DefaultValue);
                Assert.NotEqual(ca.Scale, ClickAnimation.ScaleProperty.DefaultValue);
            }

            [Fact]
            public void ShouldHaveDefaultValues()
            {
                TriggerBase triggerBase = _animatedLabel.Triggers.FirstOrDefault(t => t is TriggerBase);
                var et = (EventTrigger)triggerBase;
                var ba = (BeginAnimation)et.Actions.FirstOrDefault(a => a is BeginAnimation);

                Assert.NotNull(ba.Animation);

                var a = ba.Animation;

                Assert.Equal(a.Target, _animatedLabel);
                Assert.Equal(a.Delay, BaseAnimation.DelayProperty.DefaultValue);
                Assert.Equal(a.Duration, BaseAnimation.DurationProperty.DefaultValue);
                Assert.Equal(a.Easing, BaseAnimation.EasingProperty.DefaultValue);
                Assert.Equal(a.RepeatForever, BaseAnimation.RepeatForeverProperty.DefaultValue);

                var ca = (ClickAnimation)ba.Animation;

                Assert.Equal(ca.Scale, ClickAnimation.ScaleProperty.DefaultValue);
            }

            public void Dispose()
            {
                _animatedLabel = null;
                _page = null;
            }
        }
    }
}