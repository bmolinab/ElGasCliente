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
	public partial class RecoveryPassPage2 : ContentPage
	{
        RecoveryPassViewModel viewModel;
        public RecoveryPassPage2(string username)
        {
           viewModel= new RecoveryPassViewModel(username);
            InitializeComponent();
            BindingContext = viewModel;
        }
	}
}