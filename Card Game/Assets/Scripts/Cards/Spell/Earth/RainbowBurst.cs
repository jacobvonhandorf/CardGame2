using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowBurst : SpellCard
{
    public const int CARD_ID = 82;
    private const int DAMAGE = 5;
    private const int CARDS_DRAWN = 3;
    private const int CARDS_TO_SHUFFLE_BACK = 3;
    public override int cardId => CARD_ID;
    public override List<Tile> legalTargetTiles => Board.instance.allTiles;

    public override bool additionalCanBePlayedChecks()
    {
        int numUniqueGemsInHand = 0;
        List<int> usedIds = new List<int>();
        foreach (Card c in owner.hand.getCardList())
        {
            if (c.hasTag(Tag.Gem) && !usedIds.Contains(c.cardId))
            {
                usedIds.Add(c.cardId);
                numUniqueGemsInHand++;
            }
        }
        return numUniqueGemsInHand >= 5;
    }

    private List<int> selectedCardIds = new List<int>();
    private List<Card> cardsToShuffleBack = new List<Card>();
    private int gemsNeededToShuffleBack;
    protected override void doEffect(Tile t)
    {
        selectedCardIds.Clear();
        cardsToShuffleBack.Clear();
        gemsNeededToShuffleBack = CARDS_TO_SHUFFLE_BACK;

        List<Card> pickableCards = owner.hand.getAllCardsWithTag(Tag.Gem);
        CompoundQueueableCommand.Builder cmdBuilder = new CompoundQueueableCommand.Builder();
        for (int i = 0; i < CARDS_TO_SHUFFLE_BACK; i++)
        {
            QueueableCommand cmd = CardPicker.CreateCommand(pickableCards, 1, 1, "Select a gem to shuffle back. " + gemsNeededToShuffleBack + " remaining", owner, delegate (List<Card> cardList)
            {
                pickableCards.RemoveAll(c => c.cardId == cardList[0].cardId); // remove already selected cards
                cardsToShuffleBack.Add(cardList[0]);
                gemsNeededToShuffleBack--;
            });
            cmdBuilder.addCommand(cmd);
        }
        cmdBuilder.addCommand(SingleTileTargetEffect.CreateCommand(GameManager.Get().getAllTilesWithCreatures(owner.getOppositePlayer(), false), delegate (Tile targetTile)
        {
            foreach (Card c in cardsToShuffleBack)
                c.moveToCardPile(owner.deck, this);
            owner.deck.shuffle();
            targetTile.creature.takeDamage(DAMAGE, this);
            owner.drawCards(CARDS_DRAWN);
        }));
        cmdBuilder.BuildAndQueue();
    }
}
