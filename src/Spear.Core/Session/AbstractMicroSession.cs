using System;
using Spear.Core.Session.Abstractions;
using Spear.Core.Session.Models;

namespace Spear.Core.Session
{
    public abstract class AbstractMicroSession : IMicroSession
    {
        protected SessionDto TempSession { get; private set; }

        public object UserId => TempSession?.UserId ?? GetUserId();

        public abstract string UserName { get; }
        public abstract string Role { get; }

        protected abstract object GetUserId();
    }
}
