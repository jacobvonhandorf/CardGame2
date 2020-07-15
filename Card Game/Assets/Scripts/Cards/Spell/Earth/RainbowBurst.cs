using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowBurst : SpellCard, Effect
{
    public const int CARD_ID = 82;
    private const int DAMAGE = 5;
    private const int CARDS_DRAWN = 3;
    private const int CARDS_TO_SHUFFLE_BACK = 3;

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
        gemsNeededToShuffleBack = CARDS_TO_SHUFFLE_BACK;

        List<Card> pickableCards = sourcePlayer.hand.getAllCardsWithTag(Tag.Gem);
        CompoundQueueableCommand.Builder cmdBuilder = new CompoundQueueableCommand.Builder();
        for (int i = 0; i < CARDS_TO_SHUFFLE_BACK; i++)
        {
            QueueableCommand cmd = CardPicker.CreateCommand(pickableCards, 1, 1, "Select a gem to shuffle back. " + gemsNeededToShuffleBack + " remaining", sourcePlayer, delegate (List<Card> cardList)
            {
                pickableCards.RemoveAll(c => c.getCardId() == cardList[0].getCardId()); // remove already selected cards
                cardsToShuffleBack.Add(cardList[0]);
                gemsNeededToShuffleBack--;
            });
            cmdBuilder.addCommand(cmd);
        }
        cmdBuilder.addCommand(SingleTileTargetEffect.CreateCommand(GameManager.Get().getAllTilesWithCreatures(owner.getOppositePlayer(), false), delegate (Tile t)
        {
            foreach (Card c in cardsToShuffleBack)
                c.moveToCardPile(owner.deck, this);
            owner.deck.shuffle();
            t.creature.takeDamage(DAMAGE);
            owner.drawCards(CARDS_DRAWN);
        }));
        cmdBuilder.BuildAndQueue();
    }
}
