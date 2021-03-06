﻿using ElGas.Helpers;
using ElGas.Models;
using ElGas.Services;
using GalaSoft.MvvmLight.Command;
using Rg.Plugins.Popup.Services;
using Syncfusion.SfRating.XForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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


        private bool isVisible = true;
        public bool IsVisible
        {
            set
            {
                if (isVisible != value)
                {
                    isVisible = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsVisible"));
                }
            }
            get
            {
                return isVisible;
            }
        }

        private bool isBusy = false;
        public bool IsBusy
        {
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsBusy"));
                }
            }
            get
            {
                return isBusy;
            }
        }

        #endregion
        #region commands
        public ICommand CalificarCommand { get { return new RelayCommand(Calificar); } }
        public async void Calificar()
        {
            try
            {
                IsBusy = true;
                IsVisible = false;
                var compra = new Compra
                {
                    IdCompra = Settings.IdCompra,
                    IdCliente = Settings.idCliente,
                    IdDistribuidor = Settings.IdDistribuidor,
                    Calificacion = Valor
                };

                var response = await ApiServices.InsertarAsync<Compra>(compra, new Uri(Constants.BaseApiAddress), "/api/Compras/Calificar");


                Settings.Pedidos = false;
                Settings.Calificar = false;
                IsBusy = true;
                IsVisible = false;
                await PopupNavigation.PopAllAsync();
                await App.Navigator.Navigation.PopToRootAsync();

            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }

        }
        #endregion
    }
}
