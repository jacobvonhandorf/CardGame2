using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IScriptCreature : IScriptPermanent
{
    int BaseAttack { get; set; }
    int BaseRange { get; set; }
    int BaseMovement { get; set; }
    int AttackStat { get; set; }
    int Range { get; set; }
    int Movement { get; set; }
    bool HasMovedThisTurn { get; set; }
    bool HasDoneActionThisTurn { get; set; }
    bool CanDeployFrom { get; set; }
    List<EmptyHandler> ActivatedEffects { get; }

    void ResetToBaseStats();
    void Bounce(Card source);
    void MoveByEffect(Tile tile, Card source);
    void MoveByEffect(int x, int y, Card source);
}
