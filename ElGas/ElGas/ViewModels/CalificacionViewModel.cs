using ElGas.Helpers;
using ElGas.Models;
using ElGas.Services;
using GalaSoft.MvvmLight.Command;
using Rg.Plugins.Popup.Services;
using Syncfusion.SfRating.XForms;
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
       
        public int valor = 0 ;
        public int Valor
        {
            get { return valor; }
            set
            {
                if (this.valor != value)
                {

                    this.valor = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Valor"));
                }
               
            }
        }
        #endregion
        #region commands
        public ICommand CalificarCommand { get { return new RelayCommand(Calificar); } }
        public async void Calificar()
        {
            var compra = new Compra {
                IdCompra = Settings.IdCompra,
                IdCliente= Settings.idCliente,
                IdDistribuidor=Settings.IdDistribuidor,
                Calificacion= Valor
            };

            var response = await ApiServices.InsertarAsync<Compra>(compra, new Uri(Constants.BaseApiAddress), "/api/Compras/Calificar");


            Settings.Pedidos = false;
            Settings.Calificar = false;
            await PopupNavigation.PopAllAsync();
            await App.Navigator.Navigation.PopToRootAsync();
            

        }
        #endregion

    }
}
