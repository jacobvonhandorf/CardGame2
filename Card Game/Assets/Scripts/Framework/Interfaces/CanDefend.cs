using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Damageable
{
    void onDefend();
    void takeDamage(int damage);
    Transform getRootTransform();
    Vector2 getCoordinates();
    Card getSourceCard();
    Player getController();
}
