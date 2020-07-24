using UnityEngine;
using System.Collections;
using System;

public class TestCreatureEffects : CreatureEffects
{
    public override EventHandler<OnDefendArgs> onDefend => delegate (object sender, OnDefendArgs e)
    {
        
    };
    public override EventHandler onDeploy => delegate (object sender, EventArgs e)
    {

    };
    public override EventHandler<Creature.OnAttackArgs> onAttack => delegate (object sender, Creature.OnAttackArgs e)
    {

    };
    public override EventHandler<Card.AddedToCardPileArgs> onMoveToCardPile => delegate (object sender, Card.AddedToCardPileArgs e)
    {

    };
}
