using System;

namespace Zeke.PoolableGameObjects
{
    public interface IPoolableGameObjectConfirmator
    {
        public Action<IPoolableGameObjectConfirmator> PoolableReady { get; set; }
        public Action<IPoolableGameObjectConfirmator> PoolableBusy { get; set; }
    }
}