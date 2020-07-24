using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "New Creature Data", menuName = "Card Data/Creature Data")]
public class CreatureCardData : CardData
{
    public int health;
    public int attack;
    public int movement = 3;
    public int range = 1;
    public CreatureEffects effects;
}
