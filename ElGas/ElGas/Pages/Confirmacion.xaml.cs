using ElGas.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TK.CustomMap;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ElGas.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Confirmacion : ContentPage
	{
        ConfirmacionViewModel viewModel;
		public Confirmacion (TKCustomMapPin posicion)
		{
            viewModel = new ConfirmacionViewModel(posicion);
            BindingContext = viewModel;
			InitializeComponent ();
        
		}
	}
}