﻿using ElGas.ViewModels;
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
	public partial class RecoveryPassPage : ContentPage
	{
        RecoveryPassViewModel viewModel = new RecoveryPassViewModel("");
		public RecoveryPassPage ()
		{
			InitializeComponent ();
            BindingContext = viewModel;
		}
	}
}