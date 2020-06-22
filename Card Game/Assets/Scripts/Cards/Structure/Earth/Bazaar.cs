using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bazaar : Structure
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
        controller.increaseGoldPerTurn(2);
    }

    public override void onRemoved()
    {
        controller.increaseGoldPerTurn(-2);
    }

    public override Effect getEffect()
    {
        return new BazaarEff();
    }

    private class BazaarEff : Effect
    {
        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            if (sourcePlayer.GetActions() <= 0)
            {
                GameManager.Get().showToast("You do not have enough actions to use this structure");
                return;
            }
            sourcePlayer.addGold(1);
            sourcePlayer.addActions(-1);
        }
    }

    public override List<Card.Tag> getTags()
    {
        List<Card.Tag> tags = new List<Card.Tag>();
        tags.Add(Card.Tag.Income);
        return tags;
    }

    public override int getCardId()
    {
        return 44;
    }
}
