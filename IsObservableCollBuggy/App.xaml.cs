using Xamarin.Forms;

namespace IsObservableCollBuggy
{
    public partial class App : Application
    {
        private MainPage _mainPage;

        public App()
        {
            InitializeComponent();

            _mainPage = new MainPage();
            MainPage = new NavigationPage(_mainPage);
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
