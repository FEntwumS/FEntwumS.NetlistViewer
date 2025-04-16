using System.Text;

namespace FEntwumS.WaveformInteractor;

public class Util
{
    public static uint ComputeOneAtATimeHash(ReadOnlySpan<byte> data)
    {
        uint hash = 0;
        for (int i = 0; i < data.Length; ++i)
        {
            hash += data[i];
            hash += (hash << 10);
            hash ^= (hash >> 6);
        }
        hash += (hash << 3);
        hash ^= (hash >> 11);
        hash += (hash << 15);
        return hash;
    }
}