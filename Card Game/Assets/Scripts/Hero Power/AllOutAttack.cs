using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllOutAttack : HeroPower
{
    private const int goldCost = 9;

    public void activate(Player controller)
    {
        List<Creature> creatures = controller.ControlledCreatures;
        foreach (Creature c in creatures)
        {
            c.hasDoneActionThisTurn = false;
            c.hasMovedThisTurn = false;
            c.UpdateHasActedIndicators();
        }

        controller.Actions += creatures.Count;
    }

    public bool canBeActivatedCheck(Player controller)
    {
        return controller.Gold >= goldCost;
    }

    public string getEffectText()
    {
        return goldCost + " gold: All creatures you control regain the ability to move and act this turn. Gain 1 action for each creature you control";
    }
}
