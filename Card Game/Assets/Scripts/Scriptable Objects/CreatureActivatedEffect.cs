using UnityEditor;
using UnityEngine;

public abstract class CreatureActivatedEffect : MonoBehaviour
{
    public abstract void activate(Creature creature);
}
