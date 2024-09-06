using CommunityToolkit.Maui.Core;
using ObjectDetector.ViewModel;

namespace ObjectDetector
{
    public partial class MainPage : ContentPage
    {
        private readonly MainPageViewModel _viewModel;

        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;

            MyCamera.MediaCaptured += MyCamera_MediaCaptured;
        }

        protected async override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            // Perform any additional setup after navigation if needed
        }

        protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
        {
            base.OnNavigatedFrom(args);
            MyCamera.MediaCaptured -= MyCamera_MediaCaptured;
        }

        private void MyCamera_MediaCaptured(object? sender, CommunityToolkit.Maui.Views.MediaCapturedEventArgs args)
        {
            _viewModel.HandleMediaCaptured(args.Media);
        }
    }
}
