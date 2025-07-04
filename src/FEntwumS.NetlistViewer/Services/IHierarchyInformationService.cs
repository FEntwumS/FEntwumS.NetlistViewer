namespace FEntwumS.NetlistViewer.Services;

public interface IHierarchyInformationService
{
    public void setMaxHeight(ulong netlistId,double maxHeight);
    public double getMaxHeight(ulong netlistId);
    
    public void setMaxWidth(ulong netlistId,double maxWidth);
    public double getMaxWidth(ulong netlistId);
    
    public void setTopX(ulong netlistId, double topX);
    public double getTopX(ulong netlistId);
    
    public void setTopY(ulong netlistId, double topY);
    public double getTopY(ulong netlistId);
    
    public void setTopWidth(ulong netlistId, double topWidth);
    public double getTopWidth(ulong netlistId);
    
    public void setTopHeight(ulong netlistId, double topHeight);
    public double getTopHeight(ulong netlistId);
}