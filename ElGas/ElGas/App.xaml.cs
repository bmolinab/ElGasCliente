using ElGas.Helpers;
using ElGas.Pages;
using ElGas.Services;
using ElGas.ViewModels;
using System;
using TK.CustomMap.Api.Google;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

//[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
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

            GmsPlace.Init("AIzaSyDAmhu79jCKlkE6KIVSqgxlIl83gJj_rkk");
            GmsDirection.Init("AIzaSyDAmhu79jCKlkE6KIVSqgxlIl83gJj_rkk");

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjQzMkAzMTM2MmUzMjJlMzBiRXp6WjNDdFdzZTRjcmc4YWxJdXU2eVZ2OHhWOWJ0dHhQakVvZ0YvZmNZPQ==");
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
