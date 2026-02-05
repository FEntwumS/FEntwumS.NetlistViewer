using OneWare.Essentials.Models;

namespace FEntwumS.NetlistViewer.Helpers.Validators;

public class RequestTimeoutValidator : ISettingValidation
{
	public bool Validate(object? value, out string? warningMessage)
	{
		warningMessage = "Timeout must be a positive integer";

		if (Int64.TryParse((string?) value, null, out var timeoutValue) && timeoutValue > 0)
		{
			warningMessage = null;
			
			return true;
		}

		return false;
	}
}