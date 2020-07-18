using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Topaz : SpellCard
{
    public override int cardId => 20;
    protected override List<Tag> getTags() => new List<Tag>() { Tag.Gem };
    public override List<Tile> legalTargetTiles => new List<Tile>();

    public override void onInitialization()
    {
        E_AddedToCardPile += Topaz_E_AddedToCardPile;
    }
    private void OnDestroy()
    {
        E_AddedToCardPile -= Topaz_E_AddedToCardPile;
    }

    private void Topaz_E_AddedToCardPile(object sender, AddedToCardPileArgs e)
    {
        if (e.newCardPile is Hand && e.source != null)
        {
            owner.addGold(1);
            owner.addMana(1);
            showInEffectsView();
        }
    }

    protected override void doEffect(Tile t) { }
}
