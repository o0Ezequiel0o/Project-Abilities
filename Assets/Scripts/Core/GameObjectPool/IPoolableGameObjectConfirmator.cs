public interface IPoolableGameObjectConfirmator
{
    public bool CanGetPoolable { get; }

    public void OnPoolableGet();
}