using Nubud.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nubud.Services
{
    public class NubudService
    {
        private static NubudService _service;
        private HttpClient client;

        private NubudService()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://beta.nubud.net");
        }

        public static NubudService Instance
        {
            get
            {
                if (_service == null)
                {
                    _service = new NubudService();
                }
                return _service;
            }
        }

        public Task<HttpResponseMessage> Get(string url)
        {
            return client.GetAsync(url);
        }

        public Task<HttpResponseMessage> Post(string url, List<KeyValuePair<string, string>> data)
        {
            return client.PostAsync(url, new FormUrlEncodedContent(data));
        }

        public Task<HttpResponseMessage> Put(string url, List<KeyValuePair<string, string>> data)
        {
            return client.PutAsync(url, new FormUrlEncodedContent(data));
        }

        public Task<HttpResponseMessage> Delete(string id)
        {
            return client.DeleteAsync(string.Format("/articles/{0}", id));
        }

        public Task<HttpResponseMessage> Login(LoginModel model)
        {
            var values = new List<KeyValuePair<string, string>>();
            values.Add(new KeyValuePair<string, string>("email", model.Email));
            values.Add(new KeyValuePair<string, string>("password", model.Password));
            return client.PostAsync("/login", new FormUrlEncodedContent(values));
        }

        public Task<HttpResponseMessage> Register(LoginModel model)
        {
            var values = new List<KeyValuePair<string, string>>();
            values.Add(new KeyValuePair<string, string>("email", model.Email));
            values.Add(new KeyValuePair<string, string>("password", model.Password));
            values.Add(new KeyValuePair<string, string>("confirmpassword", model.ConfirmPassword));
            return client.PostAsync("/register", new FormUrlEncodedContent(values));
        }

    }
}
