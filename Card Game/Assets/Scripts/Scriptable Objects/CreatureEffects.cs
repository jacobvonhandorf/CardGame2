using UnityEngine;
using System;
using static Creature;
using static Card;

public abstract class CreatureEffects : MonoBehaviour
{
    public Creature creature { get; set; }
    public Card card { get; set; }

    public virtual EmptyHandler onInitilization { get; }
    public virtual EventHandler onDeploy { get; }
    public virtual EventHandler onDeath { get; }
    public virtual EventHandler<OnDefendArgs> onDefend { get; }
    public virtual EventHandler<OnDamagedArgs> onDamaged { get; }
    public virtual EventHandler<OnAttackArgs> onAttack { get; }
    public virtual EventHandler<AddedToCardPileArgs> onMoveToCardPile { get; }
    public virtual ActivatedEffect activatedEffect { get; }
}
