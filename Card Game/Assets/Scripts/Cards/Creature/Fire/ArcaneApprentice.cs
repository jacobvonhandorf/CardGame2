using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneApprentice : Creature, Effect
{
    public const int EFFECT_RANGE = 2;

    public override int cardId => 66;

    public override int getStartingRange()
    {
        return 1;
    }

    public override Effect getEffect()
    {
        return this;
    }

    public override List<Card.Tag> getTags()
    {
        List<Card.Tag> tags = new List<Card.Tag>();
        tags.Add(Card.Tag.Arcane);
        return tags;
    }

    private void OnEnable()
    {
        GameEvents.E_SpellCast += GameEvents_E_SpellCast;
    }
    private void OnDisable()
    {
        GameEvents.E_SpellCast -= GameEvents_E_SpellCast;
    }
    private void GameEvents_E_SpellCast(object sender, GameEvents.SpellCastEventArgs e)
    {
        if (e.spell.owner == controller)
            addCounters(Counters.arcane, 1);
    }

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        if (hasDoneActionThisTurn)
        {
            GameManager.Get().showToast("You have already acted with this creature this turn");
            return;
        }

        int selectedValue = -1;
        QueueableCommand xPickCmd = XPickerBox.CreateAsCommand(1, hasCounter(Counters.arcane), "How many counters to remove?", controller, delegate (int x)
        {
            selectedValue = x;
        });
        QueueableCommand targetSelect = SingleTileTargetEffect.CreateCommand(GameManager.Get().getAllTilesWithCreatures(controller.getOppositePlayer(), false), delegate (Tile t)
        {
            removeCounters(Counters.arcane, selectedValue);
            t.creature.takeDamage(selectedValue);
            hasDoneActionThisTurn = true;
        });
        new CompoundQueueableCommand.Builder().addCommand(xPickCmd).addCommand(targetSelect).BuildAndQueue();
    }
}
