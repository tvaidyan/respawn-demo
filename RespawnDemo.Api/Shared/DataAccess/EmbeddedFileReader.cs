using Microsoft.Extensions.FileProviders;
using System.Text;

namespace RespawnDemo.Api.Shared.DataAccess;
public interface IEmbeddedFileReader
{
    string ReadFile(string filePath);
}

public class EmbeddedFileReader : IEmbeddedFileReader
{
    private readonly IFileProvider fileProvider;

    public EmbeddedFileReader(IFileProvider fileProvider)
    {
        this.fileProvider = fileProvider;
    }

    public string ReadFile(string filePath)
    {
        using var stream = fileProvider.GetFileInfo(filePath).CreateReadStream();
        using var streamReader = new StreamReader(stream, Encoding.UTF8);
        return streamReader.ReadToEnd();
    }
}
