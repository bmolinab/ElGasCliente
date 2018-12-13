using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace ElGas.Models
{
    public class FacebookProfile : INotifyPropertyChanged
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Email { get; set; }

        
        //public UriImageSource Cover { get; set; }
        //public UriImageSource Picture { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
