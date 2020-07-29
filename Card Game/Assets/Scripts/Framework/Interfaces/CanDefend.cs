using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Damageable
{
    Transform transform { get; }
    Card SourceCard { get; }
    void takeDamage(int damage, Card source);
    Vector2 getCoordinates();
    Player getController();
    void TriggerOnDefendEvents(object sender, OnDefendArgs args);
}
