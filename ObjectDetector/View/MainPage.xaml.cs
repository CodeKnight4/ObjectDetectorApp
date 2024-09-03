using CommunityToolkit.Maui.Camera;
namespace ObjectDetector
{
    public partial class MainPage : ContentPage
    {
        bool isCameraEnabled = false;

        public MainPage()
        {
            InitializeComponent();
            CameraWindow.IsVisible = false;
            DetectBtn.IsVisible = false;
        }

        private async void OnCameraClicked(object sender, EventArgs e)
        {
            CameraBtn.IsEnabled = false;

            await ToggleCameraBtn();

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

            }

            await Task.Delay(1000); // Give a second to the program to setup the device camera
        }
    }

}
