using ElGas.Helpers;
using ElGas.Models;
using ElGas.Pages;
using ElGas.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
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
        public string CalleUno { get; set; }
        public string Numero { get; set; }

        public string CalleDos { get; set; }
        public string Sector { get; set; }

        public string message = "";
        public string Message
        {
            set
            {
                if (message != value)
                {
                    message = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Message"));
                }
            }
            get
            {
                return message;
            }
        }
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

        #region Cosntructor
        public RegisterViewModel(Cliente cliente)
        {
            isError = false;
            IsError = false;

            Cliente = cliente;
            Username = cliente.Correo;
            Password = cliente.Identificacion;
            ConfirmPassword = cliente.Identificacion;

            cliente.Identificacion = "";

           

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
                    if (Password == ConfirmPassword)
                    {
                        Cliente.Direccion = String.Format("{0}, {1}, {2}, {3}",CalleUno,Numero,CalleDos,Sector);
                      
                            var isRegistered = await _apiServices.RegisterUserAsync
                            (Username, Password, ConfirmPassword, Cliente);
                            Settings.Username = Username;
                            Settings.Password = Password;

                            IsBusy = false;

                            if (isRegistered)
                            {
                                Message = "Se registró con éxito";
                                await App.Current.MainPage.DisplayAlert("El Gas", Message, "Aceptar");
                                App.Current.MainPage = new NavigationPage(new LoginPage());

                            }
                            else
                            {
                                Message = "No  pudimos registrar su cuenta, reintentelo";
                                await App.Current.MainPage.DisplayAlert("El Gas", Message, "Aceptar");
                            }
                        
                        //else
                        //{
                        //    IsBusy = false;
                        //    Message = "La contraseña debe tener 8-16 caracteres e incluir al menos una minúscula, una mayúscula, un número y un caracter especial";
                        //    IsError = true;
                        //}

                    }
                    else
                    {
                        IsBusy = false;
                        Message = "Las contraseñas no coincide";
                        await App.Current.MainPage.DisplayAlert("El Gas", Message, "Aceptar");

                        // IsError = true;
                    }


                });
            }
        }

        public ICommand NextCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if(Password!=null&&Password!="")
                    {
                        if (ConfirmPassword == Password )
                        {
                            if (Password.Length>3)
                            {
                                Cliente.Correo = Username;
                                Cliente.Identificacion = Password;
                                App.Current.MainPage = new NavigationPage(new RegisterPage2(Cliente));
                            }
                            else
                            {
                                await App.Current.MainPage.DisplayAlert("Error", "Las contraseña debe tener al menos 4 caracteres", "Aceptar");

                            }



                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Error", "Las contraseñas no coinciden", "Aceptar");
                        }

                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Todos los campos son obligatorios", "Aceptar");

                    }



                });
            }
        }


        #endregion
    }
}
