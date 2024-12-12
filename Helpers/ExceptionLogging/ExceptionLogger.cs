using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Helpers.ExceptionLogging
{
    public static class ExceptionLogger
    {
        private static readonly string LogFilePath = "exceptions.log";

        public static void LogException(Exception ex)
        {
            using (StreamWriter writer = new StreamWriter(LogFilePath, true))
            {
                writer.WriteLine($"[{DateTime.Now}] {ex.Message}");
                writer.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }
    }
}
