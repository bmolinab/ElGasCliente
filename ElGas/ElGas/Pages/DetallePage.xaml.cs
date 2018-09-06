using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ElGas.Models;
using ElGas.ViewModels;
using System.Diagnostics;

namespace ElGas.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DetallePage : ContentPage
	{ 

		public DetallePage (ComprasRequest comprasRequest)
        {
            try
            {
                BindingContext = new DetalleViewModel(comprasRequest);

                InitializeComponent();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
           

		}
	}
}