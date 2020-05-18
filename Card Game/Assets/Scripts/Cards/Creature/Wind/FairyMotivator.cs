using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyMotivator : Creature
{
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

    public override List<Card.Tag> getTags()
    {
        List<Card.Tag> tags = new List<Card.Tag>();
        tags.Add(Card.Tag.Fairy);
        return tags;
    }

    public override int getCardId()
    {
        return 48;
    }
}
