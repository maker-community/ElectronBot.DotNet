using ICSharpCode.SharpZipLib.Zip;

namespace Verdure.ElectronBot.Core.Helpers;
public static class ZipFileCreatorHelper
{
    /// <summary>
    /// Create a ZIP file of the files provided.
    /// </summary>
    /// <param name="files">The files to add to the ZIP file.</param>
    /// <param name="zipPath">The path of the ZIP file to create.</param>
    public static void CreateZipFile(IEnumerable<string> files, string zipPath, string password = null)
    {
        // Create a zip output stream
        using (var zipStream = new ZipOutputStream(File.Create(zipPath)))
        {
            // Set password
            if (!string.IsNullOrWhiteSpace(password))
            {
                zipStream.Password = password;
            }

            // Loop through files to compress
            foreach (var file in files)
            {
                // Create a zip entry for each file
                var entry = new ZipEntry(Path.GetFileName(file));
                entry.DateTime = DateTime.Now;
                zipStream.PutNextEntry(entry);

                // Copy file content to zip stream
                using (var fs = File.OpenRead(file))
                {
                    fs.CopyTo(zipStream);
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
        // 创建一个ZipInputStream对象，并打开要解压的文件
        using (var zipStream = new ZipInputStream(File.OpenRead(zipPath)))
        {
            ZipEntry entry;

            // 循环读取每个条目
            while ((entry = zipStream.GetNextEntry()) != null)
            {
                // 获取条目的完整路径
                string entryPath = Path.Combine(extractPath, entry.Name);

                // 如果条目是一个目录，则创建该目录
                if (entry.IsDirectory)
                {
                    Directory.CreateDirectory(entryPath);
                }
                else // 如果条目是一个文件，则创建该文件并写入数据
                {
                    // 创建父级目录，如果不存在的话
                    Directory.CreateDirectory(Path.GetDirectoryName(entryPath));

                    // 创建一个FileStream对象，并打开要写入的文件
                    using (var fileStream = File.Create(entryPath))
                    {
                        // 创建一个缓冲区，用于存储从ZipInputStream中读取的数据
                        byte[] buffer = new byte[4096];
                        int bytesRead;

                        // 循环读取数据，直到达到文件末尾
                        while ((bytesRead = zipStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            // 将数据写入FileStream中
                            fileStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
        }
    }
}
