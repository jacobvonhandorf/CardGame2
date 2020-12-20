using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PityTheWeakEffs : SpellEffects
{
    public override List<Tile> ValidTiles => Board.Instance.AllTiles;

    public override void DoEffect(Tile t)
    {
        Card cardToAdd = null;
        foreach (Card c in Owner.Deck.GetAllCardsWithType(CardType.Creature))
        {
            if (cardToAdd == null)
                cardToAdd = c;
            else if ((c as CreatureCard).Creature.AttackStat < (cardToAdd as CreatureCard).Creature.AttackStat)
                cardToAdd = c;
        }
        if (cardToAdd == null)
        {
            Toaster.Instance.DoToast("No targets for " + card.CardName);
            return;
        }
        cardToAdd.MoveToCardPile(Owner.Hand, card);
    }
}
