using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemMercenaryCamp : Structure, Effect
{
    public override int cardId => 41;

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        if (sourcePlayer.GetActions() < 1)
        {
            GameManager.Get().showToast("Not enough actions to activate this effect");
            return;
        }
        if (sourcePlayer.hand.getAllCardsWithTag(Card.Tag.Gem).Count < 1)
        {
            GameManager.Get().showToast("You must have a Gem in your hand to activate this effect");
            return;
        }

        Card selectedCard;
        QueueableCommand pickCmd = CardPicker.CreateCommand(controller.hand.getAllCardsWithTag(Card.Tag.Gem), 1, 1, "Select a Gem to shuffle into your deck", controller, delegate (List<Card> cardList)
        {
            selectedCard = cardList[0];
        });
        QueueableCommand singleTileCmd = SingleTileTargetEffect.CreateCommand(tile.getAdjacentTiles(), delegate (Tile t)
        {
            CreatureCard newCreature = GameManager.Get().createCardById(GemMercenary.CARD_ID, sourcePlayer) as CreatureCard;
            GameManager.Get().createCreatureOnTile(newCreature.creature, targetTile, sourcePlayer);
            controller.subtractActions(1);
        });
    }

    public override Effect getEffect()
    {
        return this;
    }
}
