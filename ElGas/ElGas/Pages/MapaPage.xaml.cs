﻿using ElGas.Helpers;
using ElGas.Services;
using ElGas.ViewModels;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TK.CustomMap;
using TK.CustomMap.Overlays;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ElGas.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MapaPage : ContentPage
	{
        MapaViewModel viewModel = new MapaViewModel();
        public MapaPage()
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Settings.CantidadSeguimientoPage==1)
            {
                Settings.CantidadSeguimientoPage = 0;

            }

            if (Settings.CantidadCalificacionPage == 1)
            {
                Settings.CantidadCalificacionPage = 0;

            }
        }
    }
}