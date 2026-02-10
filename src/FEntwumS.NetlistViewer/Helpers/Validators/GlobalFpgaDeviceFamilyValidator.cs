using FEntwumS.NetlistViewer.Services;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;

namespace FEntwumS.NetlistViewer.Helpers.Validators;

public class GlobalFpgaDeviceFamilyValidator : ISettingValidation
{
	public bool Validate(object? value, out string? warningMessage)
	{
		warningMessage = "Invalid device family";
		
		string currentManufacturer = ServiceManager.GetService<ISettingsService>().GetSettingValue<string>(FentwumSNetlistViewerSettingsHelper.FpgaManufacturerKey);

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