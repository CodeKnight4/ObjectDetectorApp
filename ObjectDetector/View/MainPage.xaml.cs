using CommunityToolkit.Maui.Camera;
namespace ObjectDetector
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCameraClicked(object sender, EventArgs e)
        {
            //count++;

            //if (count == 1)
            //    CameraBtn.Text = $"Clicked {count} time";
            //else
            //    CameraBtn.Text = $"Clicked {count} times";

            //SemanticScreenReader.Announce(CameraBtn.Text);
        }
    }

}
