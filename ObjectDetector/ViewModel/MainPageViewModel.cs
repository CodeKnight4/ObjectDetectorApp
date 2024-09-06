using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace ObjectDetector.ViewModel
{
    public partial class MainPageViewModel : ObservableObject
    {
        private Stream? _capturedImageStream;
        private bool isCameraEnabled = false;

        [ObservableProperty]
        private bool isCameraVisible;

        [ObservableProperty]
        private bool isDetectButtonVisible;

        [ObservableProperty]
        private string cameraImageSource;

        public ICommand CameraCommand { get; }
        public ICommand DetectCommand { get; }

        public MainPageViewModel()
        {
            CameraCommand = new AsyncRelayCommand(OnCameraClicked);
            DetectCommand = new AsyncRelayCommand(DetectBtnClicked);

            isCameraVisible = false;
            isDetectButtonVisible = false;
            cameraImageSource = "video_solid.png";
        }

        private async Task OnCameraClicked()
        {
            isCameraEnabled = !isCameraEnabled;

            if (isCameraEnabled)
            {
                CameraImageSource = "video_slash_solid.png";
                IsCameraVisible = true;
                IsDetectButtonVisible = true;
            }
            else
            {
                CameraImageSource = "video_solid.png";
                IsCameraVisible = false;
                IsDetectButtonVisible = false;
            }

            await Task.Delay(1000); // Give a second to the program to setup the device camera
        }

        private async Task DetectBtnClicked()
        {
            // Need to add functionality next
        }

        public void HandleMediaCaptured(Stream mediaStream)
        {
            _capturedImageStream = mediaStream;
            Console.WriteLine(_capturedImageStream);
        }
    }
}

