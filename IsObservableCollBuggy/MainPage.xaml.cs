using Xamarin.Forms;

namespace IsObservableCollBuggy
{
    public partial class MainPage : ContentPage
    {
        private Pages.Home _home;
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(_home);
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _home = new Pages.Home();
        }
        protected override void OnDisappearing()
        {
            _home = null;
            base.OnDisappearing();
        }
    }
}
