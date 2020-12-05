using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Creature Data", menuName = "Card Data/Creature Data")]
public class CreatureCardData : CardData, IHaveReadableStats
{
    public int health;
    public int attack;
    public int movement = 3;
    public int range = 1;
    public CreatureEffects effects;

    public override IHaveReadableStats ReadableStats => this;
    public UnityEvent E_OnStatChange => null;
    public override string CardTypeString => "Creature";
    public Dictionary<StatType, object> StatList => throw new NotImplementedException();

    public bool TryGetValue(StatType statType, out object value)
    {
        switch (statType)
        {
            case StatType.BaseGoldCost: value = goldCost; break;
            case StatType.GoldCost: value = goldCost; break;
            case StatType.BaseManaCost: value = -1; break;
            case StatType.ManaCost: value = -1; break;
            case StatType.Health: value = health; break;
            case StatType.BaseHealth: value = health; break;
            case StatType.Attack: value = attack; break;
            case StatType.BaseAttack: value = attack; break;
            case StatType.Movement: value = movement; break;
            case StatType.BaseMovement: value = movement; break;
            case StatType.Range: value = range; break;
            case StatType.BaseRange: value = range; break;
            case StatType.Name: value = cardName; break;
            case StatType.TypeText: value = TypeText; break;
            case StatType.EffectText: value = effectText; break;
            case StatType.Art: value = art; break;
            case StatType.ElementalId: value = elementalIdentity; break;
            default:
                value = default;
                Debug.LogError("Unexpected card type");
                break;
        }
        return true;
    }
}
