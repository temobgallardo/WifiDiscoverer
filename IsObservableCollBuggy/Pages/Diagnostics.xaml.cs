
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IsObservableCollBuggy.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Diagnostics : ContentPage
    {
        public Diagnostics()
        {
            InitializeComponent();
            BindingContext = new Models.Diagnostics();
        }
    }
}