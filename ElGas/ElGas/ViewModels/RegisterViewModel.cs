using ElGas.Helpers;
using ElGas.Models;
using ElGas.Pages;
using ElGas.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace ElGas.ViewModels
{
    public class RegisterViewModel: INotifyPropertyChanged
    {
        private readonly ApiServices _apiServices = new ApiServices();
        public event PropertyChangedEventHandler PropertyChanged;

        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Message { get; set; }

        public Cliente Cliente{get;set;}

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

        #region Cosntructor
        public RegisterViewModel()
        {
            Cliente = new Cliente();
        }
        #endregion

        #region Commands
        public ICommand RegisterCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsBusy = true;
                    var isRegistered = await _apiServices.RegisterUserAsync
                        (Username, Password, ConfirmPassword, Cliente);

                    Settings.Username = Username;
                    Settings.Password = Password;

                    IsBusy = false;

                    if (isRegistered)
                    {
                        Message = "Se registro con exito :)";
                        await App.Current.MainPage.DisplayAlert("El Gas", Message, "Aceptar");
                        App.Current.MainPage = new NavigationPage(new LoginPage());

                    }
                    else
                    {
                        Message = "No  pudimos registrar su cuenta, reintentelo";
                        await App.Current.MainPage.DisplayAlert("El Gas", Message, "Aceptar");


                    }


                });
            }
        }

        #endregion
    }
}
