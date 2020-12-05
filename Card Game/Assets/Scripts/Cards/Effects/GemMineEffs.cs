using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemMineEffs : StructureEffects
{
    private bool effectUsedThisTurn = false;

    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        structure.Counters.Add(CounterType.Mine, 3);
    };

    public override EmptyHandler activatedEffect => delegate ()
    {
        if (structure.Counters.AmountOf(CounterType.Mine) == 0)
        {
            GameManager.Get().ShowToast("You must have a mine counter on Gem Mine to activate its effect");
            return;
        }
        if (structure.Controller.GetActions() <= 0)
        {
            GameManager.Get().ShowToast("You do not have enough actions to use this effect");
            return;
        }
        if (effectUsedThisTurn)
        {
            GameManager.Get().ShowToast("You can only activate " + card.CardName + " once per turn.");
            return;
        }

        List<Card> gemCards = structure.Controller.deck.getAllCardsWithTag(Card.Tag.Gem);
        if (gemCards.Count > 0)
        {
            gemCards[UnityEngine.Random.Range(0, gemCards.Count)].MoveToCardPile(structure.Controller.hand, card);
            effectUsedThisTurn = true;
            structure.Counters.Remove(CounterType.Mine, 1);
            structure.Controller.subtractActions(1);
            GameEvents.E_TurnEnd += GameEvents_E_TurnEnd;
        }
        else
        {
            GameManager.Get().ShowToast("No gems left in deck");
            return;
        }
    };

    private void GameEvents_E_TurnEnd(object sender, EventArgs e)
    {
        effectUsedThisTurn = false;
        GameEvents.E_TurnEnd -= GameEvents_E_TurnEnd;
    }
}
