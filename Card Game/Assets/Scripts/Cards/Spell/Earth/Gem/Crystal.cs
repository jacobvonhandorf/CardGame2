using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : SpellCard
{
    public const int CARD_ID = 26;

    public override bool canBePlayed()
    {
        return false;
    }

    public override List<Tile> getLegalTargetTiles()
    {
        return new List<Tile>();
    }

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

    protected override Effect getEffect()
    {
        return null;
    }

    protected override List<Tag> getTags()
    {
        List<Tag> tags = new List<Tag>();
        tags.Add(Tag.Gem);
        return tags;
    }

    public override int getCardId()
    {
        return CARD_ID;
    }
}
