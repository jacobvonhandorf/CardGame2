using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaManipulator : Structure
{
    public override bool canDeployFrom()
    {
        return true;
    }

    public override bool canWalkOn()
    {
        return true;
    }

    public override int getCardId()
    {
        return 42;
    }

    public override void onAnySpellCast(SpellCard spell)
    {
        if (sourceCard.isStructure)
            controller.addMana(1);
    }
}
