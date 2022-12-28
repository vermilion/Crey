using Spear.Core.Helper;

namespace Spear.Core.Extensions
{
    ///<summary>
    /// 字符串通用扩展类
    ///</summary>
    public static class CommonExtension
    {
        /// <summary>
        /// 判断是否为空
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 获取该值的MD5
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Md5(this string str)
        {
            return str.IsNullOrEmpty() ? str : EncryptHelper.Hash(str, EncryptHelper.HashFormat.MD532);
        }

        /// <summary> 获取环境变量 </summary>
        /// <param name="name">变量名称</param>
        /// <param name="target">存储目标</param>
        /// <returns></returns>
        public static string Env(this string name, EnvironmentVariableTarget? target = null)
        {
            return target.HasValue
                ? Environment.GetEnvironmentVariable(name, target.Value)
                : Environment.GetEnvironmentVariable(name);
        }

        /// <summary> 获取环境变量 </summary>
        /// <param name="name">变量名称</param>
        /// <param name="type">值类型</param>
        /// <param name="target">存储目标</param>
        /// <returns></returns>
        public static object Env(this string name, Type type, EnvironmentVariableTarget? target = null)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            var env = name.Env(target);

            return string.IsNullOrWhiteSpace(env) ? null : env.CastTo(type);
        }

        /// <summary> 获取环境变量 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">变量名称</param>
        /// <param name="def">默认值</param>
        /// <param name="target">存储目标</param>
        /// <returns></returns>
        public static T Env<T>(this string name, T def = default, EnvironmentVariableTarget? target = null)
        {
            var value = name.Env(typeof(T), target);
            if (value == null) return def;
            return (T)value;
        }
    }
}
