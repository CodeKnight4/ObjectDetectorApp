using System.Diagnostics;
using Microsoft.Maui.Primitives;
using Microsoft.ML;
using ObjectDetector.Model;
using ObjectDetector.YoloParser;
using SkiaSharp;
using Color = System.Drawing.Color;
using Path = System.IO.Path;

namespace ObjectDetector.Services
{
    public class ObjectDetectionService
    {
        private readonly Stream _imageStream;
        private readonly string imagesFolder;
        private readonly string outputFolder;
        private readonly string modelFolder;
        private string modelFilePath;
        public MLContext? MlContext { get; private set; }
        public IList<YoloBoundingBox> ListOfBoxes { get; private set; }

        public ObjectDetectionService(Stream imageStream)
        {
            _imageStream = imageStream;
            imagesFolder = Path.Combine(FileSystem.AppDataDirectory, "Images");
            modelFolder = Path.Combine(FileSystem.AppDataDirectory, "Models");
            outputFolder = Path.Combine(imagesFolder, "Output");
            modelFilePath = Path.Combine(modelFolder, "TinyYolo2_model.onnx");
            ListOfBoxes = [];

            ObjectDetectionService.CreateDirectoryIfNotExists(imagesFolder);
            ObjectDetectionService.CreateDirectoryIfNotExists(outputFolder);
            ObjectDetectionService.CreateDirectoryIfNotExists(modelFolder);

        }

        private static void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Trace.WriteLine($"Creating folder: {path}");
                Directory.CreateDirectory(path);
            }
        }

        public async Task SetUpFilePaths()
        {
            var modelStream = await ObjectDetectionService.LoadModelFileAsync("Models/TinyYolo2_model.onnx");
            if (_imageStream == null)
            {
                throw new ArgumentNullException(nameof(_imageStream), "Image stream cannot be null");
            }

            modelFilePath = await ObjectDetectionService.SaveStreamToTemporaryFile(modelStream, modelFilePath);

            Trace.WriteLine("Hi there");
        }

        public async Task InitializeAsync()
        {
            MlContext = new MLContext();

            try
            {
                if (!File.Exists(modelFilePath))
                {
                    await SetUpFilePaths();
                }
                
                var imagePath = await ObjectDetectionService.SaveStreamToTemporaryFile(_imageStream, Path.Combine(imagesFolder,"CapturedImg.png"));
                ImageNetData imageNetData = new()
                {
                    ImagePath = imagePath, // Newly created image path
                    Label = "Test"
                };

                IDataView imageDataView = MlContext.Data.LoadFromEnumerable(new[] { imageNetData });

                // Create instance of model scorer
                var modelScorer = new OnnxModelScorer(imagesFolder, modelFilePath, MlContext);
                // Trace.WriteLine("Step 1");

                // Use model to score data
                IEnumerable<float[]> probabilities = modelScorer.Score(imageDataView);
                // Trace.WriteLine("Step 2");

                YoloOutputParser parser = new();

                var boundingBoxes =
                    probabilities
                    .Select(probability => parser.ParseOutputs(probability))
                    .Select(boxes => YoloOutputParser.FilterBoundingBoxes(boxes, 5, .5F));

                // Trace.WriteLine("Step 3");
                string imageFileName = "ProcessedImage.png";
                IList<YoloBoundingBox> detectedObjects = boundingBoxes.ElementAt(0);
                // Trace.WriteLine("Step 4");
                DrawBoundingBox(detectedObjects);
                // Trace.WriteLine("Step 5");
                ObjectDetectionService.LogDetectedObjects(imageFileName, detectedObjects);
                // Trace.WriteLine("We have finished");

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                Trace.WriteLine("We have failed");
            }

            Trace.WriteLine("========= End of Process..Hit any Key ========");
        }

        public void DrawBoundingBox(IList<YoloBoundingBox> filteredBoundingBoxes)
        {
            var imagePath = Path.Combine(imagesFolder, "CapturedImg.png");
            using var inputStream = File.OpenRead(imagePath);
            using var skBitmap = SKBitmap.Decode(inputStream);
            var originalImageHeight = skBitmap.Height;
            var originalImageWidth = skBitmap.Width;

            using var skCanvas = new SKCanvas(skBitmap);
            foreach (var box in filteredBoundingBoxes)
            {
                var x = (uint)Math.Max(box.Dimensions.X, 0);
                var y = (uint)Math.Max(box.Dimensions.Y, 0);
                var width = (uint)Math.Min(originalImageWidth - x, box.Dimensions.Width);
                var height = (uint)Math.Min(originalImageHeight - y, box.Dimensions.Height);

                x = (uint)originalImageWidth * x / OnnxModelScorer.ImageNetSettings.imageWidth;
                y = (uint)originalImageHeight * y / OnnxModelScorer.ImageNetSettings.imageHeight;
                width = (uint)originalImageWidth * width / OnnxModelScorer.ImageNetSettings.imageWidth;
                height = (uint)originalImageHeight * height / OnnxModelScorer.ImageNetSettings.imageHeight;

                // Added updated box lengths to new list to use outside
                ListOfBoxes.Add(new YoloBoundingBox()
                {
                    Dimensions = new BoundingBoxDimensions
                    {
                        X = x,
                        Y = y,
                        Width = width,
                        Height = height,
                    },
                    Confidence = box.Confidence,
                    Label = box.Label,
                    BoxColor = box.BoxColor
                });
                string text = $"{box.Label} ({box.Confidence * 100:0}%)";

                // Draw Bounding Box
                using var paint = new SKPaint
                {
                    Color = box.BoxColor.ToSKColor(),
                    IsStroke = true,
                    StrokeWidth = 3.2f,
                    Style = SKPaintStyle.Stroke,
                    IsAntialias = true,
                };
                skCanvas.DrawRect(x, y, width, height, paint);

                // Draw Text Background
                paint.IsStroke = false;
                using var textPaint = new SKPaint
                {
                    Color = SKColors.Black,
                    Typeface = SKTypeface.FromFamilyName("Arial"),
                    TextSize = 12,
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                };
                var textWidth = textPaint.MeasureText(text);
                var textHeight = textPaint.FontMetrics.CapHeight;
                skCanvas.DrawRect(new SKRect(x, y - textHeight - 5, x + textWidth + 5, y), paint);

                // Draw Text
                paint.Color = SKColors.White;
                skCanvas.DrawText(text, x + 2, y - 2, textPaint);
            }

            var outputPath = Path.Combine(outputFolder, "ProcessedImg.png");
            using var outputStream = File.OpenWrite(outputPath);
            skBitmap.Encode(outputStream, SKEncodedImageFormat.Png, 100);
        }

        // The code below only works for windows, may return back to it if SkiaSharp doesn't work out

        //void DrawBoundingBox(IList<YoloBoundingBox> filteredBoundingBoxes)
        //{
        //    Image image = Image.FromFile(Path.Combine(imagesFolder, "CapturedImg.png"));
        //    var originalImageHeight = image.Height;
        //    var originalImageWidth = image.Width;

        //    foreach (var box in filteredBoundingBoxes)
        //    {
        //        var x = (uint)Math.Max(box.Dimensions.X, 0);
        //        var y = (uint)Math.Max(box.Dimensions.Y, 0);
        //        var width = (uint)Math.Min(originalImageWidth - x, box.Dimensions.Width);
        //        var height = (uint)Math.Min(originalImageHeight - y, box.Dimensions.Height);

        //        x = (uint)originalImageWidth * x / OnnxModelScorer.ImageNetSettings.imageWidth;
        //        y = (uint)originalImageHeight * y / OnnxModelScorer.ImageNetSettings.imageHeight;
        //        width = (uint)originalImageWidth * width / OnnxModelScorer.ImageNetSettings.imageWidth;
        //        height = (uint)originalImageHeight * height / OnnxModelScorer.ImageNetSettings.imageHeight;

        //        string text = $"{box.Label} ({box.Confidence * 100:0}%)";

        //        using Graphics thumbnailGraphic = Graphics.FromImage(image);
        //        thumbnailGraphic.CompositingQuality = CompositingQuality.HighQuality;
        //        thumbnailGraphic.SmoothingMode = SmoothingMode.HighQuality;
        //        thumbnailGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;

        //        // Define Text Options
        //        Font drawFont = new("Arial", 12, FontStyle.Bold);
        //        SizeF size = thumbnailGraphic.MeasureString(text, drawFont);
        //        SolidBrush fontBrush = new(Color.Black);
        //        Point atPoint = new((int)x, (int)y - (int)size.Height - 1);

        //        // Define BoundingBox options
        //        Pen pen = new(box.BoxColor, 3.2f);
        //        SolidBrush colorBrush = new(box.BoxColor);

        //        thumbnailGraphic.FillRectangle(colorBrush, (int)x, (int)(y - size.Height - 1), (int)size.Width, (int)size.Height);

        //        thumbnailGraphic.DrawString(text, drawFont, fontBrush, atPoint);

        //        // Draw bounding box on image
        //        thumbnailGraphic.DrawRectangle(pen, x, y, width, height);
        //    }
        //    image.Save(Path.Combine(outputFolder, "ProcessedImg.png"));
        //}

        static void LogDetectedObjects(string imageName, IList<YoloBoundingBox> boundingBoxes)
        {
            Trace.WriteLine($".....The objects in the image {imageName} are detected as below....");

            foreach (var box in boundingBoxes)
            {
                Trace.WriteLine($"{box.Label} and its Confidence score: {box.Confidence}");
            }

            Trace.WriteLine("");
        }

        public static async Task<Stream> LoadModelFileAsync(string fileName)
        {
            try
            {
                Stream fileStream = await FileSystem.OpenAppPackageFileAsync(fileName);
                return fileStream;
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                throw new FileNotFoundException($"Unable to find file: {fileName}", ex);
            }
        }

        private static async Task<string> SaveStreamToTemporaryFile(Stream stream, string pathName)
        {
            try
            {
                stream.Position = 0;
                using (var fileStream = new FileStream(pathName, FileMode.Create, FileAccess.Write))
                {
                    await stream.CopyToAsync(fileStream);
                }
                return pathName;
            }
            
            catch (Exception ex)
            {
                throw new FileNotFoundException(ex.Message);
            }
        }
    }
}

public static class ColorExtensions
{
    public static SKColor ToSKColor(this Color color)
    {
        return new SKColor(color.R, color.G, color.B, color.A);
    }
}
