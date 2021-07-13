using Xamarin.Forms;

namespace IsObservableCollBuggy
{
    public partial class App : Application
    {
        private Pages.Diagnostics _diagnostics;
        private MainPage _mainPage;

        public App()
        {
            InitializeComponent();

            _diagnostics = new Pages.Diagnostics();
            _mainPage = new MainPage();
            MainPage = new NavigationPage(_mainPage);
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
