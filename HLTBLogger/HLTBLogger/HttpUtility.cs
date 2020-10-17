﻿using HLTBLogger.ViewModel;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace HLTBLogger
{
    public class HttpUtility
    {
        public bool IsLoggedIn { get; private set; }
        private HttpClient Client;
        private HLTBLogger.App CurrentApp { get => App.Current as HLTBLogger.App; }

        public HttpUtility() {
            IsLoggedIn = false;

            var HttpClientHandler = new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };

            Client = new HttpClient(HttpClientHandler)
            {
                Timeout = new TimeSpan(0,0,5)
            };
        }

        public async Task<bool> Login()
        {
            var data = new Dictionary<string, string>();
            data["username"] = CurrentApp.HLTBUsername;
            data["password"] = CurrentApp.HLTBPassword;
            data["Submit"] = "Login";
            data["recaptcha_response"] = "";

            var body = new FormUrlEncodedContent(data);

            var response = await Client.PostAsync("https://howlongtobeat.com/login?t=in", body);
            var content = await response.Content.ReadAsStringAsync();

            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            // If we find the password field, an error has occured and we did not successfully sign in.
            // This is my hack because HLTB always returns status 200 no matter what.
            var passwordFormField = doc.GetElementbyId("password");
            IsLoggedIn = passwordFormField == null;

            return IsLoggedIn;
        }

        public async Task<IEnumerable<GameInfo>> GetCurrentGames()
        {
            var currentApp = (App.Current as HLTBLogger.App);

            string username = currentApp.HLTBUsername;

            var data = new Dictionary<string, string>();
            data["n"] = username;
            data["playing"] = "1";

            var body = new System.Net.Http.FormUrlEncodedContent(data);

            var response = await Client.PostAsync("https://howlongtobeat.com/user_games_list", body);
            var content = await response.Content.ReadAsStringAsync();

            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            return doc.DocumentNode.SelectNodes("//table[@class='user_game_list']//a")
                .Where(node => !String.IsNullOrWhiteSpace(node.InnerText))
                .Select(node =>
                    new GameInfo()
                    {
                        Name = node.InnerText?.Trim(),
                        HLTBGameID = node.Attributes["href"]?.Value.Split('=').Last()
                    });
        }
    }
}