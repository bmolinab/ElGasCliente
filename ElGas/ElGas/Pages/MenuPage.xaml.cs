using ElGas.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ElGas.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        MenuViewModel viewModel;
        public MenuPage()
        {
            viewModel = new MenuViewModel();
            BindingContext = viewModel;
            InitializeComponent();
        }
    }
}