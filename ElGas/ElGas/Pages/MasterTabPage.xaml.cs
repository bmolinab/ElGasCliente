using ElGas.Helpers;
using ElGas.Services;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

            //MessagingCenter.Subscribe<string>("update", "Hi", (sender) =>
            //{
            //    Paginaprincipal();
            //});

        }


        


        public async void Paginaprincipal()
        {
            var i = this.Children.IndexOf(this.CurrentPage);
            if (i == 0)
            {

                if (Settings.Pedidos)
                {
                    Settings.CantidadCalificacionPage = 0;
                    if (Settings.CantidadSeguimientoPage == 0)
                    {
                        Settings.CantidadSeguimientoPage = 1;
                        await App.Navigator.PushAsync(new SeguimientoPage());
                    }

                }
                else
                {
                    Settings.CantidadSeguimientoPage = 0;
                }

                if (Settings.Pedidos!=true)
                {

               
                if (Settings.Calificar)
                {
                        if (Settings.CantidadCalificacionPage == 0)
                        {
                            Settings.CantidadCalificacionPage = 1;
                            await App.Navigator.PushAsync(new Calificacion());
                        }
                        else
                        {
                            Settings.CantidadSeguimientoPage = 0;
                        }

                    }
                else
                {

                    var apiService = new ApiServices();

                    var result = await apiService.CompraSinCalificarPorCliente();

                    if (result != null)
                    {
                            if (Settings.CantidadCalificacionPage == 0)
                            {
                                Settings.IdCompra = result.IdCompra;
                                Settings.IdDistribuidor = result.IdDistribuidor.Value;
                                await App.Navigator.PushAsync(new Calificacion());
                            }
                            else
                            {
                                Settings.CantidadSeguimientoPage = 0;
                            }
                        }
                }
                }

            }

        }

        protected override bool OnBackButtonPressed()
        {
            if (paginaactual != "Inicio")
            {
                Tbpage.CurrentPage = NavigatorM;
               
                return true;
            }

            if (Settings.CantidadSeguimientoPage == 1)
            {
                Settings.CantidadSeguimientoPage = 0;
            }

            if (Settings.CantidadCalificacionPage == 1)
            {
                Settings.CantidadCalificacionPage = 0;
            }


            return base.OnBackButtonPressed();
        }
    }
}
