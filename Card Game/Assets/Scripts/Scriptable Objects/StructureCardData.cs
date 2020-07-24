using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Structure", menuName = "Card Data/Structure Data")]
public class StructureCardData : CardData
{
    public int health;
    public StructureEffects effects;
}
