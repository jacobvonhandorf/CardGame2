using UnityEngine;
using System.Collections;

public interface IScriptCreatureCard : IScriptCard
{
    IScriptCreature Creature { get; }
    CounterController CounterController { get; }

    void ResetToBaseStats();
}
