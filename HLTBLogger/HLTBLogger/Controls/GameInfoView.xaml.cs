using HLTBLogger.Utility;
using HLTBLogger.ViewModel;
using System;
using System.Timers;
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
        private HLTBLogger.App appRef;
        private StopWatch stopWatch = new StopWatch();

        private Timer stopwatch_UpdateUITimer = new Timer(500)
        {
            AutoReset = true,
            Enabled = false
        };

        public GameInfoView()
        {
            InitializeComponent();

            appRef = App.Current as HLTBLogger.App;
            stopwatch_UpdateUITimer.Elapsed += Stopwatch_UpdateUITimer_Elapsed;

            this.PropertyChanged += GameInfoView_PropertyChanged;
        }

        private async void GameInfoView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GameInfo))
            {
                if (GameInfo != null && GameInfo.HLTBImageSourceUri != null)
                {
                    var image = await appRef.HLTBClient.GetImageFromUri(GameInfo.HLTBImageSourceUri);
                    Device.BeginInvokeOnMainThread(() => ImgHLTBGameImage.Source = image);
                }
            }
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
            stopWatch.Reset();
            Stopwatch_UpdateUITimer_Elapsed(this, null);

            BtnStart.IsVisible = true;
            FrmTiming.IsVisible = false;
            FrmSubmit.IsVisible = false;
        }

        private void BtnStart_Clicked(object sender, EventArgs e)
        {
            BtnStart.IsVisible = false;
            FrmTiming.IsVisible = true;


            stopWatch.Start();
            stopwatch_UpdateUITimer.Enabled = true;
        }

        private void BtnStop_Clicked(object sender, EventArgs e)
        {
            stopWatch.Stop();

            FrmTiming.IsVisible = false;
            FrmSubmit.IsVisible = true;

            stopwatch_UpdateUITimer.Enabled = false;

        }

        private void BtnReset_Clicked(object sender, EventArgs e)
        {
            resetForm();
        }

        private async void BtnSubmit_Clicked(object sender, EventArgs e)
        {
            var HLTBClient = appRef.HLTBClient;
            await HLTBClient.SubmitTime(this.GameInfo, stopWatch.Elapsed);

            resetForm();
        }

        private void Stopwatch_UpdateUITimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() => LblTimer.Text = stopWatch.Elapsed.ToString(@"hh\:mm\:ss"));
        }
    }
}