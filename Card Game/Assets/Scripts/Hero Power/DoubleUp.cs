using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleUp : HeroPower
{
    public void activate(Player controller)
    {
        throw new System.Exception("Not implemented");
        //GameManager.Get().queueCardPickerEffect(controller, controller.hand.getAllCardsWithType(Card.CardType.Creature), this, 1, 1, false, "Select a card to buff");
    }

    public bool canBeActivatedCheck(Player controller)
    {
        return controller.hand.getAllCardsWithType(Card.CardType.Creature).Count > 0;
    }

    public string getEffectText()
    {
        return "Double the cost of a creature card in your hand. Double that same creature's attack and health";
    }

    public void receiveCardList(List<Card> cardList)
    {
        foreach (Card c in cardList)
        {
            Creature creature = (c as CreatureCard).creature;
            creature.addAttack(creature.getAttack());
            creature.addHealth(creature.getHealth());
            c.setGoldCost(c.getGoldCost() * 2);
            c.setManaCost(c.getManaCost() * 2);
        }
    }
}
