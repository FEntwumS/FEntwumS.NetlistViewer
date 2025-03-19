using Avalonia;
using OneWare.Essentials.ViewModels;

namespace FEntwumS.NetlistViewer.Types;

public class NetlistElement
{
    private double x { get; set; }
    private double y { get; set; }
    private int type { get; set; }
    private double width { get; set; }
    private double height { get; set; }
    private string? labelText { get; set; }
    private List<Point>? points { get; set; }
    private ushort zIndex { get; set; }

    private int signalindex { get; set; }
    private string? signalname { get; set; }
    private string? vector_signals { get; set; }
    private string? cellname { get; set; }
    private string? celltype { get; set; }
    private string? path { get; set; }
    private string? srcLocation { get; set; }
    private string? signalvalue { get; set; }
    private int indexInSignal { get; set; }
    private string? signaltype { get; set; }
    private bool isHighlighted { get; set; }
    private double fontSize { get; set; }

    public double xPos
    {
        get => this.x;
        set
        {
            x = value;
        }
    }

    public double yPos
    {
        get => this.y;
        set
        {
            y = value;
        }
    }

    public double Width
    {
        get => this.width;
        set
        {
            this.width = value;
        }
    }

    public double Height
    {
        get => this.height;
        set
        {
            this.height = value;
        }
    }

    public int Type
    {
        get => this.type;
        set
        {
            this.type = value;
        }
    }

    public string? LabelText
    {
        get => this.labelText;
        set
        {
            this.labelText = value;
        }
    }

    public List<Point>? Points
    {
        get => this.points;
        set
        {
            this.points = value;
        }
    }

    public ushort ZIndex
    {
        get => this.zIndex;
        set
        {
            this.zIndex = value;
        }
    }

    public int Signalindex
    {
        get => this.signalindex;
        set
        {
            this.signalindex = value;
        }
    }

    public string? Signalname
    {
        get => this.signalname;
        set
        {
            this.signalname = value;
        }
    }

    public string? VectorSignals
    {
        get => this.vector_signals;
        set
        {
            this.vector_signals = value;
        }
    }

    public string? Cellname
    {
        get => this.cellname;
        set
        {
            this.cellname = value;
        }
    }

    public string? Celltype
    {
        get => this.celltype;
        set
        {
            this.celltype = value;
        }
    }

    public string? Path
    {
        get => this.path;
        set
        {
            this.path = value;
        }
    }

    public string? SrcLocation
    {
        get => this.srcLocation;
        set
        {
            this.srcLocation = value;
        }
    }

    public string? Signalvalue
    {
        get => this.signalvalue;
        set
        {
            this.signalvalue = value;
        }
    }

    public int IndexInSignal
    {
        get => this.indexInSignal;
        set
        {
            this.indexInSignal = value;
        }
    }

    public string? SignalType
    {
        get => this.signaltype;
        set
        {
            this.signaltype = value;
        }
    }

    public bool IsHighlighted
    {
        get => this.isHighlighted;
        set
        {
            this.isHighlighted = value;
        }
    }

    public double FontSize
    {
        get => this.fontSize;
        set
        {
            this.fontSize = value;
        }
    }
}