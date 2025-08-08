namespace FEntwumS.NetlistViewer.Services;

public class OAATHashService : IHashService
{
	// Adapted from https://en.wikipedia.org/wiki/Jenkins_hash_function#one_at_a_time
	public uint ComputeHash(ReadOnlySpan<byte> input)
	{
		UInt32 hash = 0;

		for (int i = 0; i < input.Length; i++)
		{
			hash += input[i];
			hash += hash << 10;
			hash ^= hash >> 6;
		}

		hash += hash << 3;
		hash ^= hash >> 11;
		hash += hash << 15;

		return hash;
	}
}