using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyNeophyteEffs : CreatureEffects
{
    public override EventHandler onDeploy => deployEff;

    private void deployEff(object sender, EventArgs e)
    {
        CardPicker.CreateAndQueue(creature.controller.hand.getAllCardsWithType(Card.CardType.Creature), 1, 1, "Select a card to give +1/+1", creature.controller, delegate (List<Card> cardList)
        {
            (cardList[0] as CreatureCard).creature.Health += 1;
            (cardList[0] as CreatureCard).creature.BaseAttack += 1;
        });
    }
}
