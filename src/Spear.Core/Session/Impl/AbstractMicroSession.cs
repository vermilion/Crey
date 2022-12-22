using System;

namespace Spear.Core.Session.Impl
{
    public abstract class AbstractMicroSession : IMicroSession
    {
        protected SessionDto TempSession { get; private set; }

        public object UserId => TempSession?.UserId ?? GetUserId();

        public abstract string UserName { get; }
        public abstract string Role { get; }

        protected abstract object GetUserId();

        public IDisposable Use(SessionDto sessionDto)
        {
            TempSession = sessionDto;
            return new DisposeAction(() => { TempSession = null; });
        }
    }
}
