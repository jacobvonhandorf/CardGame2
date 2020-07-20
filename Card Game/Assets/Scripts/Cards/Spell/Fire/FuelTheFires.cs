using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelTheFires : SpellCard
{
    public override int cardId => 6;
    public override List<Tile> legalTargetTiles => Board.instance.allTiles;

    public override void onInitialization()
    {
        toolTipInfos.Add(ToolTipInfo.arcaneSpell);
    }

    public override bool additionalCanBePlayedChecks()
    {
        foreach (Creature c in GameManager.Get().getAllCreaturesControlledBy(owner))
        {
            if (c.hasTag(Tag.Arcane))
                return true;
        }
        return false;
    }

    protected override List<Tag> getTags()
    {
        List<Tag> tags = new List<Tag>();
        tags.Add(Tag.Arcane);
        return tags;
    }

    protected override void doEffect(Tile t)
    {
        CardPicker.CreateAndQueue(owner.hand.getCardList(), 0, owner.hand.getCardList().Count, "Select cards to disard", owner, delegate (List<Card> cardList)
        {
            foreach (Card c in cardList)
                c.moveToCardPile(owner.graveyard, this);
        });
    }
}
