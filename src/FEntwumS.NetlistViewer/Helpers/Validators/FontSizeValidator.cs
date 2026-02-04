using OneWare.Essentials.Models;

namespace FEntwumS.NetlistViewer.Helpers.Validators;

public class FontSizeValidator : ISettingValidation
{
	public bool Validate(object? value, out string? warningMessage)
	{
		warningMessage = "Font size must be a positive integer";

		if (value is int and > 0)
		{
			warningMessage = null;
			
			return true;
		}

		return false;
	}
}