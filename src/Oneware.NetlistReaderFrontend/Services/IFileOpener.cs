using Avalonia.Platform.Storage;

namespace Oneware.NetlistReaderFrontend.Services;

public interface IFileOpener
{
    public Task<IStorageFile?> OpenFileAsync();
}