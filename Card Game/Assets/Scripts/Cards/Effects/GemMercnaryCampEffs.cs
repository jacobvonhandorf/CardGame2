using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemMercnaryCampEffs : StructureEffects
{
    public override EmptyHandler activatedEffect => delegate ()
    {
        if (Controller.Actions < 1)
        {
            Toaster.Instance.DoToast("Not enough actions to activate this effect");
            return;
        }
        if (Controller.Hand.GetAllCardsWithTag(Tag.Gem).Count < 1)
        {
            Toaster.Instance.DoToast("You must have a Gem in your hand to activate this effect");
            return;
        }

        Card selectedCard;
        IQueueableCommand pickCmd = CardPicker.CreateCommand(Controller.Hand.GetAllCardsWithTag(Tag.Gem), 1, 1, "Select a Gem to shuffle into your deck", Controller, delegate (List<Card> cardList)
        {
            selectedCard = cardList[0];
        });
        IQueueableCommand singleTileCmd = SingleTileTargetEffect.CreateCommand(Structure.Tile.AdjacentTiles, delegate (Tile t)
        {
            CreatureCard newCreature = GameManager.Instance.CreateCardById((int) CardIds.Mercenary, Controller) as CreatureCard;
            GameManager.Instance.createCreatureOnTile(newCreature.Creature, t, Controller);
            Controller.Actions -= 1;
        });
    };
}
