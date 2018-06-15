using ElGas.Helpers;
using ElGas.Pages;
using ElGas.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ElGas.Services
{
    public class NavigationService
    {
        public void NavigateBack() => App.Navigator.PopToRootAsync();

        public async Task Navigate(string pageName)
        {
            App.Master.IsPresented = false;
            switch (pageName)
            {
                //case "VerificarAutoPage":
                //    await App.Navigator.PushAsync(new VerificarAutoPage(), true);
                //    break;

                //case "ConsultarMultas":
                //    await App.Navigator.PushAsync(new ConsultarAutoPage());
                //    break;
                //case "PonerMulta":
                //    await App.Navigator.PushAsync(new PonerMultaPage(), true);
                //    break;

                //case "PasswordPage":
                //    await App.Navigator.PushAsync(new PasswordPage());
                //    break;

            
                case "MainPage":
                    await App.Navigator.PopToRootAsync();
                    break;

                default: break;
            }
        }

        internal void SetMainPage()
        {

   

           // App.Current.MainPage = new MasterPage();

            if (!string.IsNullOrEmpty(Settings.AccessToken))
            {
                if (Settings.AccessTokenExpirationDate < DateTime.UtcNow.AddHours(1))
                {
                    var loginViewModel = new LoginViewModel();
                    loginViewModel.LoginCommand.Execute(null);
                }
                App.Current.MainPage = new NavigationPage(new MasterPage());
            }
            else if (!string.IsNullOrEmpty(Settings.Username)
                  && !string.IsNullOrEmpty(Settings.Password))
            {
                App.Current.MainPage = new NavigationPage(new LoginPage());
            }
            else
            {
                App.Current.MainPage = new NavigationPage(new LoginPage());
            }


        }

    }
}
