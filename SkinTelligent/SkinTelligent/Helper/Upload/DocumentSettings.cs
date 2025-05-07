namespace SkinTelligent.Api.Helper.Upload
{
    public class DocumentSettings
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };

        public static string UploadFile(IFormFile file, string folderName)
        {
            ValidateFile(file);

            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\image", folderName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileHash = GetFileHash(file);

            string existingFile = Directory.GetFiles(folderPath)
                .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == fileHash)!;

            if (existingFile != null)
            {
                return Path.GetFileName(existingFile);
            }

            string extension = Path.GetExtension(file.FileName).ToLower();
            string uniqueFileName = $"{fileHash}{extension}";
            string filePath = Path.Combine(folderPath, uniqueFileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(fileStream);

            return uniqueFileName;
        }

        public static string UpdateFile(IFormFile newFile, string folderName, string oldFileName)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\image", folderName);

            string oldFilePath = Path.Combine(folderPath, oldFileName);
            if (File.Exists(oldFilePath))
            {
                File.Delete(oldFilePath);
            }

            return UploadFile(newFile, folderName);
        }

        public static bool DeleteFile(string folderName, string fileName)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\image", folderName);
            string filePath = Path.Combine(folderPath, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }

            return false;
        }

        private static string GetFileHash(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private static void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new InvalidOperationException("File is empty or null.");
            }

            string extension = Path.GetExtension(file.FileName).ToLower();
            if (!AllowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException("Unsupported file type.");
            }

            string contentType = file.ContentType;
            if (!contentType.StartsWith("image/") && contentType != "application/pdf")
            {
                throw new InvalidOperationException("Only image and PDF files are allowed.");
            }
        }
    }
}
