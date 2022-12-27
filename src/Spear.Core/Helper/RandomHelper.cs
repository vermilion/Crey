using System.Security.Cryptography;

namespace Spear.Core.Helper;

/// <summary>
/// 随机数辅助
/// </summary>
public static class RandomHelper
{
    /// <summary>
    /// 获取线程级随机数
    /// </summary>
    /// <returns></returns>
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
