using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ObjectDetector.Services;
using ObjectDetector.View;
using ObjectDetector.YoloParser;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace ObjectDetector.ViewModel
{
    public partial class MainPageViewModel : BaseViewModel
    {
        private readonly CameraView _cameraView;
        private IList<YoloBoundingBox> _boundingBoxes;

        [ObservableProperty]
        private IDrawable drawable;

        [ObservableProperty]
        private ImageSource? capturedImageStream;

        [ObservableProperty]
        private double currentZoom;

        [ObservableProperty]
        private bool isCameraVisible;

        [ObservableProperty]
        private bool isDetectButtonVisible;

        [ObservableProperty]
        private bool isFrontCameraSelected;

        [ObservableProperty]
        private bool isRearCameraSelected;

        [ObservableProperty]
        private string cameraImageSource;

        public ICommand CameraCommand { get; }
        public ICommand CaptureImageCommand { get; }

        public bool IsCameraNotVisible => !IsCameraVisible;

        public MainPageViewModel(CameraView cameraView)
        {
            CameraCommand = new AsyncRelayCommand(OnCameraClicked);
            CaptureImageCommand = new AsyncRelayCommand(CaptureImage);
            _cameraView = cameraView;
            _boundingBoxes = [];
            Drawable = new EmptyDrawable();
            IsCameraVisible = false;
            IsDetectButtonVisible = false;
            CameraImageSource = "video_solid.png";
            CapturedImageStream = "dotnet_bot.png";
        }

        private async Task CaptureImage()
        {
            try
            {
                await _cameraView.CaptureImage(CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There were errors: {ex}");
            }
        }

        private async Task OnCameraClicked()
        {
            IsCameraVisible = !IsCameraVisible;
            OnPropertyChanged(nameof(IsCameraNotVisible));
            try
            {
                if (IsCameraVisible)
                {
                    await _cameraView.StartCameraPreview(CancellationToken.None);
                    CameraImageSource = "video_slash_solid.png";
                    IsDetectButtonVisible = true;
                    CapturedImageStream = "";

                }
                else
                {
                    _cameraView.StopCameraPreview();
                    CameraImageSource = "video_solid.png";
                    IsDetectButtonVisible = false;
                    CapturedImageStream = "";  // Will update to hold frontpage image
                    Drawable = new EmptyDrawable();
                }
            }

            catch (Exception ex)
            {
                Trace.WriteLine($"Failed to start camera window: {ex.Message}");
            }

            await Task.Delay(1000); // Give a second to the program to setup the device camera
        }

        public async Task HandleMediaCaptured(MediaCapturedEventArgs imageStream)
        {
            if (imageStream == null)
                return;

            try
            {
                Trace.WriteLine($"Image saved to variable");
                var detectionService = new ObjectDetectionService(imageStream.Media);
                await detectionService.InitializeAsync();
                _boundingBoxes = detectionService.ListOfBoxes;
                Drawable = new BoundingBoxesDrawable(_boundingBoxes);
                
                // CapturedImageStream = Path.Combine(FileSystem.AppDataDirectory, "Images/Output/ProcessedImg.png");
                // Troublesome line of code ^

            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Failed to save image: {ex.Message}");
            }

            await Task.Delay(3000);
        }
    }
}

