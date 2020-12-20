using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyNeophyteEffs : CreatureEffects
{
    public override EventHandler onDeploy => deployEff;

    private void deployEff(object sender, EventArgs e)
    {
        CardPicker.CreateAndQueue(creature.Controller.Hand.GetAllCardsWithType(CardType.Creature), 1, 1, "Select a card to give +1/+1", creature.Controller, delegate (List<Card> cardList)
        {
            (cardList[0] as CreatureCard).Creature.Health += 1;
            (cardList[0] as CreatureCard).Creature.BaseAttack += 1;
        });
    }
}
