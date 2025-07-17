using CommunityToolkit.Mvvm.Messaging.Messages;

namespace FEntwumS.NetlistViewer.Types.Messages;

public class ZoomToToplevelMessage : ValueChangedMessage<ulong>
{
    public ZoomToToplevelMessage(ulong value) : base(value) { }
}