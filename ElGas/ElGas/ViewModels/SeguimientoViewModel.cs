using ElGas.Pages;
using ElGas.Services;
using GalaSoft.MvvmLight.Command;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using System.Threading.Tasks;
using Plugin.Messaging;
using ElGas.Helpers;

namespace ElGas.ViewModels
{
    public class SeguimientoViewModel: INotifyPropertyChanged
    {
        #region services
        ApiServices apiService = new ApiServices();

        #endregion
        #region Properties
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructor
        public SeguimientoViewModel()
        {
            Ok();
        }
        #endregion

        #region Commands
        public ICommand ContactCommand { get { return new RelayCommand(Contact); } }
        public async void Contact()
        {
            var PhoneCallTask = CrossMessaging.Current.PhoneDialer;
            if (PhoneCallTask.CanMakePhoneCall)
            {
                PhoneCallTask.MakePhoneCall("0000000000","Nombre Vendedor");
            }
        }

        public ICommand CancelCommand { get { return new RelayCommand(Cancel); } }
        public async void Cancel()
        {

            var action = await App.Current.MainPage.DisplayAlert("Pedido a Cancelar", "N Tanques", "Confirmar", "Regresar");
            if (action)
            {
                await App.Current.MainPage.DisplayAlert("Pedido a Cancelar", "Su pedido de N tanques ha sido cancelado", "Aceptar");

                Settings.Pedidos = false;

                await App.Navigator.Navigation.PopToRootAsync();
            }

        }

        public async void Ok()
        {
            await Task.Delay(2000);
            await App.Current.MainPage.DisplayAlert("Notificación", "su pedido está a pocos metros de llegar", "Aceptar");
            await Task.Delay(2000);
            var page = new Calificacion();

            await   PopupNavigation.PushAsync(page);


        }
        #endregion
    }
}
