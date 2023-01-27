using System.IO;
using System.Threading.Tasks;

namespace YellowDuck.Api.Services.Files
{
    public interface IFileManager
    {
        Task<string> UploadFile(string container, FileInfos file);
        Task<FileInfos> DownloadFile(string container, string fileId);
        Task DeleteFile(string container, string fileId);

        Task<FileAttributes> GetFileAttributes(string container, string fileId);
        Task<Stream> GetFileStream(string container, string fileId);
        Task<bool> Exists(string container, string fileId);
    }
}