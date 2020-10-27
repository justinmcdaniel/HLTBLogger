using HLTBLogger.ViewModel;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Xamarin.Forms;
using System.IO;
using Xamarin.Forms.Internals;

namespace HLTBLogger
{
    public class HttpUtility
    {
        public static readonly string SUBMIT_PROTIME_HOURS_KEY = "protime_h";
        public static readonly string SUBMIT_PROTIME_MINUTES_KEY = "protime_m";
        public static readonly string SUBMIT_PROTIME_SECONDS_KEY = "protime_s";
        public static readonly string SUBMIT_COMPMONTH_KEY = "compmonth";
        public static readonly string SUBMIT_COMPDAY_KEY = "compday";
        public static readonly string SUBMIT_COMPYEAR_KEY = "compyear";

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
                Timeout = new TimeSpan(0,0,20)
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

        public async Task<bool> SubmitTime(GameInfo gameInfo, TimeSpan timeToAdd) 
        {
            //if (timeToAdd.TotalSeconds < 0)
            //{
            //    return true;
            //}

            var response = await Client.GetAsync("https://howlongtobeat.com/" + gameInfo.EditUrl);
            var content = await response.Content.ReadAsStringAsync();

            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            var inputNodes = doc.DocumentNode.SelectNodes("//form[@name='submit_form']//input");
            var selectNodes = doc.DocumentNode.SelectNodes("//select");

            var payloadData = new Dictionary<string, string>();
            foreach (var input in inputNodes)
            {
                if (input.Attributes["type"]?.Value.ToUpper() == "CHECKBOX")
                {
                    // Handle checkboxes by omitting if not checked.
                    payloadData[input.Attributes["name"].Value] = input.Attributes["checked"] == null ? String.Empty : "1";
                }
                else
                {
                    payloadData[input.Attributes["name"].Value] = input.Attributes["value"]?.Value ?? String.Empty;
                }
            }
            foreach (var select in selectNodes)
            {
                var payloadValue = String.Empty;
                var payloadValueNodes = doc.DocumentNode.SelectNodes($"//select[@name='{select.Attributes["name"].Value}']//*[@selected='selected']")
                    ?? doc.DocumentNode.SelectNodes($"//select[@name='{select.Attributes["name"].Value}']//option");
                var payloadValueNode = payloadValueNodes == null ? default : payloadValueNodes.First();

                if (payloadValueNode != default(HtmlNode))
                {
                    payloadValue = payloadValueNode.Attributes["value"]?.Value ?? String.Empty;
                }

                payloadData[select.Attributes["name"].Value] = payloadValue;
            }

            int hours = Convert.ToInt32(payloadData[HttpUtility.SUBMIT_PROTIME_HOURS_KEY]);
            int minutes = Convert.ToInt32(payloadData[HttpUtility.SUBMIT_PROTIME_MINUTES_KEY]);
            int seconds = Convert.ToInt32(payloadData[HttpUtility.SUBMIT_PROTIME_SECONDS_KEY]);
            TimeSpan newPlayTime = new TimeSpan(hours, minutes, seconds) + timeToAdd;

            payloadData[HttpUtility.SUBMIT_PROTIME_HOURS_KEY] = newPlayTime.Hours.ToString("0");
            payloadData[HttpUtility.SUBMIT_PROTIME_MINUTES_KEY] = newPlayTime.Minutes.ToString("0");
            payloadData[HttpUtility.SUBMIT_PROTIME_SECONDS_KEY] = newPlayTime.Seconds.ToString("0");

            var body = new FormUrlEncodedContent(payloadData);
            var submitPostResponse = await Client.PostAsync("https://howlongtobeat.com/submit", body);
            var submitPostContent = await submitPostResponse.Content.ReadAsStringAsync();
            

            return true;
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

            var gameInfoTBodyNodes = doc.DocumentNode.SelectNodes("//table[@class='user_game_list']/tbody")
                .ToList();

            var result = new List<GameInfo>();

            foreach (var tbodyNode in gameInfoTBodyNodes)
            {
                var gameInfo = new GameInfo();
                result.Add(gameInfo);

                this.extractNameAndHLTBGameID(gameInfo, tbodyNode);
                this.extractEditUrl(gameInfo, tbodyNode);
                await this.loadGameInfoDetails(gameInfo);
            }

            return result;
        }

        public async Task<ImageSource> GetImageFromUri(Uri imageUri)
        {
            System.IO.Stream imagestream = await this.Client.GetStreamAsync(imageUri);
            return ImageSource.FromStream(() => imagestream);
        }

        private void extractNameAndHLTBGameID(GameInfo gameInfo, HtmlNode tbodyNode)
        {
            var titleNode = tbodyNode.SelectNodes("tr/td/a")
                        .Where(node => !String.IsNullOrWhiteSpace(node.InnerText))
                        .FirstOrDefault();

            if (titleNode != default(HtmlNode))
            {
                gameInfo.Name = titleNode.InnerText?.Trim();
                gameInfo.HLTBGameID = titleNode.Attributes["href"]?.Value.Split('=').Last();
            }
        }

        private void extractEditUrl(GameInfo gameInfo, HtmlNode tbodyNode)
        {
            var editNode = tbodyNode.SelectNodes("tr/td/a[@title='Edit']").FirstOrDefault();
            if (editNode != default(HtmlNode))
            {
                gameInfo.EditUrl = editNode.Attributes["href"]?.Value;
            }
        }

        private async Task loadGameInfoDetails(GameInfo gameInfo)
        {
            var response = await this.Client.GetAsync("https://howlongtobeat.com/game?id=" + gameInfo.HLTBGameID);
            var content = await response.Content.ReadAsStringAsync();

            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            loadGameImage(gameInfo, doc);

        }

        private bool loadGameImage(GameInfo gameInfo, HtmlDocument doc)
        {
            var gameImgUrl = doc.DocumentNode
                .SelectNodes("//div[contains(@class, 'game_image')]/img")
                .Select(img => img.Attributes.Where(attr => attr.Name.ToLower().Contains("src")).FirstOrDefault()?.Value ?? String.Empty)
                .FirstOrDefault();

            if (!String.IsNullOrEmpty(gameImgUrl))
            {
                gameInfo.HLTBImageSourceUri = new Uri(gameImgUrl);

                return true;
            }

            return false;
        }
    }
}
