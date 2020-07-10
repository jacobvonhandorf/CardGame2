using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents
{
    // on spell cast
    public static event EventHandler<SpellCastEventArgs> E_SpellCast;
    public class SpellCastEventArgs : EventArgs { public SpellCard spell { get; set; } }
    public delegate void SpellCastEventHandler(SpellCastEventArgs e);
    public static void TriggerSpellCastEvents(object sender, SpellCastEventArgs args) { if (E_SpellCast != null) E_SpellCast.Invoke(sender, args); }

    // on creature death TODO INVOKE
    public static event EventHandler<CreatureDeathEventArgs> E_CreatureDeath;
    public class CreatureDeathEventArgs : EventArgs { public Creature creature { get; set; } }
    public delegate void CreatureDeathEventHandler(CreatureDeathEventArgs e);
    public static void TriggerCreatureDeathEvents(object sender, CreatureDeathEventArgs args) { if (E_CreatureDeath != null) E_CreatureDeath.Invoke(sender, args); }

    // on turn start
    public static event EventHandler E_TurnStart;
    public static void TriggerTurnStartEvents(object sender) { if (E_TurnStart != null) E_TurnStart.Invoke(sender, EventArgs.Empty); }

    // on turn end TODO INVOKE
    public static event EventHandler E_TurnEnd;
    public static void TriggerTurnEndEvents(object sender) { if (E_TurnEnd != null) E_TurnEnd.Invoke(sender, EventArgs.Empty); }


    // to add
    // creature etb
    // creature move
}
