using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IsObservableCollBuggy.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Home : ContentPage
    {
        private/* readonly*/ WifiPageStructure _wifiPageStructure;
        private/* readonly*/ Diagnostics _diagnostics;
        public Home()
        {
            InitializeComponent();
            //_wifiPageStructure = new WifiPageStructure();
            //_diagnostics = new Diagnostics();
        }

        private async void ZappButton_Clicked(object s, EventArgs e)
        {
            //ZappSpinner.IsRunning = true;

            //await Task.Delay(2000);

            //ZappSpinner.IsRunning = false;

            await Navigation.PushAsync(_wifiPageStructure);
        }
        private async void NavigateButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(_diagnostics);
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            _wifiPageStructure = new WifiPageStructure();
            _diagnostics = new Diagnostics();
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _wifiPageStructure = null;
            _diagnostics = null;
        }
    }
}