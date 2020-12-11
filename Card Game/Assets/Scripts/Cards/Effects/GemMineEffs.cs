using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemMineEffs : StructureEffects
{
    private bool effectUsedThisTurn = false;

    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        Structure.Counters.Add(CounterType.Mine, 3);
    };

    public override EmptyHandler activatedEffect => delegate ()
    {
        if (Structure.Counters.AmountOf(CounterType.Mine) == 0)
        {
            GameManager.Get().ShowToast("You must have a mine counter on Gem Mine to activate its effect");
            return;
        }
        if (Structure.Controller.Actions <= 0)
        {
            GameManager.Get().ShowToast("You do not have enough actions to use this effect");
            return;
        }
        if (effectUsedThisTurn)
        {
            GameManager.Get().ShowToast("You can only activate " + Card.CardName + " once per turn.");
            return;
        }

        List<Card> gemCards = Structure.Controller.Deck.GetAllCardsWithTag(Card.Tag.Gem);
        if (gemCards.Count > 0)
        {
            gemCards[UnityEngine.Random.Range(0, gemCards.Count)].MoveToCardPile(Structure.Controller.Hand, Card);
            effectUsedThisTurn = true;
            Structure.Counters.Remove(CounterType.Mine, 1);
            Structure.Controller.Actions -= 1;
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
