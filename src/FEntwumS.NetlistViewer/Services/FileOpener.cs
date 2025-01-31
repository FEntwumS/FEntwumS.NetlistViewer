using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace FEntwumS.NetlistViewer.Services;

public class FileOpener : IFileOpener
{
    private Window _target;

    public FileOpener(Window target)
    {
        _target = target;
    }
    
    public void setWindow(Window target) { _target = target; }
    
    public async Task<IStorageFile?> OpenFileAsync()
    {
        var netlistFile = await _target.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Choose netlist",
            AllowMultiple = false
        });

        return netlistFile.Count >= 1 ? netlistFile[0] : null;
    }
}