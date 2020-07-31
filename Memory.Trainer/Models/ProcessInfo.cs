using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace LegoCityUnderCover.Trainer.Models
{
    public class ProcessInfo
    {
        public string Name { get; set; }
        public string ProcessName { get; set; }
        public List<MemAddress> MemoryAddresses { get; set; } = new List<MemAddress>();

        public override string ToString()
        {
            return $"{Name} | {ProcessName}";
        }

        public static ProcessInfo ReadFile(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            using (var streamReader = new StreamReader(filePath))
            {
                return JsonConvert.DeserializeObject<ProcessInfo>(streamReader.ReadToEnd());
            }
        }

        public static void WriteFile(string filePath, ProcessInfo processInfo)
        {
            var dirPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            using (var streamWriter = new StreamWriter(filePath))
            {
                streamWriter.Write(JsonConvert.SerializeObject(processInfo));
            }
        }
    }
}
