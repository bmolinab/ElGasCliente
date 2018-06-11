using ElGas.Helpers;
using ElGas.Models;
using ElGas.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace ElGas.ViewModels
{
    public class RegisterViewModel
    {
        private readonly ApiServices _apiServices = new ApiServices();

        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Message { get; set; }

        public Cliente Cliente{get;set;}

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
                    var isRegistered = await _apiServices.RegisterUserAsync
                        (Username, Password, ConfirmPassword, Cliente);

                    Settings.Username = Username;
                    Settings.Password = Password;

                    if (isRegistered)
                    {
                        Message = "Se registro con exito :)";
                    }
                    else
                    {
                        Message = "No  pudimos registrar su cuenta, reintentelo";
                    }
                });
            }
        }

        #endregion
    }
}
