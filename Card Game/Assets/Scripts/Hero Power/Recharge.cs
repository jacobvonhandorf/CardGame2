using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recharge : HeroPower
{
    //private const int manaCost = 5;
    //private const int goldCost = 5;
    private const int cardsToShuffleBack = 10;
    private const int cardToDraw = 3;
    private const int manaGain = 7;
    private const int goldGain = 7;
    private const int totalCost = 12;

    private Player controller; // store controller so that cards can be shuffled back into their deck
    public void activate(Player controller)
    {
        this.controller = controller;

        //GameManager.Get().queueXPickerEffect(this, "Select how much gold you want to spend. You must spend " + totalCost + " total between gold and mana", 0, totalCost, false, controller);
    }

    public bool canBeActivatedCheck(Player controller)
    {
        if (controller.graveyard.getCardList().Count < cardsToShuffleBack)
            return false;
        int totalResources = 0;
        totalResources += controller.getMana();
        totalResources += controller.getGold();
        if (totalResources < totalCost)
            return false;

        return true;
    }

    public string getEffectText()
    {
        //string returnString = manaCost + " mana, " + goldCost + " gold, shuffle " + cardsToShuffleBack + " cards from your graveyard into your deck: ";
        //returnString += "Gain " + goldGain + " gold. Gain " + manaGain + " mana. Draw " + cardToDraw + " cards.";
        string returnString = totalCost + " gold/mana, shuffle " + cardsToShuffleBack + " cards from your graveyard into your deck: ";
        returnString += "Gain " + goldGain + " gold. Gain " + manaGain + " mana. Draw " + cardToDraw + " cards.";

        return returnString;
    }

    public void receiveCardList(List<Card> cardList)
    {
        foreach (Card c in cardList)
        {
            c.moveToCardPile(controller.deck, null);
        }
        controller.deck.shuffle();

        //controller.addMana(-manaCost);
        //controller.addGold(-goldCost);

        controller.addMana(manaGain);
        controller.addGold(goldGain);

        controller.drawCards(cardToDraw);
    }

    public void receiveXPick(int value)
    {
        throw new System.Exception("Not implemented");
        controller.addGold(-value);
        controller.addMana(-(totalCost - value));
        //GameManager.Get().queueCardPickerEffect(controller, controller.graveyard.getCardList(), this, cardsToShuffleBack, cardsToShuffleBack, true, "Select cards to shuffle back");
    }
}
