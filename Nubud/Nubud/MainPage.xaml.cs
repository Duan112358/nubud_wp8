using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Nubud.Models;
using Nubud.Resources;
using Nubud.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nubud
{
    public partial class MainPage : BasePage
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = App.ArticlesViewModel;
            BuildLocalizedApplicationBar();

            AddUrlOverlay.VisibilityChanged += AddUrlOverlay_VisibilityChanged;
            FeedbackOverlay.VisibilityChanged += FeedbackOverlay_VisibilityChanged;
        }

        private async void AddUrlOverlay_VisibilityChanged(object sender, EventArgs e)
        {
            if (AddUrlOverlay.Visibility == System.Windows.Visibility.Collapsed && object.Equals("add", AddUrlOverlay.Tag))
            {
                if (!Utils.IsNetworkAvailable())
                {
                    Utils.ShowToastNotify(AppResources.NetworkError);
                    return;
                }
                var url = AddUrlOverlay.Url.Text;
                if (!string.IsNullOrEmpty(url) && url.StartsWith("http://") || url.StartsWith("https://"))
                {
                    var addResult = await App.ArticlesViewModel.AddUrl(url);
                    if (!addResult)
                    {
                        Utils.ShowToastNotify(AppResources.AddError);
                    }
                }
                else
                {
                    Utils.ShowToastNotify(AppResources.InvalidUrl);
                }
            }
        }

        private void FeedbackOverlay_VisibilityChanged(object sender, EventArgs e)
        {
            if (ApplicationBar != null)
            {
                ApplicationBar.IsVisible = (FeedbackOverlay.Visibility != Visibility.Visible);
                if (FeedbackOverlay.Visibility == System.Windows.Visibility.Visible)
                {
                    ApplicationBar.IsVisible = false;
                }
                else
                {
                    ApplicationBar.IsVisible = true;
                }
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.Keys.Contains("IsAuthed"))
            {
                NavigationService.RemoveBackEntry();
                App.ArticlesViewModel.LoadArticles(true);
            }
            else
            {
                if (!App.UserViewModel.HasLogin())
                {
                    NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
                }
                else
                {
                    if (!App.ArticlesViewModel.Articles.Any())
                    {
                        App.ArticlesViewModel.LoadArticles();
                    }
                }
            }

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            var qs = NavigationContext.QueryString;
            base.OnNavigatedFrom(e);
        }

        private void BuildLocalizedApplicationBar()
        {
            if (!App.SettingsViewModel.IsDark)
            {
                ApplicationBar.BackgroundColor = Color.FromArgb(255, 50, 205, 200);
            }

            Refresh = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
            Refresh.Text = AppResources.Refresh;

            Add = ApplicationBar.Buttons[1] as ApplicationBarIconButton;
            Add.Text = AppResources.Add;

            Settings = ApplicationBar.MenuItems[0] as ApplicationBarMenuItem;
            Settings.Text = AppResources.Settings;

            Logout = ApplicationBar.MenuItems[1] as ApplicationBarMenuItem;
            Logout.Text = AppResources.Logout;
        }
        private void ArticlesList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selected = ArticlesList.SelectedItem as ArticleModel;
            if (selected != null)
            {
                NavigationService.Navigate(new Uri(string.Format("/DetailsPage.xaml?ID={0}", selected.ID), UriKind.Relative));
            }
            ArticlesList.SelectedItem = null;
        }

        private void ArchivedList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = ArchivedList.SelectedItem as ArticleModel;
            if (selected != null)
            {
                NavigationService.Navigate(new Uri(string.Format("/DetailsPage.xaml?ID={0}", selected.ID), UriKind.Relative));
            }
            ArchivedList.SelectedItem = null;
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            App.ArticlesViewModel.LoadArticles(true);
            App.ArticlesViewModel.LoadArchived(true);
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Add_Click(object sender, EventArgs e)
        {
            AddUrlOverlay.Show();
            AddUrlOverlay.Url.SelectionStart = AddUrlOverlay.Url.Text.Length;
            AddUrlOverlay.Url.Focus();
        }

        private void Logout_Click(object sender, EventArgs e)
        {
            App.UserViewModel.Logout();
            NavigationService.Navigate(new Uri("/Login.xaml", UriKind.RelativeOrAbsolute));
        }

        private void ClearCache_Click(object sender, EventArgs e)
        {
            App.ArticlesViewModel.ClearCache();
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Pivot.SelectedIndex == 1)
            {
                var viewModel = App.ArticlesViewModel;
                if (!viewModel.Archived.Any())
                {
                    viewModel.LoadArchived();
                }
            }
        }
    }
}