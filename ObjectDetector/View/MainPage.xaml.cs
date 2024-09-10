using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using ObjectDetector.ViewModel;

namespace ObjectDetector
{
    public partial class MainPage : ContentPage
    {
        private readonly MainPageViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();
            _viewModel = new MainPageViewModel(MyCamera);
            BindingContext = _viewModel;

            // MyCamera.MediaCaptured += MyCamera_MediaCaptured;
        }

        //protected override void OnNavigatedTo(NavigatedToEventArgs args)
        //{
        //    base.OnNavigatedTo(args);
        //    
        //}

        protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
        {
            base.OnNavigatedFrom(args);
            MyCamera.MediaCaptured -= MyCamera_MediaCaptured;
        }

        private async void MyCamera_MediaCaptured(object? sender, CommunityToolkit.Maui.Views.MediaCapturedEventArgs args)
        {
            await _viewModel.HandleMediaCaptured(args);
        }
    }
}
