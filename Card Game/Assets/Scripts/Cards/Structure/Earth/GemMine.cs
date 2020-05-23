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
        foreach (Card c in controller.deck.getCardList())
        {
            if (c.hasTag(Card.Tag.Gem))
            {
                controller.hand.addCardByEffect(c);
                break;
            }
        }
    }
}
