using Avalonia.Animation;
using FEntwumS.NetlistViewer.Services;
using OneWare.Essentials.Services;

namespace FEntwumS.NetlistViewer.Helpers;

public class SettingsUpgrader
{
    public static bool NeedsUpgrade()
    {
        IStorageService storageService = ServiceManager.GetService<IStorageService>();
        string currentSettingsVersion = storageService.GetKeyValuePairValue(FentwumSNetlistViewerSettingsHelper.FentwumsSettingVersionKey) ?? "0";
        
        return currentSettingsVersion != FentwumSNetlistViewerSettingsHelper.ExpectedSettingsVersion;
    }
    
    public static async Task UpgradeSettingsIfNecessaryAsync()
    {
        ISettingsService settingsService = ServiceManager.GetService<ISettingsService>();
        IStorageService storageService = ServiceManager.GetService<IStorageService>();
        IPaths paths = ServiceManager.GetService<IPaths>();
        int currentSettingsVersion = Convert.ToInt32(storageService.GetKeyValuePairValue(FentwumSNetlistViewerSettingsHelper.FentwumsSettingVersionKey) ?? "0");
        

        if (currentSettingsVersion <= 0)
        {
            settingsService.SetSettingValue(FentwumSNetlistViewerSettingsHelper.EnableHierarchyViewKey, true);
        }

        if (currentSettingsVersion <= 1)
        {
            if (settingsService.GetSettingValue<string>(FentwumSNetlistViewerSettingsHelper
                    .AutomaticNetlistGenerationKey) == "Every 5 minutes")
            {
                settingsService.SetSettingValue(FentwumSNetlistViewerSettingsHelper.AutomaticNetlistGenerationKey, "Interval");
            }
        }

        if (currentSettingsVersion <= 2)
        {
	        settingsService.SetSettingValue(FentwumSNetlistViewerSettingsHelper.UseHierarchicalBackendKey, true);
	        settingsService.SetSettingValue(FentwumSNetlistViewerSettingsHelper.PerformanceTargetKey, "Intelligent Ahead Of Time");
	        settingsService.SetSettingValue(FentwumSNetlistViewerSettingsHelper.EnableHierarchyViewKey, true);
	        settingsService.SetSettingValue(FentwumSNetlistViewerSettingsHelper.AlwaysRegenerateNetlistsKey, false);
        }
        
        storageService.SetKeyValuePairValue(FentwumSNetlistViewerSettingsHelper.FentwumsSettingVersionKey, FentwumSNetlistViewerSettingsHelper.ExpectedSettingsVersion);
        await storageService.SaveAsync();
    }
}