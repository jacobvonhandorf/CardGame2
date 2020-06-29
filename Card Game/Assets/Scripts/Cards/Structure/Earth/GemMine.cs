using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemMine : Structure, Effect
{
    public const int CARD_ID = 74;

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

        List<Card> gemCards = controller.deck.getAllCardsWithTag(Card.Tag.Gem);
        if (gemCards.Count > 0)
            gemCards[Random.Range(0, gemCards.Count)].moveToCardPile(controller.hand, true);
        else
        {
            GameManager.Get().showToast("No gems left in deck");
            return;
        }
        /*
        foreach (Card c in controller.deck.getCardList())
        {
            if (c.hasTag(Card.Tag.Gem))
            {
                controller.hand.addCardByEffect(c);
                break;
            }
        }*/
        removeCounters(Counters.mine, 1);
        controller.subtractActions(1);
    }

    public override List<Keyword> getInitialKeywords()
    {
        return new List<Keyword>()
        {
            Keyword.deploy
        };
    }
}
