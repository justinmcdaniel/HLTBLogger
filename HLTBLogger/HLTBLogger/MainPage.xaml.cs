using HLTBLogger.ViewModel;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace HLTBLogger
{
    public partial class MainPage : ContentPage
    {
        public IEnumerable<GameInfoModel> CurrentGames { get; private set; }

        public MainPage(string username)
        {
            InitializeComponent();

            initializeGamesList(username);

            BindingContext = this;
        }

        private void initializeGamesList(string username)
        {
            var client = new HttpClient();
            var data = new Dictionary<string, string>();
            data["n"] = username;
            data["playing"] = "1";

            var body = new System.Net.Http.FormUrlEncodedContent(data);

            var response = client.PostAsync("https://howlongtobeat.com/user_games_list", body).Result;
            var content = response.Content.ReadAsStringAsync().Result;

            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            CurrentGames = doc.DocumentNode.SelectNodes("//table[@class='user_game_list']//a")
                .Where(node => !String.IsNullOrWhiteSpace(node.InnerText))
                .Select(node =>
                    new GameInfoModel()
                    {
                        Name = node.InnerText?.Trim(),
                        HLTBGameID = node.Attributes["href"]?.Value.Split('=').Last()
                    });
        }
    }
}
