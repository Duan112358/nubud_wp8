using Microsoft.Phone.Controls;
using Nubud.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubud.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public SettingsViewModel()
        {
            FontSize = Utils.SMALL;
            Theme = Utils.LIGHT;
            DoubleTapExit = true;
        }

        private int fontSie;
        public int FontSize
        {
            get { return fontSie; }
            set
            {
                if (fontSie != value)
                {
                    fontSie = value;
                    NotifyPropertyChanged("FontSize");
                }
            }
        }

        private string theme;
        public string Theme
        {
            get { return theme; }
            set
            {
                if (theme != value)
                {
                    theme = value;
                    NotifyPropertyChanged("Theme");
                    if (IsDark)
                    {
                        ThemeManager.OverrideOptions = ThemeManagerOverrideOptions.SystemTrayAndApplicationBars;
                    }
                    ThemeManager.InvertTheme();
                }
            }
        }

        public bool IsDark { get { return Theme == Utils.DARK; } }

        private bool doubleTap;
        public bool DoubleTapExit
        {
            get { return doubleTap; }
            set
            {
                if (doubleTap != value)
                {
                    doubleTap = value;
                    NotifyPropertyChanged("DoubleTapExit");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
