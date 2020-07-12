using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class Renovations : SpellCard
{
    public override int getCardId()
    {
        return 11;
    }

    public override List<Tile> getLegalTargetTiles()
    {
        List<Tile> returnList = new List<Tile>();
        foreach (Structure s in GameManager.Get().board.getAllStructures(owner))
        {
            returnList.Add(s.tile);
        }
        return returnList;
    }

    protected override Effect getEffect()
    {
        return new RenovationsEffect();
    }

    private class RenovationsEffect : Effect
    {
        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            GameManager.Get().destroyStructure(targetTile.structure);
            Deck deck = sourcePlayer.deck;
            List<Card> cardList = deck.getAllCardsWithType(CardType.Structure);
            if (cardList.Count == 0)
            {
                GameManager.Get().showToast("No structures remaining in deck");
                return;
            }

            Card card = cardList[0];
            deck.removeCard(card);
            StructureCard newStructureCard = card as StructureCard;
            // StructureCard newStructureCard = sourcePlayer.deck.getAllCardsWithType(CardType.Structure)[0] as StructureCard;
            newStructureCard.play(targetTile);
        }
    }
}
*/