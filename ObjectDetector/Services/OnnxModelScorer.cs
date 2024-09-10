using System.Diagnostics;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Onnx;
using ObjectDetector.Model;

namespace ObjectDetector.Services
{
    public class OnnxModelScorer(string imagesFolder, string modelLocation, MLContext mlContext)
    {
        private readonly string imagesFolder = imagesFolder;
        private readonly string modelLocation = modelLocation;
        private readonly MLContext mlContext = mlContext;

        public struct ImageNetSettings
        {
            public const int imageHeight = 416;
            public const int imageWidth = 416;
        }

        public struct TinyYoloModelSettings
        {
            // for checking Tiny yolo2 Model input and  output  parameter names,
            //you can use tools like Netron, 
            // which is installed by Visual Studio AI Tools

            // input tensor name
            public const string ModelInput = "image";

            // output tensor name
            public const string ModelOutput = "grid";
        }

        private TransformerChain<OnnxTransformer> LoadModel(string modelLocation)
        {
            Trace.WriteLine("Read model");
            Trace.WriteLine($"Model location: {modelLocation}");
            Trace.WriteLine($"Default parameters: image size=({ImageNetSettings.imageWidth},{ImageNetSettings.imageHeight})");

            var data = mlContext.Data.LoadFromEnumerable(new List<ImageNetData>());

            var pipeline = mlContext.Transforms.LoadImages(outputColumnName: "image", imageFolder: "", inputColumnName: nameof(ImageNetData.ImagePath))
                .Append(mlContext.Transforms.ResizeImages(outputColumnName: "image", imageWidth: ImageNetSettings.imageWidth, imageHeight: ImageNetSettings.imageHeight, inputColumnName: "image"))
                .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "image"))
                .Append(mlContext.Transforms.ApplyOnnxModel(modelFile: modelLocation, outputColumnNames: [TinyYoloModelSettings.ModelOutput], inputColumnNames: [TinyYoloModelSettings.ModelInput]));
            
            var model = pipeline.Fit(data);

            return model;
        }

        private IEnumerable<float[]> PredictDataUsingModel(IDataView testData, TransformerChain<OnnxTransformer> model)
        {
            Trace.WriteLine($"Images location: {imagesFolder}");
            Trace.WriteLine("");
            Trace.WriteLine("=====Identify the objects in the images=====");
            Trace.WriteLine("");

            IDataView scoredData = model.Transform(testData);

            IEnumerable<float[]> probabilities = scoredData.GetColumn<float[]>(TinyYoloModelSettings.ModelOutput);

            return probabilities;
        }

        public IEnumerable<float[]> Score(IDataView data)
        {
            var model = LoadModel(modelLocation);

            return PredictDataUsingModel(data, model);
        }
    }
}
