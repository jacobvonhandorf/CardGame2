using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prospector : Creature
{
    public override int cardId => 67;
    public override List<Keyword> getInitialKeywords() => new List<Keyword>() { Keyword.deploy, Keyword.combatant };

    public override void onInitialization()
    {
        E_OnDeployed += Prospector_E_OnDeployed;
        E_OnAttack += Prospector_E_OnAttack;
        E_OnDefend += Prospector_E_OnDefend;
    }

    private void Prospector_E_OnDefend(object sender, OnDefendArgs e)
    {
        shuffleObsidianIntoDeck();
    }
    private void Prospector_E_OnAttack(object sender, OnAttackArgs e)
    {
        shuffleObsidianIntoDeck();
    }
    private void Prospector_E_OnDeployed(object sender, System.EventArgs e)
    {
        shuffleObsidianIntoDeck();
    }

    private void shuffleObsidianIntoDeck()
    {
        //create obsidian
        Card obsidian = GameManager.Get().createCardById(Obsidian.CARD_ID, controller);

        // add to deck
        obsidian.moveToCardPile(controller.deck, sourceCard);

        // shuffle deck
        controller.deck.shuffle();
    }

    /* old code
    public override void onAttack()
    {
        List<Card> deckList = controller.deck.getCardList();
        List<Card> revealedCards = new List<Card>();

        foreach (Card c in deckList)
        {
            if (revealedCards.Count < 5)
                revealedCards.Add(c);
            else
                break;
        }

        List<Card> pickableCards = new List<Card>();
        foreach (Card c in revealedCards)
        {
            if (c.hasTag(Card.Tag.Gem))
                pickableCards.Add(c);
        }
        if (pickableCards.Count > 0)
            GameManager.Get().queueCardPickerEffect(controller, pickableCards, this, 1, 1, false,  "Select a Gem to add to your hand");
        else
            GameManager.Get().showToast("Prospector found no Gems");
    }

    public void receiveCardList(List<Card> cardList)
    {
        foreach (Card c in cardList)
        {
            controller.hand.addCardByEffect(c);
        }
    }
    */

}
