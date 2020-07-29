using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemMine : Structure, Effect
{
    public const int CARD_ID = 74;

    private bool effectUsedThisTurn = false;

    public override int cardId => CARD_ID;

    public override void onPlaced()
    {
        Counters.add(CounterType.Mine, 3);
    }

    public override Effect getEffect()
    {
        return this;
    }

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        if (Counters.amountOf(CounterType.Mine) == 0)
        {
            GameManager.Get().showToast("You must have a mine counter on Gem Mine to activate its effect");
            return;
        }
        if (controller.GetActions() <= 0)
        {
            GameManager.Get().showToast("You do not have enough actions to use this effect");
            return;
        }
        if (effectUsedThisTurn)
        {
            GameManager.Get().showToast("You can only activate " + cardName + " once per turn.");
            return;
        }

        List<Card> gemCards = controller.deck.getAllCardsWithTag(Card.Tag.Gem);
        if (gemCards.Count > 0)
        {
            gemCards[Random.Range(0, gemCards.Count)].moveToCardPile(controller.hand, SourceCard);
            effectUsedThisTurn = true;
            Counters.remove(CounterType.Mine, 1);
            controller.subtractActions(1);
        }
        else
        {
            GameManager.Get().showToast("No gems left in deck");
            return;
        }
    }

    public override void onTurnStart()
    {
        effectUsedThisTurn = false;
    }
}
