using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HLTBLogger
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage(bool showError = false)
        {
            InitializeComponent();

            ErrMsg.IsVisible = showError;
        }

        async private void OnLoginButtonClicked(object sender, EventArgs e)
        {
            var currentApp = App.Current as HLTBLogger.App;

            SetErrorState(false);
            BtnLogin.IsEnabled = false;

            var username = Username.Text.Trim();
            var password = Password.Text.Trim();

            if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
            {
                currentApp.HLTBUsername = username;
                currentApp.HLTBPassword = password;

                var result = await currentApp.HLTBClient.Login();
                if (result)
                {
                    Navigation.InsertPageBefore(new MainPage(), this);
                    await Navigation.PopAsync();
                }
                else
                {
                    SetErrorState(true);
                    BtnLogin.IsEnabled = true;
                }
            }
            else
            {
                SetErrorState(true);
                BtnLogin.IsEnabled = true;
            }

        }

        private void SetErrorState(bool state)
        {
            ErrMsg.IsVisible = state;
        }


    }
}