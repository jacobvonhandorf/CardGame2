using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyMotivator : Creature
{
    public override int cardId => 48;

    public override int getStartingRange()
    {
        return 1;
    }

    public override void onCreation()
    {
        foreach (Card c in controller.hand.getAllCardsWithType(Card.CardType.Creature))
        {
            (c as CreatureCard).creature.addHealth(1);
            (c as CreatureCard).creature.addAttack(1);
        }
    }

    public override List<Card.Tag> getTags() => new List<Card.Tag>() { Card.Tag.Fairy };
    public override List<Keyword> getInitialKeywords() => new List<Keyword>() { Keyword.deploy };
}
