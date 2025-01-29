using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace FEntwumS.NetlistReaderFrontend.Services;

public interface IFileOpener
{
    public Task<IStorageFile?> OpenFileAsync();

    public void setWindow(Window target);
}