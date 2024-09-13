using ObjectDetector.YoloParser;

namespace ObjectDetectorAppTest.YoloParserInfo
{
    public class DimensionsBaseTest
    {
        private readonly DimensionsBase dimensions = new();
        
        [Fact]
        public void CanSetAndGetXPosition()
        {
            var testX = 10.0f;

            dimensions.X = testX;

            Assert.Equal(testX, dimensions.X);
        }

        [Fact]
        public void CanSetAndGetYPosition()
        {
            var testY = 250.0f;

            dimensions.Y = testY;

            Assert.Equal(testY, dimensions.Y);
        }

        [Fact]
        public void CanSetAndGetHeight()
        {
            var testHeight = 90.0f;

            dimensions.Height = testHeight;

            Assert.Equal(testHeight, dimensions.Height);
        }

        [Fact]
        public void CanSetAndGetWidth()
        {
            var testWidth = 70.0f;

            dimensions.Width = testWidth;

            Assert.Equal(testWidth, dimensions.Width);
        }
    }
}