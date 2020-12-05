using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Structure", menuName = "Card Data/Structure Data")]
public class StructureCardData : CardData, IHaveReadableStats
{
    public int health;
    public StructureEffects effects;

    public override IHaveReadableStats ReadableStats => this;

    public override string CardTypeString => "Structure";

    public Dictionary<StatType, object> StatList => null;
    public UnityEvent E_OnStatChange => null;

    public bool TryGetValue(StatType statType, out object value)
    {
        switch (statType)
        {
            case StatType.BaseGoldCost: value = goldCost; break;
            case StatType.GoldCost: value = goldCost; break;
            case StatType.BaseManaCost: value = manaCost; break;
            case StatType.ManaCost: value = manaCost; break;
            case StatType.Health: value = health; break;
            case StatType.BaseHealth: value = health; break;
            case StatType.Attack: value = -1; break;
            case StatType.BaseAttack: value = -1; break;
            case StatType.Movement: value = -1; break;
            case StatType.BaseMovement: value = -1; break;
            case StatType.Range: value = -1; break;
            case StatType.BaseRange: value = -1; break;
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
