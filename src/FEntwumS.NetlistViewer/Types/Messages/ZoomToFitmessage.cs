using CommunityToolkit.Mvvm.Messaging.Messages;

namespace FEntwumS.NetlistViewer.Types.Messages;

public class ZoomToFitmessage : ValueChangedMessage<ulong>
{
    public ZoomToFitmessage(ulong value) : base(value)
    {
    }
}