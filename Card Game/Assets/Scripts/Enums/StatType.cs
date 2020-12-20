using UnityEngine;
using System.Collections;

public enum StatType
{
    BaseGoldCost,
    GoldCost,
    BaseManaCost,
    ManaCost,
    Health,
    BaseHealth,
    Attack,
    BaseAttack,
    Movement,
    BaseMovement,
    Range,
    BaseRange,
    Name,
    TypeText,
    EffectText,
    Art,
    ElementalId,
    CardType, // mostly exists so that CardVisualizations know which icons to turn on/off

    // player stats
    Gold,
    GoldPerTurn,
    Mana,
    ManaPerTurn,
    Actions,
    ActionsPerTurn,
    PlayerName
}
