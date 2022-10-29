using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;

namespace ImGui.UnitTest
{
    public class UtilFacts
    {
        [Fact]
        public void GetProjectPathWhenRunningUnitTestInVisualStudio()
        {
            Assert.Contains(@"ImGui\test\ImGui.UnitTest\", Util.UnitTestRootDir);
        }

        [Fact]
        public void CompareIdenticalImage()
        {
            var imageA = Image.Load<Rgba32>("Util/images/logo.png");
            var imageB = Image.Load<Rgba32>("Util/images/logo.png");
            var result = Util.CompareImage(imageA, imageB);
            Assert.True(result);
        }

        [Fact]
        public void CompareSightlyDifferentImage()
        {
            var imageA = Image.Load<Rgba32>("Util/images/logo.png");
            var imageB = Image.Load<Rgba32>("Util/images/logo_diff.png");
            var result = Util.CompareImage(imageA, imageB);
            Assert.True(result);
        }

        [Fact]
        public void CompareCompletelyDifferentImage()
        {
            var imageA = Image.Load<Rgba32>("Util/images/logo.png");
            var imageB = Image.Load<Rgba32>("Util/images/logo_mess.png");
            var result = Util.CompareImage(imageA, imageB);
            Assert.False(result);
        }
    }
}