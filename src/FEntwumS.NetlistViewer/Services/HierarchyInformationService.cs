using System.Collections.Concurrent;
using FEntwumS.NetlistViewer.Types;

namespace FEntwumS.NetlistViewer.Services;

public class HierarchyInformationService : IHierarchyInformationService
{
	private ConcurrentDictionary<ulong, HierarchyInformation> _hierarchyData = new();

	public void setMaxHeight(ulong netlistId, double maxHeight)
	{
		if (_hierarchyData.ContainsKey(netlistId))
		{
			_hierarchyData[netlistId].MaxHeight = maxHeight;
		}
		else
		{
			_hierarchyData[netlistId] = new HierarchyInformation() { MaxHeight = maxHeight };
		}
	}

	public double getMaxHeight(ulong netlistId)
	{
		return _hierarchyData[netlistId].MaxHeight;
	}

	public void setMaxWidth(ulong netlistId, double maxWidth)
	{
		if (_hierarchyData.ContainsKey(netlistId))
		{
			_hierarchyData[netlistId].MaxWidth = maxWidth;
		}
		else
		{
			_hierarchyData[netlistId] = new HierarchyInformation() { MaxWidth = maxWidth };
		}
	}

	public double getMaxWidth(ulong netlistId)
	{
		return _hierarchyData[netlistId].MaxWidth;
	}

	public void setTopX(ulong netlistId, double topX)
	{
		if (_hierarchyData.ContainsKey(netlistId))
		{
			_hierarchyData[netlistId].TopX = topX;
		}
		else
		{
			_hierarchyData[netlistId] = new HierarchyInformation() { TopX = topX };
		}
	}

	public double getTopX(ulong netlistId)
	{
		return _hierarchyData[netlistId].TopX;
	}

	public void setTopY(ulong netlistId, double topY)
	{
		if (_hierarchyData.ContainsKey(netlistId))
		{
			_hierarchyData[netlistId].TopY = topY;
		}
		else
		{
			_hierarchyData[netlistId] = new HierarchyInformation() { TopY = topY };
		}
	}

	public double getTopY(ulong netlistId)
	{
		return _hierarchyData[netlistId].TopY;
	}

	public void setTopWidth(ulong netlistId, double topWidth)
	{
		if (_hierarchyData.ContainsKey(netlistId))
		{
			_hierarchyData[netlistId].TopWidth = topWidth;
		}
		else
		{
			_hierarchyData[netlistId] = new HierarchyInformation() { TopWidth = topWidth };
		}
	}

	public double getTopWidth(ulong netlistId)
	{
		return _hierarchyData[netlistId].TopWidth;
	}

	public void setTopHeight(ulong netlistId, double topHeight)
	{
		if (_hierarchyData.ContainsKey(netlistId))
		{
			_hierarchyData[netlistId].TopHeight = topHeight;
		}
		else
		{
			_hierarchyData[netlistId] = new HierarchyInformation() { TopHeight = topHeight };
		}
	}

	public double getTopHeight(ulong netlistId)
	{
		return _hierarchyData[netlistId].TopHeight;
	}
}