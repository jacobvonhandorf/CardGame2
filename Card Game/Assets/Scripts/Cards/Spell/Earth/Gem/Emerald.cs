using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emerald : SpellCard
{
    public override int cardId => 22;
    public override List<Tile> getLegalTargetTiles() => new List<Tile>();
    protected override Effect getEffect() => null;
    protected override List<Tag> getTags() => new List<Tag>() { Tag.Gem };
}
