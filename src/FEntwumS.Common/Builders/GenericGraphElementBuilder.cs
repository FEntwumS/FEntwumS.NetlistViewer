using FEntwumS.Common.Controls;
using FEntwumS.Common.Types;

namespace FEntwumS.Common.Builders;

public class GenericGraphElementBuilder<T> where T: GenericGraphElementControl, new()
{
	private double _x;
	private double _y;
	private	double _width;
	private	double _height;
	private NetlistTheme? _netlistTheme;
	private string _srclocation = "";
	private int _zIndex = 0;

	public static GenericGraphElementBuilder<T> Create()
	{
		return new GenericGraphElementBuilder<T>();
	}

	public GenericGraphElementBuilder<T> WithX(double x)
	{
		this._x = x;
		return this;
	}

	public GenericGraphElementBuilder<T> WithY(double y)
	{
		this._y = y;
		return this;
	}

	public GenericGraphElementBuilder<T> WithWidth(double width)
	{
		this._width = width;
		return this;
	}

	public GenericGraphElementBuilder<T> WithHeight(double height)
	{
		this._height = height;
		return this;
	}

	public GenericGraphElementBuilder<T> WithNetlistTheme(NetlistTheme netlistTheme)
	{
		this._netlistTheme = netlistTheme;
		return this;
	}

	public GenericGraphElementBuilder<T> WithSrclocation(string srclocation)
	{
		this._srclocation = srclocation;
		return this;
	}

	public GenericGraphElementBuilder<T> WithZIndex(int zIndex)
	{
		this._zIndex = zIndex;
		return this;
	}

	public T Build()
	{
		var n = new T()
		{
			X = this._x,
			Y = this._y,
			Width = this._width,
			Height = this._height,
			srcLocation = this._srclocation,
			ZIndex = _zIndex,
			ClipToBounds = false,
			IsHitTestVisible = false
		};

		if (this._netlistTheme is not null)
		{
			n.NetlistTheme = this._netlistTheme;
		}

		return n;
	}
}