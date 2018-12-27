using ElGas.ViewModels;
using Rg.Plugins.Popup.Pages;
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
	public partial class Calificacion : ContentPage
    {
        CalificacionViewModel viewModel;

       
        public Calificacion ()
		{
            viewModel = new CalificacionViewModel();
            BindingContext = viewModel;
            InitializeComponent ();
		}
	}
}