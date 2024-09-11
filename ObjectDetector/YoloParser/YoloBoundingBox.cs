using System.Drawing;
using Color = System.Drawing.Color;

namespace ObjectDetector.YoloParser
{
    public class BoundingBoxDimensions : DimensionsBase { }

    public class YoloBoundingBox
    {
        public required BoundingBoxDimensions Dimensions { get; set; }

        public string? Label { get; set; }

        public float Confidence { get; set; }

        public RectangleF Rect
        {
            get { return new RectangleF(Dimensions.X, Dimensions.Y, Dimensions.Width, Dimensions.Height); }
        }

        public Color BoxColor { get; set; }

    }
}
