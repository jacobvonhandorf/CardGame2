using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerMage : Creature
{
    public const int CARD_ID = 80;

    public override int getCardId()
    {
        return CARD_ID;
    }

    public override int getStartingRange()
    {
        return 1;
    }

    private static List<Card> cardReferences = new List<Card>();
    public override void onInitialization()
    {
        if (cardReferences.Count == 0)
        {
            cardReferences.Add(GameManager.Get().createCardById(PowerDraw.CARD_ID, controller));
            cardReferences.Add(GameManager.Get().createCardById(PowerBlast.CARD_ID, controller));
            cardReferences.Add(GameManager.Get().createCardById(Inferno.CARD_ID, controller));
        }
    }
    private void OnDestroy() // clear the list when game ends. 
    {
        cardReferences.Clear();
    }

    public override void onCreation()
    {
        //GameManager.Get().queueCardPickerEffect(controller, cardReferences, this, 1, 1, true, "Select a card to add to your hand");
        CardPicker.CreateAndQueue(cardReferences, 1, 1, "Select a card to add to your hand", controller, delegate (List<Card> cardList)
        {
            GameManager.Get().createCardById(cardList[0].getCardId(), controller).moveToCardPile(controller.hand, sourceCard);
        });
    }

    public override List<Card.Tag> getTags()
    {
        return new List<Card.Tag>() { Card.Tag.Arcane };
    }

    public override List<Keyword> getInitialKeywords()
    {
        return new List<Keyword>() { Keyword.deploy };
    }
}
