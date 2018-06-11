using ElGas.Helpers;
using ElGas.Pages;
using ElGas.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace ElGas
{
	public partial class App : Application
	{
        public static NavigationPage Navigator { get; internal set; }
        public App ()
		{
			InitializeComponent();
            SetMainPage();
			//MainPage = new NavigationPage( new LoginPage());

		}

        private void SetMainPage()
        {
            if (!string.IsNullOrEmpty(Settings.AccessToken))
            {
                if (Settings.AccessTokenExpirationDate < DateTime.UtcNow.AddHours(1))
                {
                    var loginViewModel = new LoginViewModel();
                    loginViewModel.LoginCommand.Execute(null);
                }
                MainPage = new NavigationPage(new MapaPage());
            }
            else if (!string.IsNullOrEmpty(Settings.Username)
                  && !string.IsNullOrEmpty(Settings.Password))
            {
                MainPage = new NavigationPage(new LoginPage());
            }
            else
            {
                MainPage = new NavigationPage(new LoginPage());
            }
        }
        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
