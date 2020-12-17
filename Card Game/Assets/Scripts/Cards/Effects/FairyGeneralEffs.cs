using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyGeneralEffs : CreatureEffects
{
    public override EmptyHandler onInitilization => delegate ()
    {
        //GameEvents.E_CreaturePlayed += GameEvents_E_CreaturePlayed;
    };
    /*
    private void GameEvents_E_CreaturePlayed(object sender, GameEvents.CreaturePlayedArgs e)
    {
        if (enabled && e.creature.Controller == creature.Controller)
        {
            creature.AttackStat += 1;
            creature.Health += 1;
        }
    }
    */
}
