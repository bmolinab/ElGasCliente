using ElGas.Pages;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace ElGas.ViewModels
{
    public class MenuViewModel
    {
        #region Properties

        #endregion

        #region Constructor
        public MenuViewModel()
        {

        }

        #endregion

        #region Services

        #endregion

        #region Commands
        public ICommand LogoutCommand { get { return new RelayCommand(Salir); } }
        private async void Salir()
        {
            Helpers.Settings.idCliente = 0;
            Helpers.Settings.IdCompra = 0;
            Helpers.Settings.IdDistribuidor = 0;
            Helpers.Settings.TanquesGas = 0;
            Helpers.Settings.Password = "";
            Helpers.Settings.Username = "";
            Helpers.Settings.TanquesGas = 0;
          
            Helpers.Settings.AccessToken = "";
            Helpers.Settings.AccessTokenExpirationDate = new DateTime();

            Helpers.Settings.Calificar = false;
            Helpers.Settings.Pedidos = false;


            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
        #endregion
    }
}
