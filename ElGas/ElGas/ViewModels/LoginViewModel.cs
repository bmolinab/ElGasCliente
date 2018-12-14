using ElGas.Helpers;
using ElGas.Models;
using ElGas.Pages;
using ElGas.Services;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Connectivity;
using Plugin.FacebookClient;
using Plugin.FacebookClient.Abstractions;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ElGas.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Agregar referencias
        /// y agegar ciudad
        /// </summary>
        #region Services
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly ApiServices _apiServices = new ApiServices();

        string[] permisions = new string[] { "email", "public_profile" };
        #endregion
        #region Propieties
        private bool _isRemember = false;
        public bool isRemember
        {
            set
            {
                if (_isRemember != value)
                {
                    _isRemember = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("isRemember"));
                }
            }
            get
            {
                return _isRemember;
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

        public string Username { get; set; }
        public string Password { get; set; }
        #endregion
        #region Commands
        public ICommand LoginCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if (CrossConnectivity.Current.IsConnected)
                    {

                        IsBusy = true;
                        if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                        {
                            IsBusy = false;
                            await App.Current.MainPage.DisplayAlert("Información", "Debe ingresar el usuario y la contraseña", "Aceptar");
                            return;
                        }

                        var accesstoken = await _apiServices.LoginAsync(Username, Password);

                        if (accesstoken != null)
                        {
                            if(accesstoken!="1")
                            {
                                if (isRemember) Settings.AccessToken = accesstoken;
                                int OS = 0;
                                if (Device.RuntimePlatform.Equals("iOS"))
                                {
                                    OS = 1;
                                }


                                var c = new Cliente { Correo = Username, DeviceID = Settings.DeviceID, SistemaOperativo = OS };
                                var response = await ApiServices.InsertarAsync<Cliente>(c, new System.Uri(Constants.BaseApiAddress), "/api/Clientes/GetClientData");

                                var cliente = JsonConvert.DeserializeObject<Cliente>(response.Result.ToString());
                                Settings.idCliente = cliente.IdCliente;
                                Settings.NombreCompleto = cliente.Nombres + " " + cliente.Apellidos;
                                IsBusy = false;
                                Application.Current.MainPage = new NavigationPage(new MasterTabPage());

                            }
                            IsBusy = false;
                            return;

                        }
                        IsBusy = false;
                        await App.Current.MainPage.DisplayAlert("Información", "Usuario o contraseña incorrecta, Vuelve a intertarlo o presiona Olvidé mi contraseña para recuperarla", "Aceptar");
                        return;
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert(Mensaje.Titulo.Error, Mensaje.Contenido.SinInternet, Mensaje.TextoBoton.Aceptar);
                    }

                });

            }
        }

        public async  void permisos()
        {
            if (CrossConnectivity.Current.IsConnected)
            {

                try
                {
                    var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                    if (status != PermissionStatus.Granted)
                    {
                        var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);

                        Debug.WriteLine(results.Count.ToString());

                        if (status != PermissionStatus.Unknown)
                        {
                            Debug.WriteLine("Location Denied");
                        }
                    }



                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }


        public ICommand LoginFB
        {
            get
            {
                return new Command(async () =>
                {
                    await LoginAsyncFB();                   

                });
            }
        }

        public async Task LoginAsyncFB()
        {
            try
            {
                FacebookResponse<bool> response = await CrossFacebookClient.Current.LoginAsync(permisions);
                switch (response.Status)
                {
                    case FacebookActionStatus.Completed:
                        await LoadData();
                        //App.Current.MainPage.DisplayAlert("Loggeado", response.Message, "Ok"); 
                        break;
                    case FacebookActionStatus.Canceled:
                        break;
                    case FacebookActionStatus.Unauthorized:
                        await Application.Current.MainPage.DisplayAlert("Unauthorized", response.Message, "Ok");
                        break;
                    case FacebookActionStatus.Error:
                        await Application.Current.MainPage.DisplayAlert("Error", response.Message, "Ok");
                        break;
                }
            }
            catch (Exception ex)
            {

                Debug.Write(ex.Message);
            }

        }

        public async Task LoadData()
        {

            try
            {
                var jsonData = await CrossFacebookClient.Current.RequestUserDataAsync
                   (
                         new string[] { "id", "name", "email","first_name", "last_name" }, new string[] { }
                   );

                var data = JObject.Parse(jsonData.Data);

                var Profile = new FacebookProfile()
                {
                    Id = data["id"].ToString(),
                    FullName = data["name"].ToString(),
                    Email = data["email"].ToString(),
                    LastName  = data["last_name"].ToString(),
                    FirstName = data["first_name"].ToString()

                };


                var accesstoken = await _apiServices.LoginAsync(Profile.Email, Profile.Id);

                if (accesstoken != null)
                {
                    Settings.AccessToken = accesstoken;
                    var c = new Cliente { Correo = Profile.Email, DeviceID = Settings.DeviceID };
                    var response = await ApiServices.InsertarAsync<Cliente>(c, new System.Uri(Constants.BaseApiAddress), "/api/Clientes/GetClientData");
                    var cliente = JsonConvert.DeserializeObject<Cliente>(response.Result.ToString());
                    Settings.idCliente = cliente.IdCliente;
                    Settings.Username = Profile.Email;
                    Settings.Password = Profile.Id;
                    IsBusy = false;
                    Application.Current.MainPage = new NavigationPage(new MasterTabPage());

                }
                else
                {
                    await Application.Current.MainPage.Navigation.PushAsync(new AfterFBPage(Profile));
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
            //   Debug.WriteLine(data.Count);
        }

        public ICommand RegisterCommand { get { return new RelayCommand(Register); } }
        private async void Register()
        {
            if (CrossConnectivity.Current.IsConnected)
            {

                try
                {
                    await Application.Current.MainPage.Navigation.PushAsync(new RegisterPage());

                }
                catch (Exception ex)
                {
                    Debug.Write(ex.Message);
                }
            }
          
        }

       
        ICommand tapCommand;
       
        public ICommand TapCommand
        {
            get { return tapCommand; }
        }
        private async void OnTapped(object s)
        {
            if (CrossConnectivity.Current.IsConnected)
            {

                try
            {
                await Application.Current.MainPage.Navigation.PushAsync(new RegisterPage());

            }
            catch (Exception ex)
            {

                Debug.Write(ex.Message);
            }
        }

              else
            {
                await App.Current.MainPage.DisplayAlert(Mensaje.Titulo.Error, Mensaje.Contenido.SinInternet, Mensaje.TextoBoton.Aceptar);
            }
        }


        public ICommand RecoveryCommand { get { return new RelayCommand(Recovery); } }
        private async void Recovery()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    await Application.Current.MainPage.Navigation.PushAsync(new RecoveryPassPage());

                }
                catch (Exception ex)
                {

                    Debug.Write(ex.Message);
                }

            }
            else
            {
                await App.Current.MainPage.DisplayAlert(Mensaje.Titulo.Error, Mensaje.Contenido.SinInternet, Mensaje.TextoBoton.Aceptar);
            }
        }

        #endregion
        #region Constructor
        public LoginViewModel()
        {
            Username = Settings.Username;
            Password = Settings.Password;
            tapCommand = new Command(OnTapped);
           
           // permisos();
        }
        #endregion
    }
}
