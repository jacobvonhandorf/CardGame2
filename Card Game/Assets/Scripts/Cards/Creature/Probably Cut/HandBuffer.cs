using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandBuffer : Creature
{
    public override int getCardId()
    {
        return 60;
    }

    /*
    public override void onCreation()
    {
        if (controller.hand.getAllCardsWithType(Card.CardType.Creature).Count > 0)
            GameManager.Get().queueCardPickerEffect(controller, controller.hand.getAllCardsWithType(Card.CardType.Creature), new OnCreationEffect(), 1, 1, false, "Select a creature card to give +3/+3");
        else
            Debug.Log("No valid targets for hand buffer");
    }

    private class OnCreationEffect : CanReceivePickedCards
    {
        public void receiveCardList(List<Card> cardList)
        {
            if (cardList.Count > 0)
            {
                (cardList[0] as CreatureCard).creature.addHealth(3);
                (cardList[0] as CreatureCard).creature.addAttack(3);
            }
        }
    }
    */
}
