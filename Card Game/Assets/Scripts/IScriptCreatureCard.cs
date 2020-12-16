using UnityEngine;
using System.Collections;

public interface IScriptCreatureCard : IScriptCard
{
    Creature Creature { get; }
    CounterController CounterController { get; }

    void ResetToBaseStats();
}
