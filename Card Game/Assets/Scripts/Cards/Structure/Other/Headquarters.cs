using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Headquarters : Structure
{
    public const int CARD_ID = 40;

    private HeroPower heroPower;

    public override int cardId => CARD_ID;

    public override bool canDeployFrom()
    {
        return true;
    }

    public override void onRemoved()
    {
        GameManager.Get().makePlayerLose(controller);
    }

    public override Effect getEffect()
    {
        return new HQEffect(heroPower);
    }

    public void setHeroPower(HeroPower heroPower)
    {
        this.heroPower = heroPower;
        statsScript.effectText.text = statsScript.effectText.text + "\n" + heroPower.getEffectText();
    }

    private class HQEffect : Effect
    {
        private HeroPower heroPower;

        public HQEffect(HeroPower heroPower)
        {
            this.heroPower = heroPower;
        }

        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            if (heroPower.canBeActivatedCheck(sourcePlayer))
                heroPower.activate(sourcePlayer);
        }
    }
}
