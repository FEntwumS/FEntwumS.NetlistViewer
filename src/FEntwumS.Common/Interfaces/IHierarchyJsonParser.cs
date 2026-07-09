using FEntwumS.Common.Types.HierarchyView;

namespace FEntwumS.Common.Interfaces;


public interface IHierarchyJsonParser
{
	public Task<(HierarchySideBarElement? sidebarRoot, List<HierarchyViewElement>? hierarchyViewElements)>
		LoadHierarchyAsync(Stream hierarchyStream, ulong netlistId);
}