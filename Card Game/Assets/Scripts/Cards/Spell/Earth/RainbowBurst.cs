using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowBurst : SpellCard, Effect, CanReceivePickedCards
{
    public const int CARD_ID = 82;
    private const int DAMAGE = 5;
    private const int CARDS_DRAWN = 3;

    public override int getCardId()
    {
        return CARD_ID;
    }

    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().allTiles();
    }

    public override bool additionalCanBePlayedChecks()
    {
        int numUniqueGemsInHand = 0;
        List<int> usedIds = new List<int>();
        foreach (Card c in owner.hand.getCardList())
        {
            if (c.hasTag(Tag.Gem) && !usedIds.Contains(c.getCardId()))
            {
                usedIds.Add(c.getCardId());
                numUniqueGemsInHand++;
            }
        }
        return numUniqueGemsInHand >= 5;
    }

    protected override Effect getEffect()
    {
        return this;
    }

    private List<int> selectedCardIds = new List<int>();
    private List<Card> cardsToShuffleBack = new List<Card>();
    private int gemsNeededToShuffleBack;
    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        // reset values
        selectedCardIds.Clear();
        cardsToShuffleBack.Clear();
        gemsNeededToShuffleBack = 5;
        GameManager.Get().queueCardPickerEffect(sourcePlayer, sourcePlayer.hand.getAllCardsWithTag(Tag.Gem), this, 1, 1, true, "Select a gem to shuffle back. " + gemsNeededToShuffleBack + " remaining");
    }

    public void receiveCardList(List<Card> cardList)
    {
        Card selectedCard = cardList[0];
        selectedCardIds.Add(selectedCard.getCardId());
        cardsToShuffleBack.Add(selectedCard);
        gemsNeededToShuffleBack--;

        if (gemsNeededToShuffleBack == 0)
        {
            // proceed to next part of effect (select a creature to kill)
            PickCreatureEff eff = new PickCreatureEff();
            eff.sourceCard = this;
            GameManager.Get().setUpSingleTileTargetEffect(eff, owner, null, null, null, "Select a creature to deal " + DAMAGE + " to", true);
        }
        else
        {
            // select another card
            List<Card> pickableCards = owner.hand.getAllCardsWithTag(Tag.Gem);
            pickableCards.RemoveAll(c => selectedCardIds.Contains(c.getCardId())); // remove already selected cards

            GameManager.Get().queueCardPickerEffect(owner, pickableCards, this, 1, 1, true, "Select a gem to shuffle back. " + gemsNeededToShuffleBack + " remaining");
        }
    }

    private class PickCreatureEff : SingleTileTargetEffect
    {
        public RainbowBurst sourceCard;

        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            // all selections have been made so resolve all effects
            foreach (Card c in sourceCard.cardsToShuffleBack)
            {
                c.moveToCardPile(sourceCard.owner.deck, sourceCard);
            }
            sourceCard.owner.deck.shuffle();

            targetCreature.takeDamage(DAMAGE);
            sourcePlayer.drawCards(CARDS_DRAWN);
        }

        public bool canBeCancelled()
        {
            return false;
        }

        public List<Tile> getValidTargetTiles(Player sourcePlayer, Player oppositePlayer, Tile sourceTile)
        {
            return GameManager.Get().getAllTilesWithCreatures(oppositePlayer, false);
        }
    }
}
