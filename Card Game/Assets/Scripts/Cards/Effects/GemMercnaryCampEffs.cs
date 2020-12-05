using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemMercnaryCampEffs : StructureEffects
{
    public override EmptyHandler activatedEffect => delegate ()
    {
        if (controller.GetActions() < 1)
        {
            GameManager.Get().ShowToast("Not enough actions to activate this effect");
            return;
        }
        if (controller.hand.getAllCardsWithTag(Card.Tag.Gem).Count < 1)
        {
            GameManager.Get().ShowToast("You must have a Gem in your hand to activate this effect");
            return;
        }

        Card selectedCard;
        QueueableCommand pickCmd = CardPicker.CreateCommand(controller.hand.getAllCardsWithTag(Card.Tag.Gem), 1, 1, "Select a Gem to shuffle into your deck", controller, delegate (List<Card> cardList)
        {
            selectedCard = cardList[0];
        });
        QueueableCommand singleTileCmd = SingleTileTargetEffect.CreateCommand(structure.Tile.getAdjacentTiles(), delegate (Tile t)
        {
            CreatureCard newCreature = GameManager.Get().createCardById((int) CardIds.Mercenary, controller) as CreatureCard;
            GameManager.Get().createCreatureOnTile(newCreature.Creature, t, controller);
            controller.subtractActions(1);
        });
    };
}
