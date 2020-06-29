using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemGiant : Creature
{
    public override int getCardId()
    {
        return 71;
    }

    public override int getStartingRange()
    {
        return 1;
    }

    public override void onCreation()
    {
        // add 1 attack and 2 hp for each gem in hand
        int numGemsInHand = controller.hand.getAllCardsWithTag(Card.Tag.Gem).Count;
        addAttack(numGemsInHand);
        addHealth(numGemsInHand * 2);
    }

    public override List<Keyword> getInitialKeywords()
    {
        return new List<Keyword>()
        {
            Keyword.deploy,
            Keyword.armored1,
            Keyword.defender2,
            Keyword.ward
        };
    }

    /* old code
    public override void onCreation()
    {
        int attackBonus = 0;
        int healthBonus = 0;
        foreach (Card c in controller.graveyard.getAllCardsWithTag(Card.Tag.Gem))
        {
            if (c.getCardId() == Crystal.CARD_ID)
                attackBonus += 2;
            else if (c.getCardId() == Garnet.CARD_ID)
                healthBonus += 2;
            else
            {
                attackBonus += 1;
                healthBonus += 1;
            }
        }
        addAttack(attackBonus);
        addHealth(healthBonus);
    }*/

}
