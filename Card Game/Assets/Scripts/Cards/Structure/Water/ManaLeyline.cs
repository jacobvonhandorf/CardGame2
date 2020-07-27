using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaLeyline : Structure, Effect
{
    public override int cardId => 30;

    public override bool canDeployFrom()
    {
        return true;
    }

    public override void onPlaced()
    {
        controller.increaseManaPerTurn(2);
    }

    public override void onRemoved()
    {
        controller.increaseManaPerTurn(-2);
    }

    public override List<Card.Tag> getTags() => new List<Card.Tag>() { Card.Tag.Income };

    public override Effect getEffect()
    {
        return this;
    }

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        if (controller.GetActions() <= 0)
        {
            GameManager.Get().showToast("You do not have enough actions to use this ability");
            return;
        }

        controller.addMana(1);
        controller.subtractActions(1);
    }

    private class ManaLeylineEffect : Effect
    {
        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            if (sourcePlayer.GetActions() > 0)
            {
                sourcePlayer.addMana(1);
                sourcePlayer.subtractActions(1);
            }
            else
                GameManager.Get().showToast("You do not have enough Actions to activate Mana Leyline's effect");
        }
    }
}
