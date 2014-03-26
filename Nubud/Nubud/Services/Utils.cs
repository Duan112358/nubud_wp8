using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Nubud.Models;
using Nubud.ViewModels;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Nubud.Services
{
    public class Utils
    {
        public const string EMAIL_SCHEMA = @"^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$";
        private static IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        private const string USER_KEY = "__user__info__";
        private const string SETTINGS_KEY = "__settings__info";

        public static readonly int SMALL = 15;
        public static readonly int MEDIUM = 20;
        public static readonly int LARGE = 25;

        public static readonly string DARK = "DARK";
        public static readonly string LIGHT = "LIGHT";
        public static bool IsNetworkAvailable()
        {
            return DeviceNetworkInformation.IsWiFiEnabled || DeviceNetworkInformation.IsCellularDataEnabled;
        }

        public static LoginModel GetLoginModel()
        {
            if (settings.Contains(USER_KEY))
            {
                return settings[USER_KEY] as LoginModel;
            }

            return null;
        }

        public static void SaveLoginModel(LoginModel loginModel)
        {
            if (settings.Contains(USER_KEY))
            {
                settings[USER_KEY] = loginModel;
            }
            else
            {
                settings.Add(USER_KEY, loginModel);
            }
            settings.Save();
        }

        public static void ClearLoginModel()
        {
            if (settings.Contains(USER_KEY))
            {
                settings.Remove(USER_KEY);
            }
            settings.Save();
        }

        public static void SaveSettings(SettingsViewModel set)
        {
            if (settings.Contains(SETTINGS_KEY))
            {
                settings[SETTINGS_KEY] = set;
            }
            else
            {
                settings.Add(SETTINGS_KEY, set);
            }
            settings.Save();
        }

        public static SettingsViewModel GetSettings()
        {
            if (settings.Contains(SETTINGS_KEY))
            {
                return settings[SETTINGS_KEY] as SettingsViewModel;
            }
            else
            {
                var model = new SettingsViewModel();
                settings.Add(SETTINGS_KEY, model);
                return model;
            }
        }

        public static void ShowToastNotify(string message, string title = "", int timeout = 2000, double fontSize = 20, SolidColorBrush forgroundColor = null, SolidColorBrush backgroundColor = null)
        {
            ToastPrompt toastPrompt = new ToastPrompt()
            {
                Message = message,
                Title = title,
                IsTimerEnabled = true,
                MillisecondsUntilHidden = timeout,
                IsAppBarVisible = true,
                FontSize = fontSize
            };

            if (forgroundColor == null)
                toastPrompt.Foreground = new SolidColorBrush(Colors.White);
            else
                toastPrompt.Foreground = forgroundColor;

            if (backgroundColor == null)
                toastPrompt.Background = new SolidColorBrush(GetColorFromHexString("32CD32")); //new SolidColorBrush(Colors.Green);
            else
                toastPrompt.Background = backgroundColor;

            toastPrompt.TextOrientation = System.Windows.Controls.Orientation.Horizontal;
            toastPrompt.Show();
        }

        public static Color GetColorFromHexString(string s)
        {
            byte a = System.Convert.ToByte("FF", 16);//Alpha should be 255
            byte r = System.Convert.ToByte(s.Substring(0, 2), 16);
            byte g = System.Convert.ToByte(s.Substring(2, 2), 16);
            byte b = System.Convert.ToByte(s.Substring(4, 2), 16);
            return Color.FromArgb(a, r, g, b);
        }
    }
}
