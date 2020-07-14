using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Card;

public class FairyNeophyte : Creature
{
    public const int CARD_ID = 46;

    public override int getStartingRange()
    {
        return 1;
    }

    public override void onCreation()
    {
        CardPicker.CreateAndQueue(controller.hand.getAllCardsWithType(CardType.Creature), 1, 1, "Select a card to give +1/+1", controller, delegate (List<Card> cardList)
        {
            (cardList[0] as CreatureCard).creature.addAttack(1);
            (cardList[0] as CreatureCard).creature.addHealth(1);
        });
    }

    public override List<Tag> getTags()
    {
        List<Tag> tags = new List<Tag>();
        tags.Add(Tag.Fairy);
        return tags;
    }

    public override int getCardId()
    {
        return CARD_ID;
    }

    public override List<Keyword> getInitialKeywords()
    {
        return new List<Keyword>()
        {
            Keyword.deploy
        };
    }

}
