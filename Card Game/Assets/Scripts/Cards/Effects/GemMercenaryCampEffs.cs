using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemMercenaryCampEffs : StructureEffects
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
        IQueueableCommand pickCmd = CardPicker.CreateCommand(Structure.Controller.Hand.GetAllCardsWithTag(Tag.Gem), 1, 1, "Select a Gem to shuffle into your deck", Structure.Controller, delegate (List<Card> cardList)
        {
            selectedCard = cardList[0];
        });
        IQueueableCommand singleTileCmd = SingleTileTargetEffect.CreateCommand(Structure.Tile.AdjacentTiles, delegate (Tile t)
        {
            CreatureCard newCreature = GameManager.Instance.CreateCardById((int)CardIds.Mercenary, Structure.Controller) as CreatureCard;
            newCreature.Creature.CreateOnTile(t);
            Structure.Controller.Actions -= 1;
        });
    };
}
