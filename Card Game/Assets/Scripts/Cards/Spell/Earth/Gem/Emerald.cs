using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emerald : SpellCard
{
    public override int cardId => 22;
    public override List<Tile> legalTargetTiles => new List<Tile>();
    protected override List<Tag> getInitialTags() => new List<Tag>() { Tag.Gem };
    protected override void doEffect(Tile t) { }
}
