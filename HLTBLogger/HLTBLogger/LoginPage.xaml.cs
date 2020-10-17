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
        public LoginPage()
        {
            InitializeComponent();
        }

        async private void OnLoginButtonClicked(object sender, EventArgs e)
        {
            var username = Username.Text.Trim();

            Navigation.InsertPageBefore(new MainPage(username), this);
            await Navigation.PopAsync();
        }
    }
}