using NUnit.Framework;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IsObservableCollBuggy.NUnit.Tests
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class XenexButton : ContentPage
    {
        public XenexButton()
        {
            InitializeComponent();
        }

        [TestFixture]
        public class TestClass
        {
            [Test]
            public void TestMethod()
            {
                // TODO: Add your test code here
                var cp = new XenexButton();
                var xnxBtn = cp.Xenex_Button;
                Assert.AreEqual("140", xnxBtn.Duration);
                Assert.AreEqual("Clicked", xnxBtn.TriggeringEvent);
                Assert.AreEqual(1.05f, xnxBtn.ClickScale);
            }
        }
    }
}