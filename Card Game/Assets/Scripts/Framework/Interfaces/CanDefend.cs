using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Creature;

public interface Damageable
{
    void takeDamage(int damage);
    Transform getRootTransform();
    Vector2 getCoordinates();
    Card getSourceCard();
    Player getController();
    void TriggerOnDefendEvents(object sender, OnDefendArgs args);
}
