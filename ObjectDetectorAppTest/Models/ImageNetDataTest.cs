using ObjectDetector.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ObjectDetectorAppTest.Models
{
    public class ImageNetDataTest : IDisposable
    {
        private const string TestDirectory = "TestImages";

        public ImageNetDataTest()
        {
            if (Directory.Exists(TestDirectory))
            {
                Directory.Delete(TestDirectory, true);
            }
            Directory.CreateDirectory(TestDirectory);
        }

        public void Dispose()
        {
            if (Directory.Exists(TestDirectory))
            {
                Directory.Delete(TestDirectory, true);
            }
        }

        [Fact]
        public void ReadFromFile_ReturnsCorrectNumberOfFiles()
        {
            File.Create(Path.Combine(TestDirectory, "image1.jpg")).Dispose();
            File.Create(Path.Combine(TestDirectory, "image2.jpg")).Dispose();

            var result = ImageNetData.ReadFromFile(TestDirectory);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void ReadFromFile_ExcludesMdFiles()
        {
            File.Create(Path.Combine(TestDirectory, "image1.jpg")).Dispose();
            File.Create(Path.Combine(TestDirectory, "readme.md")).Dispose();

            var result = ImageNetData.ReadFromFile(TestDirectory);

            Assert.Single(result);
            Assert.Equal("image1.jpg", result.First().Label);
        }

        [Fact]
        public void ReadFromFile_HandlesEmptyDirectory()
        {
            var result = ImageNetData.ReadFromFile(TestDirectory);

            Assert.Empty(result);
        }

        [Fact]
        public void ReadFromFile_ReturnsEmptyWhenOnlyMdFiles()
        {
            File.Create(Path.Combine(TestDirectory, "readme.md")).Dispose();

            var result = ImageNetData.ReadFromFile(TestDirectory);

            Assert.Empty(result);
        }

        [Fact]
        public void ReadFromFile_ThrowsExceptionForNullPath()
        {
            Assert.Throws<ArgumentNullException>(() => ImageNetData.ReadFromFile(null));
        }

        [Fact]
        public void ReadFromFile_ThrowsExceptionForEmptyPath()
        {
            Assert.Throws<ArgumentException>(() => ImageNetData.ReadFromFile(string.Empty));
        }
    }
}

