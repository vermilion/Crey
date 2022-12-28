using Spear.Core.Helper;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Spear.Core.Extensions
{
    /// <summary> 正则相关扩展 </summary>
    public static class RegexExtension
    {
        private const string IpRegex = @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$";

        /// <summary> 是否是IP </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIp(this string ip)
        {
            return !string.IsNullOrWhiteSpace(ip) && Regex.IsMatch(ip, IpRegex);
        }
    }
}
