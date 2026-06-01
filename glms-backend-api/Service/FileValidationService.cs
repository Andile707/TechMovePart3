using System.IO;
public class FileValidationService
{
    public bool IsValidPdf(string fileName, string contentType)
    {
        if (string.IsNullOrWhiteSpace(fileName) ||
            string.IsNullOrWhiteSpace(contentType))
        {
            return false;
        }

        var extension = Path.GetExtension(fileName)
            .ToLowerInvariant();

        return extension == ".pdf" &&
               contentType == "application/pdf";
    }
}