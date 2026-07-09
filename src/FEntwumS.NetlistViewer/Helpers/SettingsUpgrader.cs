using FEntwumS.Common.Interfaces;
using FEntwumS.Common.Services;
using FEntwumS.NetlistViewer.Services;
using OneWare.Essentials.Services;

namespace FEntwumS.NetlistViewer.Helpers;

public class SettingsUpgrader
{
	public static bool NeedsUpgrade()
	{
		IStorageService storageService = ServiceManager.GetService<IStorageService>();
		ISettingsService settingsService = ServiceManager.GetService<ISettingsService>();
		int legacySettingsVersion =
			int.Parse(storageService.GetKeyValuePairValue(FentwumSNetlistViewerSettingsHelper.FentwumsSettingVersionKey) ?? "-1");

		if (settingsService.GetSettingValue<int>(FentwumSNetlistViewerSettingsHelper.FentwumsSettingVersionKey) == -1)
		{
			settingsService.SetSettingValue(FentwumSNetlistViewerSettingsHelper.FentwumsSettingVersionKey, legacySettingsVersion);
		}

		int currentSettingVersion =
			settingsService.GetSettingValue<int>(FentwumSNetlistViewerSettingsHelper.FentwumsSettingVersionKey);

		return currentSettingVersion != FentwumSNetlistViewerSettingsHelper.ExpectedSettingsVersion;
	}

	public static async Task UpgradeSettingsIfNecessaryAsync()
	{
		ISettingsService settingsService = ServiceManager.GetService<ISettingsService>();
		IStorageService storageService = ServiceManager.GetService<IStorageService>();
		IPaths paths = ServiceManager.GetService<IPaths>();
		IProjectExplorerService projectExplorerService = ServiceManager.GetService<IProjectExplorerService>();
		int currentSettingsVersion =
			settingsService.GetSettingValue<int>(FentwumSNetlistViewerSettingsHelper.FentwumsSettingVersionKey);


		if (currentSettingsVersion <= 0)
		{
			settingsService.SetSettingValue(FentwumSNetlistViewerSettingsHelper.EnableHierarchyViewKey, true);
		}

		if (currentSettingsVersion <= 1)
		{
			if (settingsService.GetSettingValue<string>(FentwumSNetlistViewerSettingsHelper
				    .AutomaticNetlistGenerationKey) == "Every 5 minutes")
			{
				settingsService.SetSettingValue(FentwumSNetlistViewerSettingsHelper.AutomaticNetlistGenerationKey,
					"Interval");
			}
		}

		if (currentSettingsVersion <= 2)
		{
			settingsService.SetSettingValue(FentwumSNetlistViewerSettingsHelper.UseHierarchicalBackendKey, true);
			settingsService.SetSettingValue(FentwumSNetlistViewerSettingsHelper.PerformanceTargetKey,
				"Intelligent Ahead Of Time");
			settingsService.SetSettingValue(FentwumSNetlistViewerSettingsHelper.EnableHierarchyViewKey, true);
			settingsService.SetSettingValue(FentwumSNetlistViewerSettingsHelper.AlwaysRegenerateNetlistsKey, false);
		}

		if (currentSettingsVersion <= 3)
		{
			settingsService.SetSettingValue(FentwumSNetlistViewerSettingsHelper.AlwaysRegenerateNetlistsKey, true);
		}

		settingsService.SetSettingValue(FentwumSNetlistViewerSettingsHelper.FentwumsSettingVersionKey, FentwumSNetlistViewerSettingsHelper.ExpectedSettingsVersion);
	}
}