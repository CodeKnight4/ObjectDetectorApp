using CommunityToolkit.Maui.Core;

namespace ObjectDetector
{
    public partial class MainPage : ContentPage
    {
        private bool isCameraEnabled = false;
        private readonly ICameraProvider _cameraProvider;
        private Stream? _capturedImageStream; 

        public MainPage(ICameraProvider cameraProvider)
        {
            InitializeComponent();
            CameraWindow.IsVisible = false;
            DetectBtn.IsVisible = false;
            this._cameraProvider = cameraProvider;
        }

        protected async override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            try
            {
                await _cameraProvider.RefreshAvailableCameras(CancellationToken.None);
                MyCamera.SelectedCamera = _cameraProvider.AvailableCameras.Where(c => c.Position == CommunityToolkit.Maui.Core.Primitives.CameraPosition.Front).FirstOrDefault();
            }
            catch (ArgumentException e)
            {
                Console.WriteLine($"Refresh failed: {e.Message}");
            };
        }

        protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
        {
            base.OnNavigatedFrom(args);

            MyCamera.MediaCaptured -= MyCamera_MediaCaptured;
            MyCamera.Handler?.DisconnectHandler();
        }

        private void MyCamera_MediaCaptured(object? sender, CommunityToolkit.Maui.Views.MediaCapturedEventArgs args)
        {
            _capturedImageStream = args.Media;
            Console.WriteLine(_capturedImageStream);
            //if (Dispatcher.IsDispatchRequired)
            //{
            //    Dispatcher.Dispatch(() => MyImage.Source = ImageSource.FromStream(() => args.Media));
            //    return;
            //}

            //MyImage.Source = ImageSource.FromStream(() => args.Media);
        }

        private async void OnCameraClicked(object sender, EventArgs e)
        {
            CameraBtn.IsEnabled = false;

            try 
            { 
                await ToggleCameraBtn(); 
            }

            catch (ArgumentException b)
            {
                Console.WriteLine($"Camera Loading failed: {b.Message}");
            };

            CameraBtn.IsEnabled = true;
        }

        private async Task ToggleCameraBtn()
        {
            isCameraEnabled = !isCameraEnabled;

            if (isCameraEnabled)
            {
                CameraBtn.ImageSource = "video_slash_solid.png";
                CameraWindow.IsVisible = true;
                DetectBtn.IsVisible= true;
            }
            else
            {
                CameraBtn.ImageSource = "video_solid.png";
                CameraWindow.IsVisible = false;
                DetectBtn.IsVisible = false;
                //MyImage.Source = null;

            }

            await Task.Delay(1000); // Give a second to the program to setup the device camera
        }

        private async void DetectBtnClicked(object sender, EventArgs e) 
        {
            try
            {
                await MyCamera.CaptureImage(CancellationToken.None);
            }

            catch (ArgumentException b)
            {
                Console.WriteLine($"Image capture failed: {b.Message}");
            };
        }
    }

}
