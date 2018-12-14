using ElGas.Models;
using ElGas.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ElGas.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AfterFBPage : ContentPage
    {
        public AfterFBPage(FacebookProfile facebookProfile)
        {
            InitializeComponent();

            //Message.Text = string.Format("Bienvenido {0}, ayudanos con estos datos para poder brindarte el mejor servicio.",facebookProfile.FullName);

            BindingContext = new AfterFBViewModel(facebookProfile);

        }
    }
}