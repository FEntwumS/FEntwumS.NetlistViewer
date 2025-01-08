using System.Text;

namespace FEntwumS.WfInteractor;

public class Util
{
    public static uint ComputeOneAtATimeHash(string input)
    {
        // Convert the string to a byte array using UTF8 encoding
        byte[] data = Encoding.UTF8.GetBytes(input);
        return ComputeOneAtATimeHash(data);
    }
    public static uint ComputeOneAtATimeHash(byte[] data)
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