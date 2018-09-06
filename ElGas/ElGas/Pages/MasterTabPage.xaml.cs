using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace ElGas.Pages
{
    public partial class MasterTabPage : TabbedPage
    {
        int vista = 0;
        public MasterTabPage()
        {
            InitializeComponent();
            App.Navigator = NavigatorM;

            Tbpage.CurrentPageChanged += Tbpage_CurrentPageChanged;

        }

        private void Tbpage_CurrentPageChanged(object sender, EventArgs e)
        {
            TabbedPage np = (TabbedPage)sender;


            switch (np.CurrentPage.Title)
            {
                case "Histórico":
                    App.Navigator = NavigatorH;
                    break;
                case "Configuración":
                    App.Navigator = NavigatorS;
                    break;
                case "Inicio":
                    App.Navigator = NavigatorM;
                    break;
                default:
                    App.Navigator = NavigatorM;
                    break;
            }

        }

    }
}
