using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : SpellCard
{
    public const int CARD_ID = 26;
    public override int cardId => CARD_ID;
    public override List<Tile> legalTargetTiles => new List<Tile>();

    public override void onInitialization()
    {
        E_AddedToCardPile += Crystal_E_AddedToCardPile;
    }
    private void OnDestroy()
    {
        E_AddedToCardPile -= Crystal_E_AddedToCardPile;
    }

    private void Crystal_E_AddedToCardPile(object sender, AddedToCardPileArgs e)
    {
        if (e.newCardPile is Hand)
        {
            showInEffectsView();
            owner.addMana(1);
        }
    }

    protected override List<Tag> getTags()
    {
        List<Tag> tags = new List<Tag>();
        tags.Add(Tag.Gem);
        return tags;
    }

    protected override void doEffect(Tile t) { }
}
