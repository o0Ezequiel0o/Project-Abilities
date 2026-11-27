using System.Collections.Generic;

public class EnemyRegistry : GameService<EnemyRegistry>
{
    private readonly List<Spawnable> enemies = new List<Spawnable>();

    public void FilterSpawnables(List<Spawnable> spawnables, List<Spawnable> filter, Filter filterType)
    {
        spawnables.Clear();

        for (int i = 0; i < enemies.Count; i++)
        {
            switch (filterType)
            {
                case Filter.Blacklist:
                    if (!filter.Contains(enemies[i])) spawnables.Add(enemies[i]);
                    break;

                case Filter.Whitelist:
                    if (filter.Contains(enemies[i])) spawnables.Add(enemies[i]);
                    break;
            }
        }
    }

    public List<Spawnable> FilterSpawnables(List<Spawnable> filter, Filter filterType)
    {
        List<Spawnable> spawnables = new List<Spawnable>();
        FilterSpawnables(spawnables, filter, filterType);
        return spawnables;
    }

    public void Add(List<Spawnable> spawnables)
    {
        for (int i = 0; i < spawnables.Count; i++)
        {
            Add(spawnables[i]);
        }
    }

    public void Add(Spawnable spawnable)
    {
        if (spawnable == null)
        {
            throw new System.Exception("Spawnable cannot be null");
        }

        enemies.Add(spawnable);
    }

    public void Remove(Spawnable spawnable)
    {
        if (spawnable == null)
        {
            throw new System.Exception("Spawnable cannot be null");
        }

        enemies.Remove(spawnable);
    }

    public enum Filter
    {
        Blacklist,
        Whitelist
    }
}