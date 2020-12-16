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
            Toaster.Instance.DoToast("You must have a mine counter on Gem Mine to activate its effect");
            return;
        }
        if (Structure.Controller.Actions <= 0)
        {
            Toaster.Instance.DoToast("You do not have enough actions to use this effect");
            return;
        }
        if (effectUsedThisTurn)
        {
            Toaster.Instance.DoToast("You can only activate " + Card.CardName + " once per turn.");
            return;
        }

        List<Card> gemCards = Structure.Controller.Deck.GetAllCardsWithTag(Tag.Gem);
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
            Toaster.Instance.DoToast("No gems left in deck");
            return;
        }
    };

    private void GameEvents_E_TurnEnd(object sender, EventArgs e)
    {
        effectUsedThisTurn = false;
        GameEvents.E_TurnEnd -= GameEvents_E_TurnEnd;
    }
}
