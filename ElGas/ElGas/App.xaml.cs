using ElGas.Helpers;
using ElGas.Pages;
using ElGas.Services;
using ElGas.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace ElGas
{
	public partial class App : Application
    {
        public static MasterPage Master { get; internal set; }
        public static NavigationPage Navigator { get; internal set; }
        //Page MenuPage = new MenuPage();


        public App ()
		{
			InitializeComponent();
            //MainPage = new NavigationPage( new LoginPage());
            NavigationService navigationService = new NavigationService();
            navigationService.SetMainPage();
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
