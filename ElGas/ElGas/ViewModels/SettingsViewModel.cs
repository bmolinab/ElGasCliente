using ElGas.Helpers;
using ElGas.Models;
using ElGas.Pages;
using ElGas.Services;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ElGas.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {

        #region Properties
        public string nombreUsuario = "";
        public string NombreUsuario
        {
            get { return nombreUsuario; }
            set { nombreUsuario = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NombreUsuario")); }

        }
        #endregion

        #region Constructor
        public event PropertyChangedEventHandler PropertyChanged;
        public SettingsViewModel()
        {
            NombreUsuario = Settings.NombreCompleto;
        }

        #endregion

        #region Services

        #endregion

        #region Commands

        public ICommand EditProfileCommand { get { return new RelayCommand(EditProfile); } }
        private async void EditProfile()
        {
            await App.Current.MainPage.DisplayAlert("Estamos trabajando", "Muy pronto podrás acceder a esta opción", "Aceptar");
        }

        public ICommand EditNotificationsCommand { get { return new RelayCommand(EditNotifications); } }
        private async void EditNotifications()
        {
            await App.Current.MainPage.DisplayAlert("Estamos trabajando", "Muy pronto podrás acceder a esta opción", "Aceptar");
        }

        public ICommand LogoutCommand { get { return new RelayCommand(Exit); } }
        private async void Exit()
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


            App.Current.MainPage = new NavigationPage(new LoginPage());
        }        
        #endregion
    }
}
