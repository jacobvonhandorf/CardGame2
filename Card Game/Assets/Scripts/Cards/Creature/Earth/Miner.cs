using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : Creature
{
    public override int getCardId()
    {
        return 68;
    }

    public override int getStartingRange()
    {
        return 1;
    }

    public override void onCreation()
    {
        CardPicker.CreateAndQueue(controller.deck.getAllCardsWithTag(Card.Tag.Gem), 1, 1, "Select a card to add to your hand", controller, delegate (List<Card> cardList)
        {
            cardList[0].moveToCardPile(controller.hand, sourceCard);
        });
    }

    public override List<Keyword> getInitialKeywords()
    {
        return new List<Keyword>()
        {
            Keyword.deploy,
            Keyword.armored1
        };
    }

}
