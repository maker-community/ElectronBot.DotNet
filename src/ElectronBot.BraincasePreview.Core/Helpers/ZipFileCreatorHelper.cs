using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Verdure.ElectronBot.Core.Helpers;
public static class ZipFileCreatorHelper
{
    /// <summary>
    /// Create a ZIP file of the files provided.
    /// </summary>
    /// <param name="files">The files to add to the ZIP file.</param>
    /// <param name="zipPath">The path of the ZIP file to create.</param>
    public static void CreateZipFile(IEnumerable<string> files, string zipPath)
    {
        //Create a ZIP archive at the specified path
        using (var zipArchive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
        {
            //Add each file to the archive
            foreach (var filePath in files)
            {
                //Get the file name from the full path
                var fileName = Path.GetFileName(filePath);

                //Create an entry for this file and copy its contents to it
                var zipEntry = zipArchive.CreateEntry(fileName);
                using (var zipEntryStream = zipEntry.Open())
                using (var fileStream = File.OpenRead(filePath))
                {
                    fileStream.CopyTo(zipEntryStream);
                }
            }
        }
    }

    /// <summary>
    /// Extract a ZIP file to a directory.
    /// </summary>
    /// <param name="zipPath">The path of the ZIP file to extract.</param>
    /// <param name="extractPath">The directory to extract the files to.</param>
    public static void ExtractZipFile(string zipPath, string extractPath)
    {
        //Extract all entries from the ZIP archive to the specified directory
        ZipFile.ExtractToDirectory(zipPath, extractPath);
    }
}




//using (var content = new MultipartFormDataContent()) {
//  var mixed = new MultipartContent("mixed") {
//    CreateFileContent (imageStream, "image.jpg", "image/jpeg"),
//    CreateFileContent (signatureStream, "image.jpg.sig", "application/octet-stream")
//  };
//content.Add(mixed, "files");
//  var response = await httpClient.PostAsync(_profileImageUploadUri, content);
//response.EnsureSuccessStatusCode();
//}

//private StreamContent CreateFileContent(Stream stream, string fileName, string contentType)
//{
//    var fileContent = new StreamContent(stream);
//    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("file") { FileName = fileName };
//    fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
//    return fileContent;
//}
