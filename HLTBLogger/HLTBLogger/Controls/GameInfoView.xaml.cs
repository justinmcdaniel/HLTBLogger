using HLTBLogger.ViewModel;
using System;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HLTBLogger.Controls
{
    public enum GameLogState
    {
        Normal,
        Recording,
        PendingSubmit
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GameInfoView : ContentView
    {

        private Stopwatch stopwatch;
        private Timer stopwatch_UpdateUITimer;

        public GameInfoView()
        {
            InitializeComponent();

            stopwatch = new Stopwatch();
            stopwatch_UpdateUITimer = new Timer(500)
            {
                AutoReset = true,
                Enabled = false
            };
            stopwatch_UpdateUITimer.Elapsed += Stopwatch_UpdateUITimer_Elapsed;
        }

        public static readonly BindableProperty GameInfoProperty = BindableProperty.Create(
            "GameInfo",        // the name of the bindable property
            typeof(GameInfo),     // the bindable property type
            typeof(GameInfoView),   // the parent object type
            GameInfo.Empty);

        public GameInfo GameInfo
        {
            get => (GameInfo)GetValue(GameInfoProperty);
            set => SetValue(GameInfoProperty, value);
        }

        private void resetForm()
        {
            stopwatch.Reset();
            Stopwatch_UpdateUITimer_Elapsed(this, null);

            BtnStart.IsVisible = true;
            FrmTiming.IsVisible = false;
            FrmSubmit.IsVisible = false;
        }

        private void BtnStart_Clicked(object sender, EventArgs e)
        {
            BtnStart.IsVisible = false;
            FrmTiming.IsVisible = true;

            stopwatch.Start();
            stopwatch_UpdateUITimer.Enabled = true;
        }

        private void BtnStop_Clicked(object sender, EventArgs e)
        {
            stopwatch.Stop();

            FrmTiming.IsVisible = false;
            FrmSubmit.IsVisible = true;

            stopwatch_UpdateUITimer.Enabled = false;

        }

        private void BtnReset_Clicked(object sender, EventArgs e)
        {
            resetForm();
        }

        private void BtnSubmit_Clicked(object sender, EventArgs e)
        {
            // TODO: Submit time
            
            resetForm();
        }

        private void Stopwatch_UpdateUITimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() => LblTimer.Text = Convert.ToString(stopwatch.Elapsed));
        }
    }
}