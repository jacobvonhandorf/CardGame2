using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellGrasp : SpellCard
{
    public const int CARD_ID = 3;
    public override int cardId => CARD_ID;
    public override List<Tile> legalTargetTiles => GameManager.Get().allTiles();

    public override void onInitialization()
    {
        toolTipInfos.Add(ToolTipInfo.arcaneSpell);
    }

    protected override void doEffect(Tile t)
    {
        CardPicker.CreateAndQueue(owner.deck.getAllCardsWithTag(Tag.Arcane), 1, 1, "Select a card to add to your hand", owner, delegate (List<Card> cardList)
        {
            cardList[0].moveToCardPile(owner.hand, this);
            foreach (Creature creature in GameManager.Get().getAllCreaturesControlledBy(owner))
            {
                if (creature.hasTag(Tag.Arcane))
                {
                    owner.addMana(1);
                    break;
                }
            }
        });
    }
}
