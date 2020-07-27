﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyFortress : Structure, Effect
{
    public override int cardId => 50;

    public override Effect getEffect()
    {
        return this;
    }

    public override List<Card.Tag> getTags() => new List<Card.Tag>() { Card.Tag.Fairy };

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        #region Validation
        if (sourcePlayer.GetActions() <= 0)
        {
            GameManager.Get().showToast("You do not have enough actions to use this effect");
            return;
        }
        if (GameManager.Get().getAllTilesWithCreatures(sourcePlayer, true).Count < 1)
        {
            GameManager.Get().showToast("You must control a creature to activate this effect");
            return;
        }
        if (GameManager.Get().getAllTilesWithCreatures(GameManager.Get().getOppositePlayer(sourcePlayer), false).Count < 1)
        {
            GameManager.Get().showToast("Your opponent must control a creature to activate this effect");
            return;
        }
        #endregion

        #region CommandBuilding
        Creature ownersCreature = null;
        Creature opponentsCreature = null;
        SingleTileTargetEffect firstSelection = new SingleTileTargetEffect(GameManager.Get().getAllTilesWithCreatures(controller, true), delegate (Tile t)
        {
            ownersCreature = t.creature;
        });
        SingleTileTargetEffect secondSelection = new SingleTileTargetEffect(GameManager.Get().getAllTilesWithCreatures(controller.getOppositePlayer(), true), delegate (Tile t)
        {
            opponentsCreature = t.creature;
            ownersCreature.bounce(sourceCard);
            opponentsCreature.bounce(sourceCard);
            sourcePlayer.subtractActions(1);
        });
        CompoundQueueableCommand cqc = new CompoundQueueableCommand.Builder().addCommand(firstSelection).addCommand(secondSelection).Build();
        InformativeAnimationsQueue.instance.addAnimation(cqc);
        #endregion
    }
}
