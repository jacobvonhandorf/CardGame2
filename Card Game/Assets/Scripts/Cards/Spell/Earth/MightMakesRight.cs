using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MightMakesRight : SpellCard, Effect
{
    public override int getCardId()
    {
        return 17;
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
        Deck deck = sourcePlayer.deck;
        CreatureCard cardToAdd = null;
        foreach (CreatureCard c in deck.getAllCardsWithType(CardType.Creature))
        {
            if (cardToAdd == null)
                cardToAdd = c;
            else
                if (cardToAdd.creature.getAttack() < c.creature.getAttack())
                    cardToAdd = c;
        }
        if (cardToAdd != null)
        {
            cardToAdd.creature.addAttack(1);
            cardToAdd.creature.addHealth(1);
            cardToAdd.moveToCardPile(sourcePlayer.hand, this);
        }
    }
}
