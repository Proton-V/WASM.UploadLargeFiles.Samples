using System;

namespace API.AspNet.Shared.Models
{
    public class FileData
    {
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public long SizeInBytes { get; set; }
    }
}