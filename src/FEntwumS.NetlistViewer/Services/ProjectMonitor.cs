using CommunityToolkit.Mvvm.Messaging;
using DynamicData.Binding;
using FEntwumS.NetlistViewer.Types.Messages;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;
using OneWare.UniversalFpgaProjectSystem.Models;

namespace FEntwumS.NetlistViewer.Services;

public class ProjectMonitor: IProjectMonitor
{
    private void CurrentDocument_FileSaved(IEditor editor)
    {
        if (editor.CurrentFile is IProjectFile projectFile && projectFile.Root is UniversalFpgaProjectRoot root)
        {
            WeakReferenceMessenger.Default.Send(new NetlistChangedMessage(root));
        }
    }
    
    public void SubscribeToSettings()
    {
        ServiceManager.GetService<IDockService>().WhenValueChanged(x => x.CurrentDocument).Subscribe(x =>
        {
            if (x is IEditor editor)
            {
                editor.FileSaved += (sender, args) => { CurrentDocument_FileSaved(editor); };
            }
        });
    }
}