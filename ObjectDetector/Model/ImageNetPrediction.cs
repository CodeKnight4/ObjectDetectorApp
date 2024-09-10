using Microsoft.ML.Data;

namespace ObjectDetector.Model
{
    public class ImageNetPrediction
    {
        [ColumnName("grid")]
        public float[]? PredictedLabels;
    }
}
