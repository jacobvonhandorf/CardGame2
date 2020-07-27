using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemMineEffs : StructureEffects
{
    private bool effectUsedThisTurn = false;

    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        structure.addCounters(Counters.mine, 3);
    };

    public override EmptyHandler activatedEffect => delegate ()
    {
        if (structure.hasCounter(Counters.mine) == 0)
        {
            GameManager.Get().showToast("You must have a mine counter on Gem Mine to activate its effect");
            return;
        }
        if (structure.controller.GetActions() <= 0)
        {
            GameManager.Get().showToast("You do not have enough actions to use this effect");
            return;
        }
        if (effectUsedThisTurn)
        {
            GameManager.Get().showToast("You can only activate " + card.cardName + " once per turn.");
            return;
        }

        List<Card> gemCards = structure.controller.deck.getAllCardsWithTag(Card.Tag.Gem);
        if (gemCards.Count > 0)
        {
            gemCards[UnityEngine.Random.Range(0, gemCards.Count)].moveToCardPile(structure.controller.hand, card);
            effectUsedThisTurn = true;
            structure.removeCounters(Counters.mine, 1);
            structure.controller.subtractActions(1);
            GameEvents.E_TurnEnd += GameEvents_E_TurnEnd;
        }
        else
        {
            GameManager.Get().showToast("No gems left in deck");
            return;
        }
    };

    private void GameEvents_E_TurnEnd(object sender, EventArgs e)
    {
        effectUsedThisTurn = false;
        GameEvents.E_TurnEnd -= GameEvents_E_TurnEnd;
    }
}
