using Xamarin.Forms;

namespace IsObservableCollBuggy
{
    public partial class MainPage : ContentPage
    {
        private readonly Pages.Home _home = new Pages.Home();
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
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}
