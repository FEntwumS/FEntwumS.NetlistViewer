namespace FEntwumS.Common.Interfaces;

public interface IHashService
{
	public UInt32 ComputeHash(ReadOnlySpan<byte> input);
}