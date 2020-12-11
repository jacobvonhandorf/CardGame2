using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemMercenaryCampEffs : StructureEffects
{
    public override EmptyHandler activatedEffect => delegate ()
    {
        if (Structure.Controller.Actions < 1)
        {
            GameManager.Get().ShowToast("Not enough actions to activate this effect");
            return;
        }
        if (Structure.Controller.Hand.GetAllCardsWithTag(Card.Tag.Gem).Count < 1)
        {
            GameManager.Get().ShowToast("You must have a Gem in your hand to activate this effect");
            return;
        }

        Card selectedCard;
        IQueueableCommand pickCmd = CardPicker.CreateCommand(Structure.Controller.Hand.GetAllCardsWithTag(Card.Tag.Gem), 1, 1, "Select a Gem to shuffle into your deck", Structure.Controller, delegate (List<Card> cardList)
        {
            selectedCard = cardList[0];
        });
        IQueueableCommand singleTileCmd = SingleTileTargetEffect.CreateCommand(Structure.Tile.AdjacentTiles, delegate (Tile t)
        {
            CreatureCard newCreature = GameManager.Get().createCardById((int)CardIds.Mercenary, Structure.Controller) as CreatureCard;
            GameManager.Get().createCreatureOnTile(newCreature.Creature, t, Structure.Controller);
            Structure.Controller.Actions -= 1;
        });

    };
}
