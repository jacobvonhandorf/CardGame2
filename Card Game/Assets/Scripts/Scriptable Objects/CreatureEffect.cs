using UnityEditor;
using UnityEngine;

public abstract class CreatureEffect
{
    protected Creature creature;
    protected CreatureCard card;

    public void doEffect(object sender)
    {
        activate();
    }
    protected abstract void activate();
}
