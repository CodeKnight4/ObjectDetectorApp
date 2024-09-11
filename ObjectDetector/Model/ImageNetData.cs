using Microsoft.ML.Data;

namespace ObjectDetector.Model
{
    public class ImageNetData
    {
        [LoadColumn(0)]
        public string? ImagePath { get; set; }

        [LoadColumn(1)]
        public string? Label { get; set; }

        public static IEnumerable<ImageNetData> ReadFromFile(string imageFolder)
        {
            return Directory
                .GetFiles(imageFolder)
                .Where(filePath => Path.GetExtension(filePath) != ".md")
                .Select(filePath => new ImageNetData { ImagePath = filePath, Label = Path.GetFileName(filePath) });
        }
    }
}
