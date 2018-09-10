using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ElGas.ViewModels;

namespace ElGas.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PedidosPage : ContentPage
	{

        public PedidosPage ()
		{
			InitializeComponent ();
            BindingContext = new PedidosViewModel();
		}

        protected override void OnAppearing()
        {
            // base.OnAppearing();
            BindingContext = new PedidosViewModel();

            //your code here;

        }
    }
}