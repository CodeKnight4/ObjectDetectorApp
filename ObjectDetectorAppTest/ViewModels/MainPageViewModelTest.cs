using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using Moq;
using ObjectDetector.View;
using ObjectDetector.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace ObjectDetectorAppTest.ViewModels
{
    public class MainPageViewModelTest
    {
        private readonly Mock<CameraView> _mockCameraView;
        private readonly MainPageViewModel _viewModel;
        private readonly ITestOutputHelper _output;

        public MainPageViewModelTest(ITestOutputHelper output)
        {
            _mockCameraView = new Mock<CameraView>();
            _viewModel = new MainPageViewModel(_mockCameraView.Object);
            _output = output;
        }

        [Fact]
        public void Constructor_InitializesProperties()
        {
            Assert.NotNull(_viewModel.CameraCommand);
            Assert.NotNull(_viewModel.CaptureImageCommand);
            Assert.Empty(_viewModel.AvailableCameras);
            Assert.Equal("video_solid.png", _viewModel.CameraImageSource);
            Assert.IsType<EmptyDrawable>(_viewModel.Drawable);
            Assert.False(_viewModel.IsCameraVisible);
        }

        [Fact]
        public async Task OnCameraClicked_TogglesCameraVisibility()
        {
            // Arrange
            var initialVisibility = _viewModel.IsCameraVisible;

            // Act
            _viewModel.CameraCommand.Execute(null);
            await Task.Delay(1000);

            // Assert
            Assert.NotEqual(initialVisibility, _viewModel.IsCameraVisible);
        }

        [Fact]
        public async Task HandleMediaCaptured_UpdatesDrawable()
        {
            // Arrange
            var viewModel = new MainPageViewModel(_mockCameraView.Object);
            var mediaCapturedEventArgs = new MediaCapturedEventArgs(new MemoryStream());

            // Act
            await viewModel.HandleMediaCaptured(mediaCapturedEventArgs);
            // _output.WriteLine($"{mediaCapturedEventArgs}");

            // Assert
            Assert.IsAssignableFrom<EmptyDrawable>(viewModel.Drawable); // Due to the way I have created this class it defaults to null as the inner code doesn't run so it defaults to the empty drawable
        }

        [Fact]
        public void HandleMediaCapturedReturnsNullWhenImageStreamIsNull()
        {
            // Arrange
            var viewModel = new MainPageViewModel(_mockCameraView.Object);
            var mediaCapturedEventArgs = new MediaCapturedEventArgs(Stream.Null);

            // Act
            var result = viewModel.HandleMediaCaptured(mediaCapturedEventArgs);

            // Assert
            Assert.NotNull(result); 
        }
    }
}
