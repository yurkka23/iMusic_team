
using Microsoft.AspNetCore.Http;

namespace iMusic.BL.Interfaces;

public interface IFileService
{
    Task<List<string>> GetFilesPathAsync(IEnumerable<IFormFile> collection);
    Task<string> GetFilePathAsync(IFormFile file);
    Task<List<string>> GetSongsFilesPathAsync(IEnumerable<IFormFile> collection);
    Task<string> GetSongFilePathAsync(IFormFile file);
    void DeleteFiles(IEnumerable<string> files);
    void DeleteFile(string file);
}
