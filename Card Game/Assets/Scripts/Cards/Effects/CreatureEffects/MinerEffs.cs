using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinerEffs : CreatureEffects
{
    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        List<Card> legalPicks = creature.controller.deck.getAllCardsWithTag(Card.Tag.Gem);
        if (legalPicks.Count < 1)
        {
            Toaster.instance.doToast("No gems for Miner");
            return;
        }
        CardPicker.CreateAndQueue(legalPicks, 1, 1, "Select a card to add to your hand", creature.controller, delegate (List<Card> cardList)
        {
            cardList[0].moveToCardPile(creature.controller.hand, card);
        });
    };
}
