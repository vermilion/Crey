using Spear.Core.Exceptions;
using Spear.Core.Extensions;
using System;

namespace Spear.Core.Session
{
    public interface IMicroSession
    {
        /// <summary> 用户ID </summary>
        object UserId { get; }

        /// <summary> 用户名 </summary>
        string UserName { get; }

        /// <summary> 角色 </summary>
        string Role { get; }

        /// <summary> 使用租户 </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        IDisposable Use(SessionDto session);
    }

    public static class MicroSessionExtensions
    {
        /// <summary> 获取UserId </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static T GetUserId<T>(this IMicroSession session, T def = default)
        {
            return session.UserId.CastTo(def);
        }

        /// <summary> 获取必须的UserId(没有将抛出异常) </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <returns></returns>
        public static T GetRequiredUserId<T>(this IMicroSession session)
        {
            var value = session.UserId.CastTo<T>();
            if (Equals(value, default(T)))
                throw new SpearException("userId 不能为空");
            return value;
        }
    }
}
