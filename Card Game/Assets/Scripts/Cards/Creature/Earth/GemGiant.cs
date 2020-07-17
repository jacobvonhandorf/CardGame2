using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemGiant : Creature
{
    public override int cardId => 71;

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
}
