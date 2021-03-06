﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProspectorEffs : CreatureEffects
{
    public override EventHandler<Creature.OnAttackArgs> onAttack => delegate (object s, Creature.OnAttackArgs e)
    {
        shuffleObsidianIntoDeck();
    };
    public override EventHandler<OnDefendArgs> onDefend => delegate (object s, OnDefendArgs e)
    {
        shuffleObsidianIntoDeck();
    };
    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        shuffleObsidianIntoDeck();
    };

    private void shuffleObsidianIntoDeck()
    {
        Card obsidian = GameManager.Instance.CreateCardById((int)CardIds.Obsidian, creature.Controller);
        obsidian.MoveToCardPile(creature.Controller.Deck, card);
        creature.Controller.Deck.Shuffle();
    }
}
