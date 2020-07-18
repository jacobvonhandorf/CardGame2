using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garnet : SpellCard
{
    public const int CARD_ID = 24;
    public override int cardId => CARD_ID;
    public override List<Tile> legalTargetTiles => new List<Tile>();

    public override void onInitialization()
    {
        E_AddedToCardPile += Garnet_E_AddedToCardPile;
    }
    private void OnDestroy()
    {
        E_AddedToCardPile -= Garnet_E_AddedToCardPile;
    }

    private void Garnet_E_AddedToCardPile(object sender, AddedToCardPileArgs e)
    {
        if (e.newCardPile is Hand)
        {
            owner.addGold(1);
            showInEffectsView();
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
