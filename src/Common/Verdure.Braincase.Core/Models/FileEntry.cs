namespace Verdure.Braincase.Core.Models;
public class FileEntry : IDisposable
{
    public string FileName
    {
        get; set;
    } = string.Empty;
    public Stream FileStream
    {
        get; set;
    } = new MemoryStream();

    public void Dispose()
    {
        FileStream?.Dispose();
    }
}
