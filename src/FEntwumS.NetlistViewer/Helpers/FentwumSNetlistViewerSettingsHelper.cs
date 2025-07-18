using OneWare.Essentials.Helpers;

namespace FEntwumS.NetlistViewer.Helpers;

public class FentwumSNetlistViewerSettingsHelper
{
    public static readonly int HierarchyMessageChannel = 1;
    public static readonly int NetlistMessageChannel = 2;
    public static readonly string dataDirectory = Path.Combine(
        Environment.GetFolderPath(
            PlatformHelper.Platform is PlatformId.OsxArm64 || PlatformHelper.Platform is PlatformId.OsxX64
                ? Environment.SpecialFolder.LocalApplicationData
                : Environment.SpecialFolder.ApplicationData), "FEntwumSNetlistViewer");
}