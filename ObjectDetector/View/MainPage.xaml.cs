using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Dispatching;
using ObjectDetector.ViewModel;
using System.Threading;
using System.Timers;

namespace ObjectDetector
{
    public partial class MainPage : ContentPage
    {
        private readonly MainPageViewModel _viewModel;
        private CancellationTokenSource? _cancellationTokenSource;

        public MainPage()
        {
            InitializeComponent();
            _viewModel = new MainPageViewModel(MyCamera);
            BindingContext = _viewModel;

            // MyCamera.MediaCaptured += MyCamera_MediaCaptured;
        }

        //protected async override void OnNavigatedTo(NavigatedToEventArgs args)
        //{
        //    base.OnNavigatedTo(args);

        //}

        protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
        {
            base.OnNavigatedFrom(args);
            MyCamera.MediaCaptured -= MyCamera_MediaCaptured;
        }

        private async void MyCamera_MediaCaptured(object? sender, MediaCapturedEventArgs args)
        {
            try 
            { 
                await _viewModel.HandleMediaCaptured(args); 
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Media wasn't able to be captured: {ex}");
            }

        }

        private void DetectSwitchToggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                // Start continuous detection
                _cancellationTokenSource = new CancellationTokenSource();
                StartContinuousCapture(_cancellationTokenSource.Token);
            }
            else
            {
                // Stop continuous detection
                _cancellationTokenSource?.Cancel();
            }
        }

        private async void StartContinuousCapture(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _viewModel.CaptureImageCommand.Execute(null);
                await Task.Delay(1000); // Delay to avoid rapid looping, adjust as needed
            }
        }
    }
}
