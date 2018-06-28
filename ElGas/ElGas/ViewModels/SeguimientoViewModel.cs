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
