﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemMercenaryCampEffs : StructureEffects
{
    public override EmptyHandler activatedEffect => delegate ()
    {
        if (structure.controller.GetActions() < 1)
        {
            GameManager.Get().showToast("Not enough actions to activate this effect");
            return;
        }
        if (structure.controller.hand.getAllCardsWithTag(Card.Tag.Gem).Count < 1)
        {
            GameManager.Get().showToast("You must have a Gem in your hand to activate this effect");
            return;
        }

        Card selectedCard;
        QueueableCommand pickCmd = CardPicker.CreateCommand(structure.controller.hand.getAllCardsWithTag(Card.Tag.Gem), 1, 1, "Select a Gem to shuffle into your deck", structure.controller, delegate (List<Card> cardList)
        {
            selectedCard = cardList[0];
        });
        QueueableCommand singleTileCmd = SingleTileTargetEffect.CreateCommand(structure.tile.getAdjacentTiles(), delegate (Tile t)
        {
            CreatureCard newCreature = GameManager.Get().createCardById(GemMercenary.CARD_ID, structure.controller) as CreatureCard;
            GameManager.Get().createCreatureOnTile(newCreature.creature, t, structure.controller);
            structure.controller.subtractActions(1);
        });

    };
}