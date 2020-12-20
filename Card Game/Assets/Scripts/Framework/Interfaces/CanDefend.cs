using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Damageable
{
    Transform transform { get; }
    Card SourceCard { get; }
    Player Controller { get; }
    Vector2 Coordinates { get; }
    void TakeDamage(int damage, Card source);
    void TriggerOnDefendEvents(object sender, OnDefendArgs args);
}
