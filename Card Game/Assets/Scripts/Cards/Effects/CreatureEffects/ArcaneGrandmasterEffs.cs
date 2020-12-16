using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneGrandmasterEffs : CreatureEffects
{
    private const int FIRST_THRESHOLD = 3;
    private const int SECOND_THRESHOLD = 6;
    private const int THIRD_THRESHOLD = 10;

    public override EmptyHandler onInitilization => delegate ()
    {
        GameEvents.E_SpellCast += GameEvents_E_SpellCast;
    };
    private void GameEvents_E_SpellCast(object sender, GameEvents.SpellCastArgs e)
    {
        if (card.CardPile is Hand && e.spell.Owner == card.Owner)
            creature.AttackStat += 1;
    }

    public override EventHandler onDeploy => delegate (object s, EventArgs e)
    {
        if (creature.AttackStat >= FIRST_THRESHOLD)
        {
            List<Card> arcaneCards = creature.Controller.Deck.GetAllCardsWithTag(Tag.Arcane);
            int index = UnityEngine.Random.Range(0, arcaneCards.Count);
            arcaneCards[index].MoveToCardPile(creature.Controller.Hand, creature.SourceCard);
        }
        if (creature.AttackStat >= SECOND_THRESHOLD)
        {
            List<Card> arcaneCards = creature.Controller.Deck.GetAllCardsWithTag(Tag.Arcane);
            int index = UnityEngine.Random.Range(0, arcaneCards.Count);
            arcaneCards[index].MoveToCardPile(creature.Controller.Hand, card);
        }
        if (creature.AttackStat >= THIRD_THRESHOLD)
        {
            SingleTileTargetEffect.CreateAndQueue(Board.Instance.GetAllTilesWithCreatures(creature.Controller.OppositePlayer, false), delegate (Tile t)
            {
                GameManager.Instance.kill(t.creature);
            });
        }

    };
}
