using HtmlAgilityPack;
using System;
using System.Net;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HLTBLogger
{
    public partial class App : Application
    {
        public static string SECURE_STORAGE_USERNAME_KEY = "HLTP_USERNAME";
        public static string SECURE_STORAGE_PASSWORD_KEY = "HLTB_PASSWORD";
        public string HLTBUsername
        {
            get => SecureStorage.GetAsync(SECURE_STORAGE_USERNAME_KEY).Result;
            set => SecureStorage.SetAsync(SECURE_STORAGE_USERNAME_KEY, value);
        }
        public string HLTBPassword
        {
            get => SecureStorage.GetAsync(SECURE_STORAGE_PASSWORD_KEY).Result;
            set => SecureStorage.SetAsync(SECURE_STORAGE_PASSWORD_KEY, value);
        }

        public HttpUtility HLTBClient { get; private set; }

        public App()
        {
            InitializeComponent();

            HLTBClient = new HttpUtility();

            if (String.IsNullOrEmpty(HLTBUsername) || String.IsNullOrEmpty(HLTBPassword))
            {
                MainPage = new NavigationPage(new LoginPage());
            }
            else
            {
                MainPage = new NavigationPage(new MainPage());
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
