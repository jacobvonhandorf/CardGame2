using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Card;

public class RainbowBurstEffs : SpellEffects
{
    private const int DAMAGE = 5;
    private const int CARDS_DRAWN = 3;
    private const int CARDS_TO_SHUFFLE_BACK = 3;

    public override List<Tile> ValidTiles => Board.instance.AllTiles;

    private List<int> selectedCardIds = new List<int>();
    private List<Card> cardsToShuffleBack = new List<Card>();
    private int gemsNeededToShuffleBack;
    public override void DoEffect(Tile t)
    {
        selectedCardIds.Clear();
        cardsToShuffleBack.Clear();
        gemsNeededToShuffleBack = CARDS_TO_SHUFFLE_BACK;

        List<Card> pickableCards = card.owner.Hand.GetAllCardsWithTag(Tag.Gem);
        CompoundQueueableCommand.Builder cmdBuilder = new CompoundQueueableCommand.Builder();
        for (int i = 0; i < CARDS_TO_SHUFFLE_BACK; i++)
        {
            IQueueableCommand cmd = CardPicker.CreateCommand(pickableCards, 1, 1, "Select a gem to shuffle back. " + gemsNeededToShuffleBack + " remaining", card.owner, delegate (List<Card> cardList)
            {
                pickableCards.RemoveAll(c => c.cardId == cardList[0].cardId); // remove already selected cards
                cardsToShuffleBack.Add(cardList[0]);
                gemsNeededToShuffleBack--;
            });
            cmdBuilder.AddCommand(cmd);
        }
        cmdBuilder.AddCommand(SingleTileTargetEffect.CreateCommand(GameManager.Get().getAllTilesWithCreatures(card.owner.OppositePlayer, false), delegate (Tile targetTile)
        {
            foreach (Card c in cardsToShuffleBack)
                c.MoveToCardPile(card.owner.Deck, card);
            card.owner.Deck.shuffle();
            targetTile.creature.TakeDamage(DAMAGE, card);
            card.owner.DrawCards(CARDS_DRAWN);
        }));
        cmdBuilder.BuildAndQueue();
    }

    public override bool CanBePlayed => additionalCanBePlayedChecks();
    public bool additionalCanBePlayedChecks()
    {
        int numUniqueGemsInHand = 0;
        List<int> usedIds = new List<int>();
        foreach (Card c in card.owner.Hand.CardList)
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
