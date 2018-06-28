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
	public partial class SeguimientoPage : ContentPage
	{
        SeguimientoViewModel viewModel;

        public SeguimientoPage ()
		{
            viewModel = new SeguimientoViewModel();
            BindingContext = viewModel;
            InitializeComponent ();
		}
	}
}