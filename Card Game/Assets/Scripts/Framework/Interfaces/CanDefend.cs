using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Damageable
{
    void takeDamage(int damage, Card source);
    Transform getRootTransform();
    Vector2 getCoordinates();
    Card getSourceCard();
    Player getController();
    void TriggerOnDefendEvents(object sender, OnDefendArgs args);
}
