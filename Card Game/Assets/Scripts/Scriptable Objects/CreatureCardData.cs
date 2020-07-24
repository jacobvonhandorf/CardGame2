using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Creature Data", menuName = "ScriptableObjects/Creature Data")]
public class CreatureCardData : CardData
{
    public int health;
    public int attack;
    public int movement = 3;
    public int range = 1;
    public List<CreatureActivatedEffect> activatedEffects;
    public OnCreatureInitialization onInitilization;
}
