using InventorySystem.Helpers.WwwRoot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Helpers
{
    public class FileUploadHelper
    {
        static WwwRootController rootController = new WwwRootController();
        public static async Task<string> UploadFileAsync(string filePath, string folderName)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path is invalid or empty.");

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(filePath);
            var targetPath = rootController.WwwRoot(folderName, fileName);

            using (var stream = new FileStream(targetPath, FileMode.Create))
            {
                using (var sourceStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    await sourceStream.CopyToAsync(stream);
                }
            }

            return $"/{folderName}/{fileName}";
        }

    }
}
