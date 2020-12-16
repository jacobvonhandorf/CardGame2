using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IScriptPermanent
{
    Tile Tile { get; }
    Vector2 Coordinates { get; }
    IScriptPlayer Controller { get; set; }
    IScriptCard SourceCard { get; }
    int Health { get; set; }
    int BaseHealth { get; set; }
    CounterController Counters { get; }

    void AddKeyword(Keyword k);
    void RemoveKeyword(Keyword k);
    bool HasKeyword(Keyword k);
    void TakeDamage(int amount, Card source);
}
