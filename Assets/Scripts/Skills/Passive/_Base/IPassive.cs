public interface IPassive
{
    public PassiveData Data { get; }

    public int Level { get; }

    public void Initialize();

    public void Update();

    public void OnRemove();

    public void Upgrade();
}