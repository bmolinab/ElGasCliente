using ElGas.Helpers;
using ElGas.Models;
using ElGas.Pages;
using ElGas.Services;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                    IsBusy = true;
                    var accesstoken = await _apiServices.LoginAsync(Username, Password);

                    if (accesstoken != null)
                    {
                        Settings.AccessToken = accesstoken;
                        var c = new Cliente { Correo = Username, DeviceID = Settings.DeviceID };
                        var response = await ApiServices.InsertarAsync<Cliente>(c, new System.Uri(Constants.BaseApiAddress), "/api/Clientes/GetClientData");
                        var cliente = JsonConvert.DeserializeObject<Cliente>(response.Result.ToString());
                        Settings.idCliente = cliente.IdCliente;
                        IsBusy = false;

                        App.Current.MainPage = new NavigationPage(new MapaPage());
                    }

                });
            }
        }

        public async  void permisos()
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

                throw;
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

        public async Task LoginAsyncFB()         {
            FacebookResponse<bool> response = await CrossFacebookClient.Current.LoginAsync(permisions);             switch (response.Status)
            {
                case FacebookActionStatus.Completed:
                    await App.Current.MainPage.DisplayAlert("Loggeado", response.Message, "Ok"); await LoadData();//App.Current.MainPage.Navigation.PushAsync(new MyProfilePage());
                    break;
                case FacebookActionStatus.Canceled:
                    break;
                case FacebookActionStatus.Unauthorized:
                    await App.Current.MainPage.DisplayAlert("Unauthorized", response.Message, "Ok");
                    break;
                case FacebookActionStatus.Error:
                    await App.Current.MainPage.DisplayAlert("Error", response.Message, "Ok");
                    break;
            }

        }

        public async Task LoadData()         {

            var jsonData = await CrossFacebookClient.Current.RequestUserDataAsync             (
                  new string[] { "id", "name", "email"}, new string[] { }             );

            var data = JObject.Parse(jsonData.Data);
            Debug.WriteLine(data.Count);
        }



        public ICommand RegisterCommand { get { return new RelayCommand(Register); } }

        private async void Register()
        {
            App.Current.MainPage = new NavigationPage(new RegisterPage());
        }

        #endregion


        #region Constructor
        public LoginViewModel()
        {
            Username = Settings.Username;
            Password = Settings.Password;
          //  permisos();
        }
        #endregion
    }
}
