using System;
using System.Collections.Generic;

namespace YellowDuck.Api.Services.Files
{
    public class FileAttributes
    {
        public DateTime? LastWriteTime { get; set; }
        public long Length { get; set; }
        public IDictionary<string, string> Metadata { get; set; }
    }
}