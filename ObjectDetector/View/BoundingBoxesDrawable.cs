using ObjectDetector.YoloParser;
using System.Globalization;

namespace ObjectDetector.View
{
    public class BoundingBoxesDrawable(IList<YoloBoundingBox> boundingBoxes) : IDrawable
    {
        private readonly IList<YoloBoundingBox> _boundingBoxes = boundingBoxes;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            foreach (var box in _boundingBoxes)
            {
                var rect = new RectF(box.Dimensions.X, box.Dimensions.Y, box.Dimensions.Width, box.Dimensions.Height);
                string text = $"{new CultureInfo("en-GB").TextInfo.ToTitleCase(box.Label)} ({box.Confidence * 100:0}%)";

                // Draw BoundingBox
                canvas.StrokeColor = ColorConverter.ConvertToMauiColor(box.BoxColor);
                canvas.StrokeSize = 2;
                canvas.DrawRectangle(rect);

                // Draw Text Background
                var font = new Microsoft.Maui.Graphics.Font("Arial");
                canvas.Font = font;
                canvas.FillColor = Colors.Black;
                var textWidth = canvas.GetStringSize(text, font, 12).Width;
                var textHeight = canvas.GetStringSize(text, font, 12).Height;
                var textBackgroundRect = new RectF(box.Dimensions.X, box.Dimensions.Y - textHeight - 5, textWidth + 5, textHeight + 5);
                canvas.FillRectangle(textBackgroundRect);

                // Draw Text
                canvas.FontColor = Colors.White;
                canvas.FontSize = 12;
                canvas.DrawString(text, box.Dimensions.X + 2, box.Dimensions.Y - textHeight + 8, HorizontalAlignment.Left);

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
