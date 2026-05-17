using Xunit;
using TechMove.Service;

namespace TechMove.Tests
{
    public class FileValidationServiceTests
    {
        [Fact]
        public void IsValidPdf_ShouldReturnFalse_WhenFileIsExe()
        {
            // Arrange
            var service = new FileValidationService();

            string fileName = "malware.exe";
            string contentType = "application/x-msdownload";

            // Act
            bool result = service.IsValidPdf(fileName, contentType);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidPdf_ShouldReturnTrue_WhenFileIsPdf()
        {
            // Arrange
            var service = new FileValidationService();

            string fileName = "agreement.pdf";
            string contentType = "application/pdf";

            // Act
            bool result = service.IsValidPdf(fileName, contentType);

            // Assert
            Assert.True(result);
        }
    }
}
