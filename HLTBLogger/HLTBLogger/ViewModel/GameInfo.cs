using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;

namespace HLTBLogger.ViewModel
{
    public class GameInfo
    {
        public string Name { get; set; }
        public string HLTBGameID { get; set; }
        public Uri HLTBImageSourceUri { get; set; }

        public GameInfo()
        {
        }

        public override string ToString()
        {
            return Name;
        }

        private static readonly GameInfo empty = new GameInfo()
        {
            Name = "Loading..."
        };
        public static GameInfo Empty { get => empty; }
    }
}
