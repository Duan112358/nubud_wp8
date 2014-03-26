using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Nubud.Services;
using Nubud.Resources;
using System.Diagnostics;
using Newtonsoft.Json;
using Nubud.Models;

namespace Nubud
{
    public partial class Login : PhoneApplicationPage
    {
        public Login()
        {
            InitializeComponent();

            DataContext = App.UserViewModel;
        }

        private void LoginAction_Click(object sender, RoutedEventArgs e)
        {
            var LoginModel = App.UserViewModel.LoginModel;

            if (Utils.IsNetworkAvailable())
            {
                LoginAction.IsEnabled = false;
                if (!LoginModel.RegisterMode)
                {
                    ProcessLogin(LoginModel);
                }
                else
                {
                    ProcessRegister(LoginModel);
                }
            }
            else
            {
                Utils.ShowToastNotify(AppResources.NetworkError, "");
            }

        }

        private async void ProcessRegister(Models.LoginModel LoginModel)
        {
            if (!string.IsNullOrEmpty(LoginModel.Email) &&
                    LoginModel.IsEmailValid() &&
                    !string.IsNullOrEmpty(LoginModel.Password)
                    && Object.Equals(LoginModel.Password, LoginModel.ConfirmPassword))
            {
                App.UserViewModel.IsLoading = true;
                var loginResult = await NubudService.Instance.Register(LoginModel);

                if (loginResult.IsSuccessStatusCode)
                {
                    LoginModel.RegisterMode = false;
                    LoginModel.IsAuthed = true;
                    Utils.SaveLoginModel(LoginModel);

                    NavigationService.Navigate(new Uri("/MainPage.xaml?IsAuthed=true", UriKind.Relative));
                }

                if (loginResult.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Utils.ShowToastNotify(AppResources.LoginError);
                }

                App.UserViewModel.IsLoading = false;
                LoginAction.IsEnabled = true;
            }
            else
            {
                if (!LoginModel.IsEmailValid())
                {
                    Utils.ShowToastNotify(AppResources.InvalidEmail);
                }
                else if (!Object.Equals(LoginModel.Password, LoginModel.ConfirmPassword))
                {
                    Utils.ShowToastNotify(AppResources.PasswordError);
                }
                LoginAction.IsEnabled = true;
            }
        }

        private async void ProcessLogin(Models.LoginModel LoginModel)
        {
            if (!string.IsNullOrEmpty(LoginModel.Email) &&
                    LoginModel.IsEmailValid() &&
                    !string.IsNullOrEmpty(LoginModel.Password))
            {
                App.UserViewModel.IsLoading = true;
                var loginResult = await NubudService.Instance.Login(LoginModel);
                var content = await loginResult.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(content))
                {
                    Utils.ShowToastNotify(AppResources.LoginError);
                }
                else
                {
                    LoginModel.RegisterMode = false;
                    LoginModel.IsAuthed = true;
                    Utils.SaveLoginModel(LoginModel);
                    App.UserViewModel.IsLoading = false;

                    NavigationService.Navigate(new Uri("/MainPage.xaml?IsAuthed=true", UriKind.Relative));
                }
                LoginAction.IsEnabled = true;
            }
            else
            {
                Utils.ShowToastNotify(AppResources.InvalidEmail);
            }
        }

        private void ToggleRegisterModel_Click(object sender, RoutedEventArgs e)
        {
            LoginAction.IsEnabled = true;
            var loginModel = App.UserViewModel.LoginModel;
            loginModel.RegisterMode = !loginModel.RegisterMode;

            if (loginModel.RegisterMode)
            {
                ConfirmPassword.Visibility = System.Windows.Visibility.Visible;
                LoginAction.Content = AppResources.Register;
                LoginLink.Content = AppResources.Login;
                PageTitle.Text = AppResources.Register;
            }
            else
            {
                ConfirmPassword.Visibility = System.Windows.Visibility.Collapsed;
                LoginAction.Content = AppResources.Login;
                LoginLink.Content = AppResources.Register;
                PageTitle.Text = AppResources.Login;
            }

        }

        private void ConfirmPassword_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var loginModel = App.UserViewModel.LoginModel;
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                LoginAction_Click(sender, e);
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            App.Current.Terminate();
            base.OnBackKeyPress(e);
        }

    }
}