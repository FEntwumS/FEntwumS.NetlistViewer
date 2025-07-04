using CommunityToolkit.Mvvm.Messaging.Messages;

namespace FEntwumS.NetlistViewer.Types.Messages;

public class ZoomToFitmessage : ValueChangedMessage<bool>
{
    public ZoomToFitmessage(bool value) : base(value)
    {
    }
}