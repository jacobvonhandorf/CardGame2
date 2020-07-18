using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : Creature
{
    public override int cardId => 68;
    public override List<Keyword> getInitialKeywords() => new List<Keyword>() { Keyword.deploy, Keyword.armored1 };

    public override void onInitialization()
    {
        E_OnDeployed += Miner_E_OnDeployed;
    }

    private void Miner_E_OnDeployed(object sender, System.EventArgs e)
    {
        CardPicker.CreateAndQueue(controller.deck.getAllCardsWithTag(Card.Tag.Gem), 1, 1, "Select a card to add to your hand", controller, delegate (List<Card> cardList)
        {
            cardList[0].moveToCardPile(controller.hand, sourceCard);
        });
    }
}
