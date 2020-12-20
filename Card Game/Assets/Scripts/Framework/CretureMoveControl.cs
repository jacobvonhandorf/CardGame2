using UnityEngine;
using System.Collections.Generic;

public static class CreatureMoveControl
{
    public static Creature CurrentCreature { get; private set; }
    private static Color FilterColor { get; } = new Color(0, 1, 0, .75f);

    private static AboveTileFilter filter;
    private static List<AboveTileFilter> LoadedFilters = new List<AboveTileFilter>();

    public static void Setup(Creature creature)
    {
        CurrentCreature = creature;
        foreach (Tile t in Board.Instance.GetAllMovableTiles(creature))
        {
            filter = Object.Instantiate(PrefabHolder.Instance.aboveTileFilter, t.transform.position, Quaternion.identity, MainCanvas.Instance.transform);
            filter.tile = t;
            filter.image.color = FilterColor;
            filter.action = delegate (Tile selectedTile)
            {
                DestroyFilters();
                creature.Move(selectedTile);
                CurrentCreature = null;
                ActionBox.instance.Show(creature);
            };
            LoadedFilters.Add(filter);
        }
    }

    public static void Cancel()
    {
        CurrentCreature = null;
        DestroyFilters();
    }

    private static void DestroyFilters()
    {
        Debug.LogError("Destroying filters " + LoadedFilters.Count);
        foreach (AboveTileFilter f in LoadedFilters)
            Object.Destroy(f.gameObject);
        LoadedFilters.Clear();
    }

}
