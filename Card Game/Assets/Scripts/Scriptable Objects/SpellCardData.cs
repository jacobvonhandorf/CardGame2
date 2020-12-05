using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Creature Data", menuName = "Card Data/Spell Data")]
public class SpellCardData : CardData, IHaveReadableStats
{
    public SpellEffects effects;

    public override string CardTypeString => "Spell";
    public override IHaveReadableStats ReadableStats => this;
    public UnityEvent E_OnStatChange => null;

    public bool TryGetValue(StatType statType, out object value)
    {
        switch (statType)
        {
            case StatType.BaseGoldCost: value = -1; break;
            case StatType.GoldCost: value = -1; break;
            case StatType.BaseManaCost: value = manaCost; break;
            case StatType.ManaCost: value = manaCost; break;
            case StatType.Health: value = -1; break;
            case StatType.BaseHealth: value = -1; break;
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
