using Xamarin.Forms;

namespace IsObservableCollBuggy
{
    public partial class App : Application
    {
        //private readonly MainPage _mainPage = new MainPage();

        public App()
        {
            InitializeComponent();

            //_mainPage = new MainPage();
            //MainPage = new NavigationPage(_mainPage);
            MainPage = new NavigationPage(new Pages.WifiPageStructure());
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
