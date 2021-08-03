using IsObservableCollBuggy.Effects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xunit;

namespace IsObservableCollBuggy.xUnit.Tests
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GradientTest : ContentPage
    {
        public GradientTest()
        {
            InitializeComponent();
        }

        public class Test : IDisposable
        {
            private GradientTest gd;
            private Label label;
            private Button btn;

            public Test()
            {
                gd = new GradientTest();
                label = gd.GradientLabel;
                btn = gd.GradientButton;
            }

            [Fact]
            public void GradientColorsAreCorrect()
            {
                var expected = new GradientColors(new Color[] { Color.FromHex("#1285A5"), Color.FromHex("#7FC8C4") });

                Assert.Equal(expected, Gradient.GetColors(label));
                Assert.Equal(expected, Gradient.GetColors(btn));
            }

            [Theory]
            [ClassData(typeof(GradientColorsTestData))]
            public void ShouldBeSameGradientColors(GradientColors actual, GradientColors expected)
            {
                Gradient.SetColors(label, actual);
                Assert.Equal(expected, Gradient.GetColors(label));
            }

            [Fact]
            public void ShouldBeOneEffectAtATime()
            {
                var expected = 1;
                var gradient = new GradientColors(new Color[] { Color.FromHex("#1285A5"), Color.FromHex("#7FC8C4") });

                Gradient.SetColors(label, gradient);
                Gradient.SetColors(label, gradient);

                Assert.Equal(expected, label.Effects.Count);
            }

            [Fact]
            public void ShouldNotAddEffectIfGradientColorsNull()
            {
                var expected = 0;
                GradientColors gradient = null;

                Gradient.SetColors(label, gradient);
                Gradient.SetColors(label, gradient);

                Assert.Equal(expected, label.Effects.Count);
            }

            [Fact]
            public void ShouldNotAddTouchIfDefault()
            {
                Color expected = default;
                GradientColors gradient = new GradientColors(new Color[] { Color.FromHex("#1285A5"), Color.FromHex("#7FC8C4") });

                Gradient.SetColors(label, gradient);
                Gradient.SetTouchColor(label, expected);

                Assert.Equal(expected, Gradient.GetTouchColor(label));
            }

            [Fact]
            public void ShouldHaveGradient()
            {
                RemoveAllEffects();

                GradientColors gradient = new GradientColors(new Color[] { Color.FromHex("#1285A5"), Color.FromHex("#7FC8C4") });

                Gradient.SetColors(label, gradient);

                var effect = label.Effects.OfType<GradientRoutingEffect>();

                Assert.True(effect.Any());
            }

            [Fact]
            public void ShouldNotHaveTouch()
            {
                RemoveAllEffects();

                Color touchColor = default;

                Gradient.SetTouchColor(label, touchColor);

                var effect = label.Effects.OfType<GradientRoutingEffect>();

                Assert.True(!effect.Any());
            }

            public void Dispose()
            {
                label = null;
                btn = null;
                gd = null;
            }

            private void RemoveAllEffects()
            {
                var effects = label.Effects.OfType<GradientRoutingEffect>();
                if (!effects.Any())
                {
                    return;
                }

                label.Effects.Remove(effects.First());
            }
        }
        public class GradientColorsTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { new GradientColors(new Color[] { Color.FromHex("#1285A5"), Color.FromHex("#7FC8C4") }), new GradientColors(new Color[] { Color.FromHex("#1285A5"), Color.FromHex("#7FC8C4") }) };
                yield return new object[] { new GradientColors(), new GradientColors() };
                yield return new object[] { new GradientColors(new Color[] { }), new GradientColors(new Color[] { }) };
                yield return new object[] { null, null };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}