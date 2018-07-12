using ElGas.Helpers;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ElGas.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPage : MasterDetailPage
    {
        public MasterPage()
        {
            InitializeComponent();
           
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.Master = this;
            App.Navigator = Navigator;

            Paginaprincipal();
            
        }
        public async void Paginaprincipal()
        {
            if (Settings.Pedidos) await Navigator.PushAsync(new SeguimientoPage());
            if(Settings.Calificar)
            {
                var page = new Calificacion();
                await   PopupNavigation.PushAsync(page);
            }
        }
    }
}