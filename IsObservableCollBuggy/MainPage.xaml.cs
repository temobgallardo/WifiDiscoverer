using Xamarin.Forms;

namespace IsObservableCollBuggy
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new Pages.Diagnostics());
            Navigation.RemovePage(this);
        }
    }
}
