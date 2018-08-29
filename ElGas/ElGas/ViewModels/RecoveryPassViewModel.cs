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
    public class RecoveryPassViewModel: INotifyPropertyChanged
    {
        #region Services
        private readonly ApiServices _apiServices = new ApiServices();
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int Code { get; set; }

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

        private bool isError = false;
        public bool IsError
        {
            set
            {
                if (isError != value)
                {
                    isError = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsError"));
                }
            }
            get
            {
                return isError;
            }
        }
        #endregion
        #region Constructor
        public RecoveryPassViewModel(string username)
        {
            Username = username;

        }
        #endregion


        #region Commands
        public ICommand SendCodeCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsBusy = true;
                    if (Password == ConfirmPassword)
                    {
                        
                        var isRegistered = await _apiServices.GenerateCode
                        (Username);
                       
                        IsBusy = false;

                        if (isRegistered)
                        {
                            await App.Current.MainPage.DisplayAlert("El Gas", "El código fue enviado a su Email registrado", "Aceptar");
                            App.Current.MainPage = new NavigationPage(new RecoveryPassPage2(Username));


                        }
                        else
                        {
                             var Message = "No  pudimos enviar el código, verifique su Email y reintentelo";
                            await App.Current.MainPage.DisplayAlert("El Gas", Message, "Aceptar");
                        }

                 

                    }
                    else
                    {
                        IsBusy = false;
                        var Message = "Las contraseñas no coincide";
                        await App.Current.MainPage.DisplayAlert("El Gas", Message, "Aceptar");

                        // IsError = true;
                    }


                });
            }
        }

      

        public ICommand ChangePasswordCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsBusy = true;
                    if (Password == ConfirmPassword)
                    {
                        var isRegistered = await _apiServices.RecoveryPass
                        (Username, Password, ConfirmPassword, Code);
                        IsBusy = false;
                        if (isRegistered)
                        {
                            await App.Current.MainPage.DisplayAlert("El Gas", "Su contraseña se actualizo exitosamente", "Aceptar");
                            App.Current.MainPage = new NavigationPage(new LoginPage());
                        }
                        else
                        {
                            var Message = "No  pudimos actualizar su contraseña, verifique el codigo, su correo y reintentelo";
                            await App.Current.MainPage.DisplayAlert("El Gas", Message, "Aceptar");
                        }
                    }
                });
            }
        }


        #endregion

    }
}
