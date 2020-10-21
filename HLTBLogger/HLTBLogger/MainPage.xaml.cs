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
        public MainPage()
        {
            InitializeComponent();

            initializeGamesList();
        }

        private async void initializeGamesList()
        {
            var HLTBClient = (App.Current as HLTBLogger.App).HLTBClient;

            if (!HLTBClient.IsLoggedIn)
            {
                await HLTBClient.Login();

                if (!HLTBClient.IsLoggedIn)
                {
                    Navigation.InsertPageBefore(new LoginPage(true), this);
                    await Navigation.PopAsync();
                }
            }

            listGames.ItemsSource = await HLTBClient.GetCurrentGames();    
        }

    }
}
