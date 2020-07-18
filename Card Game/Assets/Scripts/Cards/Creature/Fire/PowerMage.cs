using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerMage : Creature
{
    public const int CARD_ID = 80;
    public override int cardId => CARD_ID;
    public override List<Card.Tag> getTags() => new List<Card.Tag>() { Card.Tag.Arcane };
    public override List<Keyword> getInitialKeywords() => new List<Keyword>() { Keyword.deploy };

    private static List<Card> cardReferences = new List<Card>();

    public override void onInitialization()
    {
        if (cardReferences.Count == 0)
        {
            cardReferences.Add(GameManager.Get().createCardById(PowerDraw.CARD_ID, controller));
            cardReferences.Add(GameManager.Get().createCardById(PowerBlast.CARD_ID, controller));
            cardReferences.Add(GameManager.Get().createCardById(Inferno.CARD_ID, controller));
            foreach (Card c in cardReferences)
                c.removeGraphicsAndCollidersFromScene();
        }
        E_OnDeployed += PowerMage_E_OnDeployed;
    }

    private void PowerMage_E_OnDeployed(object sender, System.EventArgs e)
    {
        CardPicker.CreateAndQueue(cardReferences, 1, 1, "Select a card to add to your hand", controller, delegate (List<Card> cardList)
        {
            GameManager.Get().createCardById(cardList[0].cardId, controller).moveToCardPile(controller.hand, sourceCard);
        });
    }

    private void OnDestroy() // clear the list when game ends. 
    {
        cardReferences.Clear();
    }
}
