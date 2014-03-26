using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Nubud.Models;
using Nubud.Resources;
using Nubud.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Nubud
{
    public partial class DetailsPage : BasePage
    {
        private ArticleModel article;
        private ArticleContentModel content;
        public DetailsPage()
        {
            InitializeComponent();
        }

        void InitApplicationBar()
        {
            Original = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
            Original.Text = AppResources.Original;
            Share = ApplicationBar.Buttons[1] as ApplicationBarIconButton;
            Share.Text = AppResources.Share;

            Check = ApplicationBar.Buttons[2] as ApplicationBarIconButton;
            if (!article.IsArchived)
            {
                Check.Text = AppResources.Archived;
            }
            else
            {
                Check.Text = AppResources.UnArchive;
            }

            Delete = ApplicationBar.MenuItems[0] as ApplicationBarMenuItem;
            Delete.Text = AppResources.Delete;

            Settings = ApplicationBar.MenuItems[1] as ApplicationBarMenuItem;
            Settings.Text = AppResources.Settings;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var qs = NavigationContext.QueryString;
            var articleID = qs["ID"];
            article = App.ArticlesViewModel.GetArticle(articleID);
            content = await App.ArticlesViewModel.LoadArticleContent(articleID);
            if (content != null)
            {
                InitApplicationBar();
                Browser.NavigateToString(AddCSS(content.Content));
                Browser.Navigated += async (sender, e1) =>
                {
                    Browser.Visibility = System.Windows.Visibility.Visible;
                    var delay = 200;
                    if (App.SettingsViewModel.IsDark)
                    {
                        delay = 1000;
                    }
                    await Task.Delay(delay).ContinueWith(_ =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            ProgressRing.IsActive = false;
                            Browser.Opacity = 1;
                        });
                    });
                };
            }
            else
            {
                ApplicationBar.IsVisible = false;
                ProgressRing.IsActive = false;
                Utils.ShowToastNotify(AppResources.NotCached);
            }
            base.OnNavigatedTo(e);
        }

        private string AddCSS(string content)
        {
            var darkTheme = App.SettingsViewModel.IsDark;
            string darkColor = darkTheme ? "#22aa22" : "#000000";
            var bgColor = darkTheme ? "#000000" : "#ffffff";
            var linkColor = darkTheme ? "a{color:#1122aa;}" : "";
            var fontSize = App.SettingsViewModel.FontSize;
            string preContent = "<style>" +
                "img {border-style:none;margin:0 auto;display:block;padding:0;text-align:center;height:auto;width:100%; }" +
                "body { font-family: 'Segoe UI'; margin-left:20px; margin-right:20px;font-size:" + fontSize + "px; color: " + darkColor + ";background-color:" + bgColor + "} " + linkColor +
                "</style>";
            string script = @"<script>function getImgSrc(){return var imgs =document.querySelectorAll('img');var imgSrcs=[];for(var i=0;i<imgs.length;i++){imgSrcs.push(imgs[i].src);}return imgSrcs;}</script>";
            return preContent + content + script;
        }

        private void Browser_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.GoBack();
        }

        private async void Delete_Click(object sender, System.EventArgs e)
        {
            if (content != null)
            {
                var result = await App.ArticlesViewModel.DeleteArticle(article.ID);
                if (result)
                {
                    NavigationService.GoBack();
                }
                else
                {
                    Utils.ShowToastNotify(AppResources.NetworkError);
                }
            }
        }

        private async void Check_Click(object sender, System.EventArgs e)
        {
            var originalLike = article.IsArchived;
            article.IsArchived = !article.IsArchived;
            if (content != null)
            {
                var result = await App.ArticlesViewModel.UpdateStatus(article);
                if (result)
                {
                    if (originalLike)
                    {
                        Check.Text = AppResources.Archive;
                    }
                    else
                    {
                        Check.Text = AppResources.UnArchive;
                    }
                    Utils.ShowToastNotify(AppResources.Success);
                }
                else
                {
                    Utils.ShowToastNotify(AppResources.NetworkError);
                }
            }
        }

        private void Settings_Click(object sender, System.EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        private void Original_Click(object sender, EventArgs e)
        {
            WebBrowserTask browser = new WebBrowserTask();
            browser.Uri = new Uri(content.Uri, UriKind.RelativeOrAbsolute);
            browser.Show();
        }

        private void Share_Click(object sender, EventArgs e)
        {
            ShareLinkTask share = new ShareLinkTask();
            share.LinkUri = new Uri(content.Uri);
            share.Title = article.Title;
            share.Message = "from Nubud.";
            share.Show();
        }
    }
}