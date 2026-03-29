namespace Zeke.PoolableGameObjects
{
    public interface IPoolableGameObjectConfirmator
    {
        public bool CanGetPoolable { get; }

        public void OnRetrievedFromPool();
    }
}