using Xamarin.Forms;

namespace IsObservableCollBuggy
{
    public partial class App : Application
    {
        private Pages.Diagnostics _diagnostics;
        public App()
        {
            InitializeComponent();

            _diagnostics = new Pages.Diagnostics();
            MainPage = new NavigationPage(_diagnostics);
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            _diagnostics = null;
        }

        protected override void OnResume()
        {
        }
    }
}
