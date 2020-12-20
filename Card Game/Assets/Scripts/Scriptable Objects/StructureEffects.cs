using UnityEngine;
using System.Collections;
using System;
using static Card;

public class StructureEffects : MonoBehaviour
{
    public Structure Structure { get; set; }
    public Card Card { get; set; }
    public Player Controller { get { return Structure.Controller; } }

    public virtual EmptyHandler onInitilization { get; }
    public virtual EventHandler onDeploy { get; }
    public virtual EventHandler onLeavesField { get; }
    public virtual EventHandler<OnDefendArgs> onDefend { get; }
    //public virtual EventHandler<OnDamagedArgs> onDamaged { get; }
    public virtual EventHandler<AddedToCardPileArgs> onMoveToCardPile { get; }
    public virtual EmptyHandler activatedEffect { get; }
}
