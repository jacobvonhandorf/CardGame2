using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Card;

public class FairyNeophyte : Creature
{
    public override int getStartingRange()
    {
        return 1;
    }

    public override void onCreation()
    {
        GameManager.Get().queueCardPickerEffect(controller, controller.hand.getAllCardsWithType(Card.CardType.Creature), new EffReceiver(), 1, 1, false , "Select a card to give +1/+1");
    }

    private class EffReceiver : CanReceivePickedCards
    {
        public void receiveCardList(List<Card> cardList)
        {
            foreach(Card c in cardList)
            {
                (c as CreatureCard).creature.addAttack(1);
                (c as CreatureCard).creature.addHealth(1);
            }
        }
    }

    public override List<Card.Tag> getTags()
    {
        List<Tag> tags = new List<Tag>();
        tags.Add(Tag.Fairy);
        return tags;
    }

    public override int getCardId()
    {
        return 46;
    }

    public override List<Keyword> getInitialKeywords()
    {
        return new List<Keyword>()
        {
            Keyword.deploy
        };
    }

}
