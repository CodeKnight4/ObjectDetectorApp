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
        private IReadOnlyList<CameraInfo> availableCameras;

        [ObservableProperty]
        private IDrawable drawable;

        [ObservableProperty]
        private double currentZoom;

        [ObservableProperty]
        private bool isCameraVisible;

        [ObservableProperty]
        private string cameraImageSource;

        private CameraInfo? _selectedCamera;
        public CameraInfo? SelectedCamera
        {
            get { return _selectedCamera; }
            set
            {
                SetProperty(ref _selectedCamera, value);
                UpdateCameraView(); // Whenever selected camera is changed this will run
            }
        }

        public ICommand CameraCommand { get; }
        public ICommand CaptureImageCommand { get; }  

        public bool IsCameraNotVisible => !IsCameraVisible;

        public MainPageViewModel(CameraView cameraView)
        {
            CameraCommand = new AsyncRelayCommand(OnCameraClicked);
            CaptureImageCommand = new AsyncRelayCommand(CaptureImage);
            _cameraView = cameraView;
            _boundingBoxes = [];
            AvailableCameras = [];
            Drawable = new EmptyDrawable();
            IsCameraVisible = false;
            CameraImageSource = "video_solid.png";
        }

        private void UpdateCameraView()
        {
            if (SelectedCamera != null)
            {
                _cameraView.SelectedCamera = SelectedCamera;
            }
        }

        private async void UpdateCameraList()
        {
            try
            {
                var cameraList = await _cameraView.GetAvailableCameras(CancellationToken.None);
                AvailableCameras = cameraList;
                SelectedCamera = AvailableCameras[0];
                
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Can't find any available cameras: {ex}");
            }
        }

        private async Task CaptureImage()
        {
            try
            {
                await _cameraView.CaptureImage(CancellationToken.None);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"There were errors: {ex}");
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
                    UpdateCameraList();
                    CameraImageSource = "video_slash_solid.png";
                }
                else
                {
                    _cameraView.StopCameraPreview();
                    CameraImageSource = "video_solid.png";
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
                // Trace.WriteLine($"Image saved to variable");
                var detectionService = new ObjectDetectionService(imageStream.Media);
                await detectionService.InitializeAsync();
                _boundingBoxes = detectionService.ListOfBoxes;
                Drawable = _boundingBoxes.Any() ? new BoundingBoxesDrawable(_boundingBoxes) : new EmptyDrawable();       
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Failed to save image: {ex.Message}");
            }
        }
    }
}

