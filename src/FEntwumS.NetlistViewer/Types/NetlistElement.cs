using Avalonia;
using OneWare.Essentials.ViewModels;

namespace FEntwumS.NetlistViewer.Types;

public class NetlistElement : FlexibleWindowViewModelBase
{
    private double x { get; set; }
    private double y { get; set; }
    private int type { get; set; }
    private double width { get; set; }
    private double height { get; set; }
    private string labelText { get; set; }
    private List<Point> points { get; set; }
    private ushort zIndex { get; set; }

    private int signalindex { get; set; }
    private string signalname { get; set; }
    private string vector_signals { get; set; }
    private string cellname { get; set; }
    private string celltype { get; set; }
    private string path { get; set; }
    private string srcLocation { get; set; }
    private string signalvalue { get; set; }
    private int indexInSignal { get; set; }
    private string signaltype { get; set; }
    private bool isHighlighted { get; set; }
    private double fontSize { get; set; }

    public double xPos
    {
        get => this.x;
        set
        {
            x = value;
            OnPropertyChanged();
        }
    }

    public double yPos
    {
        get => this.y;
        set
        {
            y = value;
            OnPropertyChanged();
        }
    }

    public double Width
    {
        get => this.width;
        set
        {
            this.width = value;
            OnPropertyChanged();
        }
    }

    public double Height
    {
        get => this.height;
        set
        {
            this.height = value;
            OnPropertyChanged();
        }
    }

    public int Type
    {
        get => this.type;
        set
        {
            this.type = value;
            OnPropertyChanged();
        }
    }

    public string LabelText
    {
        get => this.labelText;
        set
        {
            this.labelText = value;
            OnPropertyChanged();
        }
    }

    public List<Point> Points
    {
        get => this.points;
        set
        {
            this.points = value;
            OnPropertyChanged();
        }
    }

    public ushort ZIndex
    {
        get => this.zIndex;
        set
        {
            this.zIndex = value;
            OnPropertyChanged();
        }
    }

    public int Signalindex
    {
        get => this.signalindex;
        set
        {
            this.signalindex = value;
            OnPropertyChanged();
        }
    }

    public string Signalname
    {
        get => this.signalname;
        set
        {
            this.signalname = value;
            OnPropertyChanged();
        }
    }

    public string VectorSignals
    {
        get => this.vector_signals;
        set
        {
            this.vector_signals = value;
            OnPropertyChanged();
        }
    }

    public string Cellname
    {
        get => this.cellname;
        set
        {
            this.cellname = value;
            OnPropertyChanged();
        }
    }

    public string Celltype
    {
        get => this.celltype;
        set
        {
            this.celltype = value;
            OnPropertyChanged();
        }
    }

    public string Path
    {
        get => this.path;
        set
        {
            this.path = value;
            OnPropertyChanged();
        }
    }

    public string SrcLocation
    {
        get => this.srcLocation;
        set
        {
            this.srcLocation = value;
            OnPropertyChanged();
        }
    }

    public string Signalvalue
    {
        get => this.signalvalue;
        set
        {
            this.signalvalue = value;
            OnPropertyChanged();
        }
    }

    public int IndexInSignal
    {
        get => this.indexInSignal;
        set
        {
            this.indexInSignal = value;
            OnPropertyChanged();
        }
    }

    public string SignalType
    {
        get => this.signaltype;
        set
        {
            this.signaltype = value;
            OnPropertyChanged();
        }
    }

    public bool IsHighlighted
    {
        get => this.isHighlighted;
        set
        {
            this.isHighlighted = value;
            OnPropertyChanged();
        }
    }

    public double FontSize
    {
        get => this.fontSize;
        set
        {
            this.fontSize = value;
            OnPropertyChanged();
        }
    }
}