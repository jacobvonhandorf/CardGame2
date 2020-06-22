using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeShifter : Creature
{
    public override int getCardId()
    {
        return 55;
    }

    public override int getStartingRange()
    {
        return 1;
    }

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
    }
}
