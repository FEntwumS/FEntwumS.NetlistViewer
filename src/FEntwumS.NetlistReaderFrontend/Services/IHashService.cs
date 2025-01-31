namespace FEntwumS.NetlistReaderFrontend.Services;

public interface IHashService
{
    public UInt32 ComputeHash(ReadOnlySpan<byte> input);
}