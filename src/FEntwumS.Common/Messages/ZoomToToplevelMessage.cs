using CommunityToolkit.Mvvm.Messaging.Messages;

namespace FEntwumS.Common.Messages;

public class ZoomToToplevelMessage : ValueChangedMessage<ulong>
{
	public ZoomToToplevelMessage(ulong value) : base(value)
	{
	}
}