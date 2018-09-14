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
using TK.CustomMap;

namespace ElGas.ViewModels
{
    public class ConfirmacionViewModel : INotifyPropertyChanged
    {
        #region Properties
        public string cilindros = "1";
        public string Cilindros
        {
            get { return cilindros; }
            set
            {
                if (this.cilindros != value)
                {

                    this.cilindros = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Cilindros"));
                }
                if (cilindros != "" && cilindros != null)
                {
                    Valor = "$"+(int.Parse(cilindros) * Settings.Precio).ToString("N2");
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Valor"));
                }
                else
                {
                    Valor = "$0";
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Valor"));
                }
            }
        }

        public string direccion = "";
        public string Direccion
        {
            get { return direccion; }
            set { direccion = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Direccion")); }

        }

        public string referencia = "";
        public string Referencia
        {
            get { return referencia; }
            set { referencia = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Referencia")); }

        }

        public string Valor { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public MapSpan centerSearch = null;
        public MapSpan CenterSearch
        {
            get { return centerSearch; }
            set
            {
                if (this.centerSearch != value)
                {

                    this.centerSearch = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CenterSearch"));
                }
            }
        }
        public ObservableCollection<TKCustomMapPin> locations;
        public ObservableCollection<TKCustomMapPin> Locations
        {
            protected set
            {
                locations = Locations;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Locations"));
            }
            get { return locations; }
        }
        #endregion
        #region Constructor
        public ConfirmacionViewModel(TKCustomMapPin posicion)
        {
            try
            {
                centerSearch = (MapSpan.FromCenterAndRadius(posicion.Position, Distance.FromMiles(.3)));
                CenterSearch = centerSearch;
                locations = new ObservableCollection<TKCustomMapPin>();
                Locations.Add(posicion);
                Direccion = Settings.Direccion;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }

        }
        #endregion
        #region commands 
        public ICommand PlusCommand { get { return new RelayCommand(Plus); } }
        private async void Plus()
        { 
            Int64 x = int.Parse(Cilindros);
            x = x + 1;
            if (x >= 4)
                x = x - 1;
            Cilindros = x.ToString();
        }
        public ICommand LessCommand { get { return new RelayCommand(Less); } }
        private async void Less()
        {
            Int64 x = int.Parse(Cilindros);
            x = x - 1;
            if (x <= 0)
                x = x + 1;
            Cilindros = x.ToString();
        }
        public ICommand OkCommand { get { return new RelayCommand(Ok); } }
        private async void Ok()
        {
            try
            {
                ApiServices apiServices = new ApiServices();
                var action = await App.Current.MainPage.DisplayAlert("Confirmar", "Por favor confirma el pedido de " + cilindros + " cilindros para:\n " + Direccion + " Ref: " + Referencia, "Confirmar", "Cancelar");
                if (action)
                {
                    Compra compra = new Compra
                    {
                        IdCliente = (int?)Settings.idCliente,
                        ValorTotal = (double?)double.Parse(Valor.Replace("$", "")),
                        Cantidad = (int?)int.Parse(Cilindros),
                        Estado = 0,
                        Latitud = (double?)CenterSearch.Center.Latitude,
                        Longitud = (double?)centerSearch.Center.Longitude,
                        Direccion= Direccion,
                        Referencia=Referencia
                    };
                    var response = await ApiServices.InsertarAsync<Compra>(compra, new Uri(Constants.BaseApiAddress), "/api/Compras/PostCompras");
                    if (response.IsSuccess)
                    {
                        await App.Current.MainPage.DisplayAlert("Gracias por tu pedido", "En breve confirmamos la entrega.", "Aceptar");
                        Settings.TanquesGas = int.Parse(Cilindros);
                        await App.Navigator.Navigation.PopToRootAsync();
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("No hemos podido realizar el pedido, por favor, intenta más tarde.", response.Message, "Aceptar");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
        }
        #endregion
    }
}
