public class FileValidationService
{
    public bool IsValidPdf(string fileName, string contentType)
    {
        var extension = Path.GetExtension(fileName).ToLower();

        return extension == ".pdf" && contentType == "application/pdf";
    }
}
