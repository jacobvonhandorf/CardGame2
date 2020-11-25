using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Damageable
{
    Transform transform { get; }
    Card SourceCard { get; }
    Player Controller { get; }
    void takeDamage(int damage, Card source);
    Vector2 getCoordinates();
    void TriggerOnDefendEvents(object sender, OnDefendArgs args);
}
