using System.Security.Cryptography;

namespace Spear.Core.Helper;

public static class RandomHelper
{
    public static Random Random()
    {
        var bytes = new byte[4];
        var rng = new RNGCryptoServiceProvider();
        rng.GetBytes(bytes);
        var seed = BitConverter.ToInt32(bytes, 0);
        var tick = DateTime.Now.Ticks + (seed);
        return new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
    }
}
