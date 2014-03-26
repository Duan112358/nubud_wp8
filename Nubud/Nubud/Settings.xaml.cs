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
using Microsoft.Phone.Tasks;

namespace Nubud
{
    public partial class Settings : BasePage
    {
        public Settings()
        {
            InitializeComponent();
            var settings = App.SettingsViewModel;
            DataContext = settings;
            Theme.SelectedIndex = settings.IsDark ? 0 : 1;
            Reading.SelectedIndex = settings.FontSize == Utils.SMALL ? 0 : (settings.FontSize == Utils.MEDIUM ? 1 : 2);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private void Offical_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask browser = new WebBrowserTask();
            browser.Uri = new Uri(AppResources.OfficalSite, UriKind.Absolute);
            browser.Show();
        }

        private void Company_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask browser = new WebBrowserTask();
            browser.Uri = new Uri(AppResources.CompanySite, UriKind.Absolute);
            browser.Show();
        }

        private void ClearCache_Click(object sender, RoutedEventArgs e)
        {
            App.ArticlesViewModel.ClearCache();
            Utils.ShowToastNotify(AppResources.ClearCacheMessage);
        }
    }
}