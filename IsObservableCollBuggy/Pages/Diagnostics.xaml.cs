
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
            // TODO: Change it to CompiledBinding after figuring out why it does not work
            BindingContext = new Models.Diagnostics();
        }
    }
}