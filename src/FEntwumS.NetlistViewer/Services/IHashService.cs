namespace FEntwumS.NetlistViewer.Services;

public interface IHashService
{
	public UInt32 ComputeHash(ReadOnlySpan<byte> input);
}