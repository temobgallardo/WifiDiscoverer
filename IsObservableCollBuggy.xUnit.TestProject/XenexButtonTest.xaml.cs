using IsObservableCollBuggy.Controls;
using IsObservableCollBuggy.Triggers;
using IsObservableCollBuggy.xUnit.Tests.Extensions;
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
    public partial class XenexButtonTest : ContentPage
    {
        public XenexButtonTest()
        {
            InitializeComponent();
        }

        public class Test
        {
            private XenexButtonTest _cp;
            private XenexButton _xnxBtn;

            public Test()
            {
                _cp = new XenexButtonTest();
                _xnxBtn = _cp.XnxButton;
            }

            [Fact]
            public void ShouldHaveDefaultValues()
            {
                Assert.Equal("140", _xnxBtn.Duration);
                Assert.Equal("Clicked", _xnxBtn.TriggeringEvent);
                Assert.Equal(1.05f, _xnxBtn.ClickScale);
            }

            [Theory]
            [ClassData(typeof(Data))]
            public void TestButtonProperties(string duration, string triggerEvent, float scale)
            {
                _xnxBtn.Duration = duration;
                _xnxBtn.TriggeringEvent = triggerEvent;
                _xnxBtn.ClickScale = scale;

                Assert.Equal(duration, _xnxBtn.Duration);
                Assert.Equal(triggerEvent, _xnxBtn.TriggeringEvent);
                Assert.Equal(scale, _xnxBtn.ClickScale);
            }

            [Theory]
            [ClassData(typeof(Data))]
            public void ShouldMakeAnimation(string duration, string triggerEvent, float scale)
            {
                _xnxBtn.Duration = duration;
                _xnxBtn.TriggeringEvent = triggerEvent;
                _xnxBtn.ClickScale = scale;
                //_xnxBtn.PerformClick();
                Assert.Equal(duration, _xnxBtn.Duration);
                Assert.Equal(triggerEvent, _xnxBtn.TriggeringEvent);
                Assert.Equal(scale, _xnxBtn.ClickScale);
            }

            public class Data : IEnumerable<object[]>
            {
                public IEnumerator<object[]> GetEnumerator()
                {
                    yield return new object[] { "140", "Clicked", 1.05f };
                    yield return new object[] { "140", "Clicked", 1.05f };
                    yield return new object[] { string.Empty, string.Empty, 0f };
                    yield return new object[] { null, null, null };
                }

                IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            }
        }
    }
}