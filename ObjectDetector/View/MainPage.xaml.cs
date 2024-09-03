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
            }
            else
            {
                CameraBtn.ImageSource = "video_solid.png";
                CameraWindow.IsVisible = false;

            }

            await Task.Delay(500); // Give a second to the program to setup the device camera
        }
    }

}
