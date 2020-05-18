using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaLeyline : Structure
{
    public override bool canDeployFrom()
    {
        return true;
    }

    public override bool canWalkOn()
    {
        return false;
    }

    public override void onPlaced()
    {
        controller.increaseManaPerTurn(2);
    }

    public override void onRemoved()
    {
        controller.increaseManaPerTurn(-2);
    }

    public override List<Card.Tag> getTags()
    {
        List<Card.Tag> tags = new List<Card.Tag>();
        tags.Add(Card.Tag.Income);
        return tags;
    }

    public override Effect getEffect()
    {
        return null;
    }

    public override int getCardId()
    {
        return 30;
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
