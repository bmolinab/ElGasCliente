using System;
using System.Collections.Generic;
using System.Text;

namespace ElGas.ViewModels
{
    public class MainViewModel
    {
        #region ViewModels
        public LoginViewModel Login
        {
            get;
            set;        
        }
        #endregion
        #region Constructor
        public MainViewModel()
        {
            this.Login = new LoginViewModel();

        }
        #endregion
    }
}
