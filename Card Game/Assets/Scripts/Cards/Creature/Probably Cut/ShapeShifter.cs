using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeShifter : Creature, Effect, OptionBoxHandler
{
    private const string ATTACK = "+1 Attack";
    private const string HEALTH = "+2 Health";

    public override int getCardId()
    {
        return 55;
    }

    public override int getStartingRange()
    {
        return 1;
    }

    public override List<Card.Tag> getTags()
    {
        return new List<Card.Tag>() { Card.Tag.Arcane };
    }

    public override void onAnySpellCast(SpellCard spell)
    {
        if (sourceCard.isCreature && spell.owner == controller)
        {
            addCounters(Counters.arcane, 1);
        }
    }

    public override Effect getEffect()
    {
        return this;
    }

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        if (hasCounter(Counters.arcane) < 2)
        {
            GameManager.Get().showToast("Shapeshifter needs 2 arcane counters to activate its effect");
            return;
        }

        List<string> options = new List<string>()
        {
            HEALTH,
            ATTACK
        };
        GameManager.Get().queueOptionSelectBoxEffect(options, this, "Would you like to increase attack or health?", true, controller);
    }

    public void receiveOptionBoxSelection(int selectedOptionIndex, string selectedOption)
    {
        if (selectedOption == ATTACK)
            addAttack(1);
        else
            addHealth(2);
        removeCounters(Counters.arcane, 2);
    }

    /* old code
    public override void onCreation()
    {
        List<string> options = new List<string>();
        options.Add("Attack");
        options.Add("Health");
        //GameManager.Get().queueOptionSelectBoxEffect(options, new MyOptionHandler(this), "Which stat to buff on " + cardName + "?");
    }

    private class MyOptionHandler : OptionBoxHandler
    {
        private Creature creature;

        public MyOptionHandler(Creature sourceCreature)
        {
            creature = sourceCreature;
        }

        public void receiveOptionBoxSelection(int selectedOptionIndex, string selectedOption)
        {
            if (selectedOptionIndex == 0) // attack
            {
                creature.addAttack(2);
            }
            else // health
            {
                creature.addHealth(2);
            }
        }
    }*/
}
