using FEntwumS.NetlistViewer.Types;
using OneWare.Essentials.Helpers;
using OneWare.Essentials.Models;

namespace FEntwumS.NetlistViewer.Helpers;

public class FentwumSNetlistViewerSettingsHelper
{
	#region Message Channel IDs

	public static readonly int HierarchyMessageChannel = 1;
	public static readonly int NetlistMessageChannel = 2;
	public static readonly int ProjectChangedMessageChannel = 3;

	#endregion

	public static readonly string ExpectedSettingsVersion = "3";

	public static readonly string DataDirectory = Path.Combine(
		Environment.GetFolderPath(
			PlatformHelper.Platform is PlatformId.OsxArm64 || PlatformHelper.Platform is PlatformId.OsxX64
				? Environment.SpecialFolder.LocalApplicationData
				: Environment.SpecialFolder.ApplicationData), "FEntwumSNetlistViewer");

	public static readonly string DataFilePath = Path.Combine(DataDirectory, "data.json");
	public static readonly string FentwumsSettingVersionKey = "Extension_SettingsVersion";
	public static readonly string NetlistGenerationSettingsChangedKey = "NetlistGenerationSettingsChanged";

	#region Settings keys

	public static readonly string NetlistPathSettingKey = "FEntwumS_NetlistReaderBackend";
	public static readonly string JavaPathSettingKey = "FEntwumS_JDKPath";
	public static readonly string JavaArgsKey = "NetlistViewer_JavaArgs";

	public static readonly string FpgaManufacturerKey = "NetlistViewer_Fpga_Manufacturer";
	public static readonly string FpgaDeviceFamilyKey = "NetlistViewer_Fpga_Device_Family";

	public static readonly string BackendAddressKey = "NetlistViewer_Backend_Address";
	public static readonly string BackendPortKey = "NetlistViewer_Backend_Port";
	public static readonly string BackendRequestTimeoutKey = "NetlistViewer_Backend_RequestTimeout";
	public static readonly string BackendUseLocalKey = "NetlistViewer_Backend_UseLocal";

	public static readonly string EntityFontSizeKey = "NetlistViewer_EntityFontSize";
	public static readonly string CellFontSizeKey = "NetlistViewer_CellFontSize";
	public static readonly string EdgeFontSizeKey = "NetlistViewer_EdgeFontSize";
	public static readonly string PortFontSizeKey = "NetlistViewer_PortFontSize";

	public static readonly string ContinueOnBinaryInstallErrorKey = "NetlistViewer_ContinueOnBinaryInstallError";
	public static readonly string UseHierarchicalBackendKey = "NetlistViewer_UseHierarchicalBackend";
	public static readonly string PerformanceTargetKey = "NetlistViewer_PerformanceTarget";
	public static readonly string AlwaysRegenerateNetlistsKey = "NetlistViewer_AlwaysRegenerateNetlists";
	public static readonly string EnableHierarchyViewKey = "NetlistViewer_EnableHierarchyView";
	public static readonly string AutomaticNetlistGenerationKey = "NetlistViewer_AutomaticNetlistGeneration";

	public static readonly string AutomaticNetlistGenerationIntervalKey =
		"NetlistViewer_AutomaticNetlistGenerationInterval";

	#endregion

	#region Project settings keys

	public static readonly string ProjectFpgaManufacturerKey = "FEntwumS_FPGA_Manufacturer";
	public static readonly string ProjectFpgaDeviceFamilyKey = "FEntwumS_FPGA_DeviceFamily";

	#endregion

	#region External settings keys

	public static readonly string OssCadSuitePathKey = "OssCadSuite_Path";
	public static readonly string AutoDownloadBinariesKey = "Experimental_AutoDownloadBinaries";

	#endregion

	#region Path generation methods

	public static string GetCcVhdlFilePath(IProjectFile projectFile)
	{
		string top = Path.GetFileNameWithoutExtension(projectFile.FullPath);

		return Path.Combine(GetBuildDirectory(projectFile), $"{top}.v");
	}

	public static string GetBuildDirectory(IProjectFile projectFile)
	{
		return Path.Combine(projectFile.Root.FullPath, "build", "netlist");
	}

	public static string GetNetlistFilePath(IProjectFile json, NetlistType netlistType)
	{
		string top = Path.GetFileNameWithoutExtension(json.FullPath);
		string suffix = netlistType switch
		{
			NetlistType.Flat => "flat",
			NetlistType.Hier => "hier",
		};

		return Path.Combine(GetBuildDirectory(json), $"{top}-{suffix}.json");
	}

	#endregion
}