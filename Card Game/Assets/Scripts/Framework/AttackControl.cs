using UnityEngine;
using System.Collections.Generic;

public class AttackControl
{
    private static Color FilterColor { get; } = new Color(1, 0, 0, .75f);

    private static AboveTileFilter filter;
    private static List<AboveTileFilter> LoadedFilters = new List<AboveTileFilter>();

    public static void Setup(Creature attacker)
    {
        foreach (Tile t in attacker.AttackableTiles)
        {
            filter = Object.Instantiate(PrefabHolder.Instance.aboveTileFilter, t.transform, MainCanvas.Instance.transform);
            filter.tile = t;
            filter.image.color = FilterColor;
            filter.action = delegate (Tile selectedTile)
            {
                DestroyFilters();
                attacker.Attack(selectedTile.Permanent as Damageable);
            };
            LoadedFilters.Add(filter);
        }
    }

    public static void Cancel()
    {
        DestroyFilters();
    }

    private static void DestroyFilters()
    {
        foreach (AboveTileFilter f in LoadedFilters)
            Object.Destroy(f.gameObject);
        LoadedFilters.Clear();
    }
}
