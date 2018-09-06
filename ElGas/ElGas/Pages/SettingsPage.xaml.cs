using ElGas.ViewModels;
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace ElGas.Pages
{
    public partial class SettingsPage : ContentPage
    {
        SettingsViewModel viewModel;
        public SettingsPage()
        {
            viewModel = new SettingsViewModel();
            BindingContext = viewModel;
            InitializeComponent();
        }
    }
}
