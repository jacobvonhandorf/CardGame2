using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Card;

public class RainbowBurstEffs : SpellEffects
{
    private const int DAMAGE = 5;
    private const int CARDS_DRAWN = 3;
    private const int CARDS_TO_SHUFFLE_BACK = 3;

    public override List<Tile> validTiles => Board.instance.allTiles;

    private List<int> selectedCardIds = new List<int>();
    private List<Card> cardsToShuffleBack = new List<Card>();
    private int gemsNeededToShuffleBack;
    public override void doEffect(Tile t)
    {
        selectedCardIds.Clear();
        cardsToShuffleBack.Clear();
        gemsNeededToShuffleBack = CARDS_TO_SHUFFLE_BACK;

        List<Card> pickableCards = card.owner.hand.getAllCardsWithTag(Tag.Gem);
        CompoundQueueableCommand.Builder cmdBuilder = new CompoundQueueableCommand.Builder();
        for (int i = 0; i < CARDS_TO_SHUFFLE_BACK; i++)
        {
            QueueableCommand cmd = CardPicker.CreateCommand(pickableCards, 1, 1, "Select a gem to shuffle back. " + gemsNeededToShuffleBack + " remaining", card.owner, delegate (List<Card> cardList)
            {
                pickableCards.RemoveAll(c => c.cardId == cardList[0].cardId); // remove already selected cards
                cardsToShuffleBack.Add(cardList[0]);
                gemsNeededToShuffleBack--;
            });
            cmdBuilder.addCommand(cmd);
        }
        cmdBuilder.addCommand(SingleTileTargetEffect.CreateCommand(GameManager.Get().getAllTilesWithCreatures(card.owner.getOppositePlayer(), false), delegate (Tile targetTile)
        {
            foreach (Card c in cardsToShuffleBack)
                c.MoveToCardPile(card.owner.deck, card);
            card.owner.deck.shuffle();
            targetTile.creature.TakeDamage(DAMAGE, card);
            card.owner.DrawCards(CARDS_DRAWN);
        }));
        cmdBuilder.BuildAndQueue();
    }

    public override bool canBePlayed => additionalCanBePlayedChecks();
    public bool additionalCanBePlayedChecks()
    {
        int numUniqueGemsInHand = 0;
        List<int> usedIds = new List<int>();
        foreach (Card c in card.owner.hand.getCardList())
        {
            if (c.Tags.Contains(Tag.Gem) && !usedIds.Contains(c.cardId))
            {
                usedIds.Add(c.cardId);
                numUniqueGemsInHand++;
            }
        }
        return numUniqueGemsInHand >= 5;
    }
}
