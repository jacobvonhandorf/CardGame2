using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PityTheWeak : SpellCard, Effect
{
    public override int getCardId()
    {
        return 7;
    }

    public override List<Tile> getLegalTargetTiles()
    {
        return GameManager.Get().allTiles();
    }

    protected override Effect getEffect()
    {
        return this;
    }

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        CreatureCard cardToAdd = null;
        foreach (CreatureCard c in sourcePlayer.deck.getAllCardsWithType(CardType.Creature))
        {
            if (cardToAdd == null)
                cardToAdd = c;
            else
                if (cardToAdd.creature.getAttack() < c.creature.getAttack())
                cardToAdd = c;
        }
        if (cardToAdd != null)
        {
            cardToAdd.setGoldCost(cardToAdd.getGoldCost() - 1);
            cardToAdd.moveToCardPile(sourcePlayer.hand, this);
        }
        else
        {
            Debug.Log("Card to add is null");
        }
    }
}
