
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IsObservableCollBuggy.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Diagnostics : ContentPage
    {
        private readonly Models.Diagnostics _vm = new Models.Diagnostics();

        public Diagnostics()
        {
            InitializeComponent();
            BindingContext = _vm;
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _vm.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            _vm.OnDisappearing();
            base.OnDisappearing();
        }
    }
}