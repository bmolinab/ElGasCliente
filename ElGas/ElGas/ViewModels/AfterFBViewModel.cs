using ElGas.Helpers;
using ElGas.Models;
using ElGas.Pages;
using ElGas.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace ElGas.ViewModels
{
    public class AfterFBViewModel : INotifyPropertyChanged
    {
        #region services
        ApiServices apiService = new ApiServices();
        #endregion
        #region Properties
        public event PropertyChangedEventHandler PropertyChanged;
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string CalleUno { get; set; }
        public string Numero { get; set; }

        public string CalleDos { get; set; }
        public string Sector { get; set; }

        public Cliente Cliente { get; set; } 
        

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

        public FacebookProfile fbProfile = new FacebookProfile();
        public FacebookProfile FBProfile
        {
            get { return fbProfile; }
            set
            {
                if (this.fbProfile != value)
                {

                    this.fbProfile = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FBProfile"));
                }

            }
        }
        #endregion
        #region Constructor
        public AfterFBViewModel(FacebookProfile facebookProfile)
        {
            try
            {
                isError = false;
                IsError = false;
                Cliente = new Cliente();
                FBProfile = facebookProfile;
                Username = fbProfile.Email;
                Password = fbProfile.Id;
                ConfirmPassword = fbProfile.Id;
                Cliente.Correo = fbProfile.Email;
                Char delimiter = ' ';
                String[] words = facebookProfile.FullName.Split(delimiter);
                if (words.Length > 1)
                {
                    Cliente.Nombres = words[0];
                    Cliente.Apellidos = words[1];
                }
            }
            catch (Exception ex)
            {

                Debug.Write(ex.Message);
            }
        }
        #endregion
        #region Commands
        public ICommand RegisterCommand
        {
            get
            {
                return new Command(async () =>
                {
                    try
                    {
                        IsBusy = true;
                        if (Password == ConfirmPassword)
                        {
                            Cliente.Direccion = String.Format("{0}, {1}, {2}, {3}", CalleUno, Numero, CalleDos, Sector);
                            Cliente.Habilitado = true;

                            var isRegistered = await apiService.RegisterUserAsync
                            (Username, Password, ConfirmPassword, Cliente);
                            Settings.Username = Username;
                            Settings.Password = Password;

                            IsBusy = false;

                            if (isRegistered)
                            {
                                var accesstoken = await apiService.LoginAsync(Username, Password);
                                if (accesstoken != null)
                                {
                                    Settings.AccessToken = accesstoken;
                                    var c = new Cliente { Correo = Username, DeviceID = Settings.DeviceID };
                                    var response = await ApiServices.InsertarAsync<Cliente>(c, new System.Uri(Constants.BaseApiAddress), "/api/Clientes/GetClientData");
                                    var cliente = JsonConvert.DeserializeObject<Cliente>(response.Result.ToString());
                                    Settings.idCliente = cliente.IdCliente;
                                    IsBusy = false;
                                    Application.Current.MainPage = new NavigationPage(new MasterTabPage());
                                }
                            }
                            else
                            {
                                Message = "Tenemos un error su cuenta, reintentelo";
                                await App.Current.MainPage.DisplayAlert("El Gas", Message, "Aceptar");
                            }
                        }
                        else
                        {
                            IsBusy = false;
                            Message = "Las contraseñas no coincide";
                            await App.Current.MainPage.DisplayAlert("El Gas", Message, "Aceptar");
                        }
                    }
                    catch (Exception ex)
                    {

                        Debug.Write(ex.Message);
                    }
                });
            }
        }

        #endregion

    }
}
