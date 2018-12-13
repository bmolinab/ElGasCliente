using ElGas.Helpers;
using ElGas.Models;
using ElGas.Pages;
using ElGas.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace ElGas.ViewModels
{
    public class AfterFBViewModel : INotifyPropertyChanged
    {
        #region Services
        private readonly ApiServices _apiServices = new ApiServices();
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

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

        private Ciudad _ciudadSeleccionada { get; set; }
        public Ciudad CiudadSeleccionada
        {
            get
            {
                return _ciudadSeleccionada;
            }

            set
            {


                if (_ciudadSeleccionada != value)
                {
                    _ciudadSeleccionada = value;
                    LoadSectors(_ciudadSeleccionada.IdCiudad);

                }
            }
        }

        public Sector SectorSeleccionado
        {
            get; set;
        }

        List<Ciudad> ciudades = new List<Ciudad>();
        public List<Ciudad> Ciudades
        {
            set
            {
                if (ciudades != value)
                {
                    ciudades = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Ciudades"));
                }
            }
            get
            {
                return ciudades;
            }
        }

        public List<Sector> Sectores
        {
            get;
            set;
        }

        private List<Sector> _sectoresPorCiudad;
        public List<Sector> SectoresPorCiudad
        {
            get
            {
                return _sectoresPorCiudad;
            }
            set

            {
                if (_sectoresPorCiudad != value)
                {
                    _sectoresPorCiudad = value;
                    OnPropertyChanged();
                }
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
                LoadCities();
                isError = false;
                IsError = false;
                Cliente = new Cliente();
                FBProfile = facebookProfile;
                Username = fbProfile.Email;
                Password = fbProfile.Id;
                ConfirmPassword = fbProfile.Id;
                Cliente.Correo = fbProfile.Email;
                Cliente.Identificacion = "";


                Cliente.Nombres = fbProfile.FirstName;
                Cliente.Apellidos = fbProfile.LastName;

            }
            catch (Exception ex)
            {

                Debug.Write(ex.Message);
            }
        }
        #endregion
        #region Commands

        #region Methods    
        public async void LoadCities()
        {
            try
            {
                Ciudades = await _apiServices.GetCiudades();
            }
            catch (Exception ex)
            {

                Debug.Write(ex.Message);
            }
        }

        public async void LoadSectors(int idCities)
        {
            try
            {
                SectoresPorCiudad = await _apiServices.GetSectors(idCities);

            }
            catch (Exception ex)
            {

                Debug.Write(ex.Message);
            }
        }
        #endregion

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
                            Cliente.IdSector = SectorSeleccionado.IdSector;

                            var isRegistered = await _apiServices.RegisterUserAsync
                            (Username, Password, ConfirmPassword, Cliente);
                            Settings.Username = Username;
                            Settings.Password = Password;

                            IsBusy = false;

                            if (isRegistered)
                            {
                                var accesstoken = await _apiServices.LoginAsync(Username, Password);
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
                                Message = "Ups! algo ha salido mal, por favor vuelve a intentar.";
                                await App.Current.MainPage.DisplayAlert("El Gas", Message, "Aceptar");
                            }
                        }
                        else
                        {
                            IsBusy = false;
                            Message = "No hemos podido validar tu cuenta de Facebook, por favor vuelve a intentar";
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

        #region PropertyChanged

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        #endregion

    }
}

