using Microsoft.Phone.Controls;
using Nubud.Models;
using Nubud.Services;
using Nubud.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Nubud.ViewModels
{
    public class UserViewModel : INotifyPropertyChanged
    {
        private LoginModel _loginModel;
        private bool isLoading;

        public UserViewModel()
        {
            _loginModel = Utils.GetLoginModel() ?? new LoginModel();
        }

        public LoginModel LoginModel { get { return _loginModel; } }

        internal void Logout()
        {
            _loginModel = new LoginModel();
            Utils.SaveLoginModel(_loginModel);
        }

        internal bool HasLogin()
        {
            return _loginModel.IsAuthed;
        }

        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    NotifyPropertyChanged("IsLoading");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string prop)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
