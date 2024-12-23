using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Helpers.WwwRoot
{
    public class WwwRootController
    {
        private readonly string rootPath;

        public WwwRootController()
        {
            rootPath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "wwwroot");
        }
        public string WwwRoot(string folder, string fileName)
        {
            var folderPath = Path.Combine(rootPath, folder);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);

            /*if (!File.Exists(filePath))
            {
                using (var stream = new FileStream(filePath, FileMode.Create));
            }*/
            return filePath;
        }
    }
}
