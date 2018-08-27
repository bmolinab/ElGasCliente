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
	public partial class RegisterPage2 : ContentPage
	{
        RegisterViewModel ViewModels ;

     
        public RegisterPage2( Models.Cliente cliente)
		{
            ViewModels = new RegisterViewModel(cliente);
            InitializeComponent ();
            BindingContext = ViewModels;
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}