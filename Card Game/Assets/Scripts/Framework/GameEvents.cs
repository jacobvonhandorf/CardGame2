using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents
{
    #region OnSpellCast
    public static event EventHandler<SpellCastArgs> E_SpellCast;
    public class SpellCastArgs : EventArgs { public SpellCard spell { get; set; } }
    public delegate void SpellCastEventHandler(SpellCastArgs e);
    public static void TriggerSpellCastEvents(object sender, SpellCastArgs args) { if (E_SpellCast != null) E_SpellCast.Invoke(sender, args); }
    #endregion
    #region OnCreatureDeath
    public static event EventHandler<CreatureDeathArgs> E_CreatureDeath;
    public class CreatureDeathArgs : EventArgs { public Creature creature { get; set; } }
    public delegate void CreatureDeathEventHandler(CreatureDeathArgs e);
    public static void TriggerCreatureDeathEvents(object sender, CreatureDeathArgs args) { if (E_CreatureDeath != null) E_CreatureDeath.Invoke(sender, args); }
    #endregion
    #region OnTurnStart
    public static event EventHandler E_TurnStart;
    public static void TriggerTurnStartEvents(object sender) { if (E_TurnStart != null) E_TurnStart.Invoke(sender, EventArgs.Empty); }
    #endregion
    #region OnTurnEnd
    public static event EventHandler E_TurnEnd;
    public static void TriggerTurnEndEvents(object sender) { if (E_TurnEnd != null) E_TurnEnd.Invoke(sender, EventArgs.Empty); }
    #endregion
    #region OnCreatureETB
    public static event EventHandler<CreaturePlayedArgs> E_CreaturePlayed;
    public class CreaturePlayedArgs : EventArgs { public Creature creature { get; set; } }
    public delegate void CreaturePlayedEventHandler(CreaturePlayedArgs e);
    public static void TriggerCreaturePlayedEvents(object sender, CreaturePlayedArgs args) { if (E_CreatureDeath != null) E_CreaturePlayed.Invoke(sender, args); }
    #endregion
    #region OnCreatureMoved
    // TODO invoke
    public static event EventHandler<CreatureMovedArgs> E_CreatureMoved;
    public class CreatureMovedArgs : EventArgs {
        public Creature creature { get; set; }
        public Card source { get; set; }
    }
    public delegate void CreatureMovedEventHandler(CreatureMovedArgs e);
    public static void TriggerMovedEvents(object sender, CreatureMovedArgs args) { if (E_CreatureDeath != null) E_CreaturePlayed.Invoke(sender, args); }
    #endregion
}
