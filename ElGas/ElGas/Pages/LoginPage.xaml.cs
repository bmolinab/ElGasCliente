﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElGas.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ElGas.Pages
{
	//[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
        
		public LoginPage ()
		{
			InitializeComponent ();
            BindingContext = new LoginViewModel();
           
                                                   
		}


	}
}