using OneWare.Essentials.Models;

namespace FEntwumS.NetlistViewer.Helpers.Validators;

public class BackendPortValidator : ISettingValidation
{
	public bool Validate(object? value, out string? warningMessage)
	{
		warningMessage = "Invalid Port";

		if (Int64.TryParse((string?) value, null, out var portValue) && portValue is >= 1024 and <= 65535)
		{
			warningMessage = null;
			return true;
		}

		return false;
	}
}