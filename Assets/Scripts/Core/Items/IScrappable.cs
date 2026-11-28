public interface IScrappable
{
    public int Priority { get; }

    public void OnScrapped();
}