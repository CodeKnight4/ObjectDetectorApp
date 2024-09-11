using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using ObjectDetector.Services;
using ObjectDetector.View;
using ObjectDetector.YoloParser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDetector.ViewModel
{
    public partial class DetectionViewModel : BaseViewModel
    {
        //private IList<YoloBoundingBox> _boundingBoxes;

        //[ObservableProperty]
        //private IDrawable drawable;

        //public DetectionViewModel()
        //{
        //    _boundingBoxes = [];
        //    Drawable = new EmptyDrawable();
        //}

        //public async Task DetectObjects(Stream imageStream)
        //{
        //    try
        //    {
        //        Trace.WriteLine($"Detecting objects...");
        //        var detectionService = new ObjectDetectionService(imageStream);
        //        await detectionService.InitializeAsync();
        //        _boundingBoxes = detectionService.ListOfBoxes;
        //        Drawable = new BoundingBoxesDrawable(_boundingBoxes);

        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.WriteLine($"Failed to detect objects: {ex.Message}");
        //    }

        //    await Task.Delay(1000);
        //}

    }
}
