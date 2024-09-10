using ObjectDetector.YoloParser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDetector.View
{
    public class BoundingBoxesDrawable(IList<YoloBoundingBox>? boundingBoxes) : IDrawable
    {
        private readonly IList<YoloBoundingBox>? _boundingBoxes = boundingBoxes;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            foreach (var box in _boundingBoxes)
            {
                var rect = new RectF(box.Dimensions.X, box.Dimensions.Y, box.Dimensions.Width, box.Dimensions.Height);
                // string text = $"{box.Label} ({box.Confidence * 100:0}%)";

                canvas.StrokeColor = ColorConverter.ConvertToMauiColor(box.BoxColor);
                canvas.StrokeSize = 2;
                canvas.DrawRectangle(rect);
      
            }
        }
    }

    public static class ColorConverter
    {
        public static Microsoft.Maui.Graphics.Color ConvertToMauiColor(System.Drawing.Color color)
        {
            return Microsoft.Maui.Graphics.Color.FromRgba(color.R, color.G, color.B, color.A);
        }
    }
}
