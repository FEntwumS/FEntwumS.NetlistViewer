using OneWare.Essentials.Models;

namespace FEntwumS.NetlistViewer.Helpers.Validators;

public class FontSizeValidator : ISettingValidation
{
	public bool Validate(object? value, out string? warningMessage)
	{
		warningMessage = "Font size must be a positive integer";

		if (Int64.TryParse((string?) value, null, out var fontsizeValue) && fontsizeValue > 0)
		{
			warningMessage = null;
			
			return true;
		}

		return false;
	}
}