﻿using ElGas.Models;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;

using System.Collections.Generic;
using System.Text;

namespace ElGas.Helpers
{
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }
        public static string Username
        {
            get
            {
                return AppSettings.GetValueOrDefault("Username", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("Username", value);
            }
        }

        public static double HoraIncial
        {
            get
            {
                return AppSettings.GetValueOrDefault("HoraIncial", 0.0);
            }
            set
            {
                AppSettings.AddOrUpdateValue("HoraIncial", value);
            }
        }
        public static double HoraFinal
        {
            get
            {
                return AppSettings.GetValueOrDefault("HoraFinal", 0.0);
            }
            set
            {
                AppSettings.AddOrUpdateValue("HoraFinal", value);
            }
        }

        public static string DeviceID
        {
            get
            {
                return AppSettings.GetValueOrDefault("DeviceID", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("DeviceID", value);
            }
        }

        public static string HoraEntrega
        {
            get
            {
                return AppSettings.GetValueOrDefault("HoraEntrega", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("HoraEntrega", value);
            }
        }

        public static int TanquesGas
        {
            get
            {
                return AppSettings.GetValueOrDefault("TanquesGas", 0);
            }
            set
            {
                AppSettings.AddOrUpdateValue("TanquesGas", value);
            }
        }

        public static int CantidadSeguimientoPage
        {
            get
            {
                return AppSettings.GetValueOrDefault("CantidadSeguimientoPage", 0);
            }
            set
            {
                AppSettings.AddOrUpdateValue("CantidadSeguimientoPage", value);
            }
        }


        public static int CantidadCalificacionPage
        {
            get
            {
                return AppSettings.GetValueOrDefault("CantidadCalificacionPage", 0);
            }
            set
            {
                AppSettings.AddOrUpdateValue("CantidadCalificacionPage", value);
            }
        }


        public static int IdCompra
        {
            get
            {
                return AppSettings.GetValueOrDefault("IdCompra", 0);
            }
            set
            {
                AppSettings.AddOrUpdateValue("IdCompra", value);
            }
        }

        public static int idCliente
        {
            get
            {
                return AppSettings.GetValueOrDefault("idCliente", 0);
            }
            set
            {
                AppSettings.AddOrUpdateValue("idCliente", value);
            }
        }
        public static int IdDistribuidor
        {
            get
            {
                return AppSettings.GetValueOrDefault("IdDistribuidor", 0);
            }
            set
            {
                AppSettings.AddOrUpdateValue("IdDistribuidor", value);
            }
        }


        public static string Password
        {
            get
            {
                return AppSettings.GetValueOrDefault("Password", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("Password", value);
            }
        }
        public static string AccessToken
        {
            get
            {
                return AppSettings.GetValueOrDefault("AccessToken", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("AccessToken", value);
            }
        }
        public static DateTime AccessTokenExpirationDate
        {
            get
            {
                return AppSettings.GetValueOrDefault("AccessTokenExpirationDate", DateTime.UtcNow);
            }
            set
            {
                AppSettings.AddOrUpdateValue("AccessTokenExpirationDate", value);
            }
        }
        public static bool Pedidos 
        {
            get
            {
                return AppSettings.GetValueOrDefault("Pedidos", false);
            }
            set
            {
                AppSettings.AddOrUpdateValue("Pedidos", value);
            }
        }

        public static bool Calificar
        {
            get
            {
                return AppSettings.GetValueOrDefault("Calificar", false);
            }
            set
            {
                AppSettings.AddOrUpdateValue("Calificar", value);
            }
        }


        public static Double Precio
        {
            get
            {
                return AppSettings.GetValueOrDefault("Precio", 3.5);
            }
            set
            {
                AppSettings.AddOrUpdateValue("Precio", value);
            }
        }

        public static string Direccion
        {
            get
            {
                return AppSettings.GetValueOrDefault("Direccion", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("Direccion", value);
            }
        }

        public static string NombreCompleto
        {
            get
            {
                return AppSettings.GetValueOrDefault("NombreCompleto", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("NombreCompleto", value);
            }
        }


    }
}
