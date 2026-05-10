namespace Zeke.PoolableGameObjects
{
    public interface IPoolableGameObjectListener
    {
        public void OnSentToPool();

        public void OnRetrievedFromPool();
    }
}