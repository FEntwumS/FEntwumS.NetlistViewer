using CommunityToolkit.Mvvm.Messaging;
using DynamicData.Binding;
using FEntwumS.NetlistViewer.Helpers;
using FEntwumS.NetlistViewer.Types;
using FEntwumS.NetlistViewer.Types.Messages;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;
using OneWare.UniversalFpgaProjectSystem.Models;

namespace FEntwumS.NetlistViewer.Services;

public class ProjectMonitor: IProjectMonitor
{
    private AutomaticNetlistGenerationType _netlistGenerationType;
    
    private void CurrentDocument_FileSaved(IEditor editor)
    {
        if (editor.CurrentFile is IProjectFile { Root: UniversalFpgaProjectRoot root } && _netlistGenerationType == AutomaticNetlistGenerationType.Always)
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

        ServiceManager.GetService<ISettingsService>()
            .GetSettingObservable<string>(FentwumSNetlistViewerSettingsHelper.AutomaticNetlistGenerationKey).Subscribe(
                x =>
                {
                    _netlistGenerationType = x switch
                    {
                        "Never" => AutomaticNetlistGenerationType.Never,
                        "Always" => AutomaticNetlistGenerationType.Always,
                        "Every 5 minutes" => AutomaticNetlistGenerationType.Interval,
                        _ => AutomaticNetlistGenerationType.Never,
                    };

                });
    }
}