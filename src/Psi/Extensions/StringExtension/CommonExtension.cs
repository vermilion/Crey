using Psi.Helper;

namespace Psi.Extensions.StringExtension;

public static class CommonExtension
{
    public static bool IsNullOrEmpty(this string str)
    {
        return string.IsNullOrEmpty(str);
    }

    public static string Md5(this string str)
    {
        return str.IsNullOrEmpty() ? str : EncryptHelper.Hash(str, EncryptHelper.HashFormat.MD532);
    }
}
