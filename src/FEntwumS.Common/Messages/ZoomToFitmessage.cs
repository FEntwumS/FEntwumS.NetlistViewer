using CommunityToolkit.Mvvm.Messaging.Messages;

namespace FEntwumS.Common.Messages;

public class ZoomToFitmessage : ValueChangedMessage<ulong>
{
	public ZoomToFitmessage(ulong value) : base(value)
	{
	}
}