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
                if (cilindros!=""&& cilindros != null)
                {
                    Valor = (int.Parse(cilindros) * 3.5) + "$";
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Valor"));
                }
                else
                {
                    Valor =  "0$";
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Valor"));
                }
            }
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
        public ConfirmacionViewModel(TKCustomMapPin posicion)
        {
            centerSearch = (MapSpan.FromCenterAndRadius(posicion.Position, Distance.FromMiles(.3)));
            CenterSearch = centerSearch;
            locations = new ObservableCollection<TKCustomMapPin>();
            Locations.Add(posicion);

        }
        

        public ICommand OkCommand { get { return new RelayCommand(Ok); } }
        private async void Ok()
        {
            ApiServices apiServices = new ApiServices();
          
                var action = await App.Current.MainPage.DisplayAlert("Confirmar", "Confirmar la compra de "+cilindros+" cilindros por el valor de "+ Valor, "Confirmar", "Cancelar");
            if(action)
            {
                Compra compra = new Compra
                {
                    IdCliente=(int?)Settings.idCliente,
                    ValorTotal=(double?)double.Parse(Valor.Replace("$","")),
                    Cantidad= (int?) int.Parse(Cilindros),
                    Estado=0,
                    Latitud=(double?) CenterSearch.Center.Latitude,
                    Longitud=(double?) centerSearch.Center.Longitude,                    
                };

                var response = await ApiServices.InsertarAsync<Compra>(compra, new Uri(Constants.BaseApiAddress), "/api/Compras/PostCompras");

                if (response.IsSuccess)
                {
                    await App.Current.MainPage.DisplayAlert("Gracias por hacer su pedido", "En breve le confirmaremos su entrega", "Aceptar");
                    await Task.Delay(2000);
                    await App.Current.MainPage.DisplayAlert("Notificación", "Su pedido ha sido confirmado, un distribuidor está en camino para realizar la entrega", "Aceptar");
                    Settings.Pedidos = true;

                    await App.Navigator.PushAsync(new SeguimientoPage());
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Tenemos un problema con su pedido", response.Message, "Aceptar");
                }

             
            }


        }



    }
}
