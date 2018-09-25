using ElGas.Helpers;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace ElGas.Pages
{
    public partial class MasterTabPage : TabbedPage
    {
        int vista = 0;
        string paginaactual = "Inicio";
        public MasterTabPage()
        {
            InitializeComponent();
            App.Navigator = NavigatorM;
            Tbpage.CurrentPageChanged += Tbpage_CurrentPageChanged;

        }

        private void Tbpage_CurrentPageChanged(object sender, EventArgs e)
        {
            TabbedPage np = (TabbedPage)sender;

            paginaactual = np.CurrentPage.Title;
            switch (np.CurrentPage.Title)
            {
                case "Pedidos":
                    App.Navigator = NavigatorH;
                    break;
                case "Configuración":
                    App.Navigator = NavigatorS;
                    break;
                case "Inicio":
                    App.Navigator = NavigatorM;
                    break;
                default:
                    App.Navigator = NavigatorM;
                    break;
            }

        }

        protected override void OnAppearing()
        {
           

            Paginaprincipal();

            MessagingCenter.Subscribe<string>("update", "Hi", (sender) => {
                Paginaprincipal();
            });

        }


        


        public async void Paginaprincipal()
        {
            if (Settings.Pedidos)
            {
                await App.Navigator.PushAsync(new SeguimientoPage());

            }
            if (Settings.Calificar)
            {
                var page = new Calificacion();
                await PopupNavigation.PushAsync(page);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            if (paginaactual != "Inicio")
            {
                Tbpage.CurrentPage = NavigatorM;

                return true;
            }
            return base.OnBackButtonPressed();
        }
    }
}
