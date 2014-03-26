using Nubud.Services;
using Nubud.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nubud.Models
{
    public class LoginModel : INotifyPropertyChanged
    {
        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                if (_email != value)
                {
                    _email = value;
                    NotifyPropertyChanged("Email");
                }
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    NotifyPropertyChanged("Password");
                }
            }
        }

        public string ConfirmPassword { get; set; }

        public bool IsEmailValid()
        {
            return !string.IsNullOrEmpty(Email) && Regex.IsMatch(Email, Utils.EMAIL_SCHEMA);
        }

        public bool IsEmpty
        {
            get
            {
                return RegisterMode ? string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Password) && object.Equals(Password, ConfirmPassword) :
                string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Password);
            }
        }

        private bool isAuthed;
        public bool IsAuthed
        {
            get { return isAuthed; }
            set
            {
                if (isAuthed != value)
                {
                    isAuthed = value;
                    NotifyPropertyChanged("IsAuthed");
                }
            }
        }

        private bool _registerMode;
        public bool RegisterMode
        {
            get { return _registerMode; }
            set
            {
                if (_registerMode != value)
                {
                    _registerMode = value;
                    NotifyPropertyChanged("LoginMode");
                }
            }
        }

        public string HelperText { get { return RegisterMode ? AppResources.RegisterHelperText : AppResources.LoginHelperText; } }

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
