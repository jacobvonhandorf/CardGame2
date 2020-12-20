using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEvents
{
    public static void clearEvents()
    {
        E_CreatureMoved = null;
    }
    public static UnityEvent<IScriptCard> E_SpellCast { get; } = new ScriptCardEvent();
    public static UnityEvent<IScriptCreature> E_CreaturePlayed { get; } = new ScriptCreatureEvent();
    public static UnityEvent<IScriptCreature> E_CreatureDeath { get; } = new ScriptCreatureEvent();
    public static UnityEvent E_TurnStart { get; } = new UnityEvent();
    public static UnityEvent E_TurnEnd { get; } = new UnityEvent();
    #region OnCreatureMoved
    public static event EventHandler<CreatureMovedArgs> E_CreatureMoved;
    public class CreatureMovedArgs : EventArgs {
        public Creature creature { get; set; }
        public Card source { get; set; }
    }
    public delegate void CreatureMovedEventHandler(CreatureMovedArgs e);
    public static void TriggerMovedEvents(object sender, CreatureMovedArgs args) { E_CreatureMoved?.Invoke(sender, args); }
    #endregion
}
