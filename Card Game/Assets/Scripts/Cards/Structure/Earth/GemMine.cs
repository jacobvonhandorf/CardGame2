using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemMine : Structure, Effect
{
    public const int CARD_ID = 74;

    private bool effectUsedThisTurn = false;

    public override bool canDeployFrom()
    {
        return true;
    }

    public override bool canWalkOn()
    {
        return false;
    }

    public override int getCardId()
    {
        return CARD_ID;
    }

    public override void onPlaced()
    {
        addCounters(Counters.mine, 3);
    }

    public override Effect getEffect()
    {
        return this;
    }

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        if (hasCounter(Counters.mine) == 0)
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
            gemCards[Random.Range(0, gemCards.Count)].moveToCardPile(controller.hand, sourceCard);
            effectUsedThisTurn = true;
            removeCounters(Counters.mine, 1);
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

    public override List<Keyword> getInitialKeywords()
    {
        return new List<Keyword>()
        {
            Keyword.deploy
        };
    }
}
