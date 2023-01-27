using System.Collections.Generic;
using System.IO;

namespace YellowDuck.Api.Services.Files
{
    public class FileInfos
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public IDictionary<string, string> Metadata { get; set; }
        public Stream Content { get; set; }
        
        public bool UseFileNameAsFileId { get; set; }
    }
}