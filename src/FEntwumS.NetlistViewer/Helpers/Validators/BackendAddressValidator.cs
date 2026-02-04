using System.Net;
using OneWare.Essentials.Models;

namespace FEntwumS.NetlistViewer.Helpers.Validators;

public class BackendAddressValidator : ISettingValidation
{
	public bool Validate(object? value, out string? warningMessage)
	{
		warningMessage = "Invalid IP";
		
		if (value is string address && IPAddress.TryParse(address, out _))
		{
			warningMessage = null;

			return true;
		}

		return false;
	}
}