using CommunityToolkit.Mvvm.Messaging.Messages;
using OneWare.UniversalFpgaProjectSystem.Models;

namespace FEntwumS.Common.Messages;

public class NetlistChangedMessage : ValueChangedMessage<UniversalFpgaProjectRoot>
{
	public NetlistChangedMessage(UniversalFpgaProjectRoot value) : base(value)
	{
	}
}