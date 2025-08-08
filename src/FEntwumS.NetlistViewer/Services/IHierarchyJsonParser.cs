using FEntwumS.NetlistViewer.Types.HierarchyView;

namespace FEntwumS.NetlistViewer.Services;

public interface IHierarchyJsonParser
{
	public Task<(HierarchySideBarElement? sidebarRoot, List<HierarchyViewElement>? hierarchyViewElements)>
		LoadHierarchyAsync(Stream hierarchyStream, ulong netlistId);
}