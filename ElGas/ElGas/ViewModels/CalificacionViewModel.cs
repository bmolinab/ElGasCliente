using ElGas.Helpers;
using ElGas.Services;
using GalaSoft.MvvmLight.Command;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace ElGas.ViewModels
{
    public class CalificacionViewModel: INotifyPropertyChanged
    {
        #region services
        ApiServices apiService = new ApiServices();

        #endregion
        #region Properties
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region commands
        public ICommand CalificarCommand { get { return new RelayCommand(Calificar); } }
        public async void Calificar()
        {

            

                Settings.Pedidos = false;
            await PopupNavigation.PopAllAsync();
            await App.Navigator.Navigation.PopToRootAsync();
            

        }
        #endregion
    }
}
