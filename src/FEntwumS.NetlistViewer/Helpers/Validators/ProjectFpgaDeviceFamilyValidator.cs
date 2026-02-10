using FEntwumS.NetlistViewer.Services;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.UniversalFpgaProjectSystem.Models;

namespace FEntwumS.NetlistViewer.Helpers.Validators;

public class ProjectFpgaDeviceFamilyValidator : ISettingValidation
{
	public bool Validate(object? value, out string? warningMessage)
	{
		warningMessage = "Invalid device family";

		var currentProject = ServiceManager.GetService<IProjectExplorerService>().ActiveProject;

		if (currentProject is not UniversalFpgaProjectRoot root)
		{
			warningMessage = "Invalid device family";
			return false;
		}

		var setting = ServiceManager.GetService<IProjectSettingsService>().GetProjectSettingsList().Find(p =>
			p.Key == FentwumSNetlistViewerSettingsHelper.ProjectFpgaManufacturerKey);

		if (setting is null)
		{
			warningMessage = "Invalid device family";
			return false;
		}
		
		string? currentManufacturer = (string?)setting.Setting.Value;

		if (currentManufacturer is null)
		{
			warningMessage = "Invalid device family";
			return false;
		}

		switch (currentManufacturer)
		{
			case "gowin":
				if (value is string gowinString &&
				    FentwumSNetlistViewerSettingsHelper.GowinFamilies.Contains(gowinString))
				{
					warningMessage = null;
					return true;
				}
				else
				{
					warningMessage = $"Invalid device family. Valid options are : {string.Join(" ", FentwumSNetlistViewerSettingsHelper.GowinFamilies)}";
					return false;
				}
			case "intel_alm":
				if (value is string almString &&
				    FentwumSNetlistViewerSettingsHelper.IntelAlmFamilies.Contains(almString))
				{
					warningMessage = null;
					return true;
				}
				else
				{
					warningMessage = $"Invalid device family. Valid options are : {string.Join(" ", FentwumSNetlistViewerSettingsHelper.IntelAlmFamilies)}";
					return false;
				}
			case "quicklogic":
				if (value is string quicklogicString &&
				    FentwumSNetlistViewerSettingsHelper.QuickLogicFamilies.Contains(quicklogicString))
				{
					warningMessage = null;
					return true;
				}
				else
				{
					warningMessage = $"Invalid device family. Valid options are : {string.Join(" ", FentwumSNetlistViewerSettingsHelper.QuickLogicFamilies)}";
					return false;
				}
			default:
				warningMessage = null;
				return true;
		}

		return false;
	}
}