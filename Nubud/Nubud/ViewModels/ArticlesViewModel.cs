using Lex.Db;
using Newtonsoft.Json;
using Nubud.Converters;
using Nubud.Models;
using Nubud.Resources;
using Nubud.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Nubud.ViewModels
{
    public class ArticlesViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ArticleModel> _articles;
        private ObservableCollection<ArticleModel> _archieved;
        private bool _isLoading;

        public DbInstance db;

        public ArticlesViewModel()
        {
            db = new DbInstance("articles.db");
            db.Map<ArticleContentModel>().Automap(i => i.ArticleID);
            db.Map<ArticleModel>().Automap(i => i.ID);
            db.InitializeAsync();
            _articles = new ObservableCollection<ArticleModel>();
            _archieved = new ObservableCollection<ArticleModel>();
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyPropertyChanged("IsLoading");
                }
            }
        }

        public bool IsLoaded { get { return !IsLoading; } }

        private bool _mutipleSelectedEnabled;
        public bool MutipleSelectEnabled
        {
            get { return _mutipleSelectedEnabled; }
            set
            {
                if (_mutipleSelectedEnabled != value)
                {
                    _mutipleSelectedEnabled = value;
                    NotifyPropertyChanged("MutipleSelectEnabled");
                }
            }
        }

        public ObservableCollection<ArticleModel> Articles { get { return _articles; } }

        public ObservableCollection<ArticleModel> Archived { get { return _archieved; } }


        internal ArticleModel GetArticle(string id)
        {
            var article = Articles.FirstOrDefault(a => a.ID == id);
            if (article != null)
            {
                return article;
            }
            else
            {
                return Archived.FirstOrDefault(a => a.ID == id);
            }
        }

        internal void ClearCache()
        {
            LocalCache.ClearLocalStorage();
            Articles.Clear();
            ClearDB();
        }

        internal void LoadArchived(bool refresh = false)
        {
            if (refresh)
            {
                Archived.Clear();
            }
            if (!Archived.Any())
            {
                var archiveds = db.Table<ArticleModel>().Where(a => a.IsArchived).OrderByDescending(a => a.ID);
                foreach (var a in archiveds)
                {
                    if (!Archived.Any(_ => _.ID == a.ID))
                    {
                        Archived.Insert(0, a);
                    }
                }
            }
        }

        public async Task<bool> UpdateStatus(ArticleModel article)
        {
            if (Utils.IsNetworkAvailable())
            {
                IsLoading = true;
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("IsArchived", article.IsArchived.ToString()));
                values.Add(new KeyValuePair<string, string>("IsFavorited", article.IsFavorited.ToString()));
                var updateResult = await NubudService.Instance.Put(string.Format("/articles/{0}/status", article.ID), values);
                IsLoading = false;
            }

            AddArticle(article);
            if (article.IsArchived)
            {
                var target = Articles.FirstOrDefault(a => a.ID == article.ID);
                Articles.Remove(target);
                Archived.Insert(0, article);
            }
            else
            {
                Articles.Insert(0, article);
                var target = Archived.FirstOrDefault(a => a.ID == article.ID);
                Archived.Remove(target);
            }

            LoadArchived(true);
            return true;
        }

        public async void LoadArticles(bool hasLogin = false)
        {
            if (Utils.IsNetworkAvailable())
            {
                IsLoading = true;
                if (hasLogin)
                {
                    var articlesResult = await NubudService.Instance.Get("/articles/data");
                    if (articlesResult.IsSuccessStatusCode)
                    {
                        var articleContent = await articlesResult.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<IEnumerable<ArticleModel>>(articleContent);

                        FilterData(data);
                        IsLoading = false;
                    }
                }
                else
                {
                    var service = NubudService.Instance;
                    var articlesResult = await service.Login(App.UserViewModel.LoginModel).ContinueWith(_ => service.Get("/articles/data").Result);
                    if (articlesResult.IsSuccessStatusCode)
                    {
                        var articleContent = await articlesResult.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<IEnumerable<ArticleModel>>(articleContent);
                        FilterData(data);
                        IsLoading = false;
                    }
                }
            }
            else
            {
                IsLoading = true;
                var articlesInDB = await db.Table<ArticleModel>().LoadAllAsync();
                FilterData(articlesInDB.OrderByDescending(a => a.ID));
                IsLoading = false;
            }
        }

        public async Task<bool> AddUrl(string url)
        {
            IsLoading = true;
            var data = new List<KeyValuePair<string, string>>();
            data.Add(new KeyValuePair<string, string>("url", url));
            var result = await NubudService.Instance.Post("/articles", data);
            if (result.IsSuccessStatusCode)
            {
                var stringContent = await result.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject(stringContent) as ErrorModel;
                if (error != null)
                {
                    return false;
                }
                else
                {
                    var nubud = JsonConvert.DeserializeObject<ArticleModel>(stringContent);
                    if (!nubud.Title.StartsWith("------"))
                    {
                        Articles.Insert(0, nubud);
                        AddArticle(nubud);
                        return true;
                    }
                }
                IsLoading = false;
            }

            return false;
        }

        private void FilterData(IEnumerable<ArticleModel> data)
        {
            Articles.Clear();
            Archived.Clear();

            foreach (var article in data)
            {
                if (!Articles.Any(d => d.ID == article.ID))
                {
                    if (!article.IsArchived)
                    {
                        Articles.Add(article);
                    }
                    else
                    {
                        Archived.Add(article);
                    }
                }
                AddArticle(article);
            }
        }

        public async Task<ArticleContentModel> LoadArticleContent(string id)
        {
            var contentTable = db.Table<ArticleContentModel>();
            if (contentTable.Any(c => c.ArticleID == id))
            {
                var result = await contentTable.LoadByKeyAsync(id);
                return result;
            }
            else
            {
                if (Utils.IsNetworkAvailable())
                {
                    var contentResult = await NubudService.Instance.Get(string.Format("/articles/{0}/content", id));
                    if (contentResult.IsSuccessStatusCode)
                    {
                        var content = JsonConvert.DeserializeObject<ArticleContentModel>(await contentResult.Content.ReadAsStringAsync());
                        content.ArticleID = id;
                        await contentTable.SaveAsync(content);

                        return content;
                    }
                }
                else
                {
                    Utils.ShowToastNotify(AppResources.NetworkError);
                }
            }
            return null;
        }

        #region Local Database Operation
        internal async Task<bool> DeleteArticle(string id)
        {
            if (Utils.IsNetworkAvailable())
            {
                var deleteResult = await NubudService.Instance.Delete(id);
                var target = Articles.FirstOrDefault(e => e.ID == id);
                if (target != null)
                {
                    Articles.Remove(target);
                    await db.Table<ArticleModel>().DeleteByKeyAsync(id);
                    await db.Table<ArticleContentModel>().DeleteByKeyAsync(id);
                }
                return true;
            }
            return false;
        }

        async void AddArticle(ArticleModel entity)
        {
            var articlesTable = db.Table<ArticleModel>();
            await articlesTable.SaveAsync(entity);
        }

        async void ClearDB()
        {
            await db.PurgeAsync();
        }

        #endregion

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
