using iMusic.BL.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace iMusic.BL.Services;

public class FileService : IFileService 
{
    public async Task<List<string>> GetFilesPathAsync(IEnumerable<IFormFile> collection)
    {
        List<string> files = new List<string>();
        foreach (var file in collection)
        {
            files.Add(await SaveFileToApi(file));
        }
        return files;
    }
    public async Task<string> GetFilePathAsync(IFormFile file)
    {
        return await SaveFileToApi(file);
    }

    public async Task<List<string>> GetSongsFilesPathAsync(IEnumerable<IFormFile> collection)
    {
        List<string> files = new List<string>();
        foreach (var file in collection)
        {
            files.Add(await SaveFileSongToApi(file));
        }
        return files;
    }
    public async Task<string> GetSongFilePathAsync(IFormFile file)
    {
        return await SaveFileSongToApi(file);
    }

    public void DeleteFiles(IEnumerable<string> files)
    {
        foreach (var file in files)
        {
            File.Delete(file);
        }
    }
    public void DeleteFile(string file)
    {
        File.Delete(file);
    }

    private async Task<string> SaveFileToApi(IFormFile file)
    {
        var folderName = Path.Combine("AppFiles", "Images");
        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
      //  var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"').Split(".");
        var fileName = file.ContentType.Split('/').Last().ToString();
        var newFileName = new string(Guid.NewGuid() + "." + fileName);
        var fullPath = Path.Combine(pathToSave, newFileName);
        var path = Path.Combine(folderName, newFileName);
        await using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return path;
    }

    private async Task<string> SaveFileSongToApi(IFormFile file)
    {
        var folderName = Path.Combine("AppFiles", "Songs");
        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"').Split(".");
        var newFileName = new string(Guid.NewGuid() + "." + fileName.Last());
        var fullPath = Path.Combine(pathToSave, newFileName);
        var path = Path.Combine(folderName, newFileName);
        await using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return path;
    }

}
