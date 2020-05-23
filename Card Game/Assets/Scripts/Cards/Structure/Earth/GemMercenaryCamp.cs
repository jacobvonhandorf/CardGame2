using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemMercenaryCamp : Structure
{
    public override bool canDeployFrom()
    {
        return true;
    }

    public override bool canWalkOn()
    {
        return false;
    }

    public override int getCardId()
    {
        return 41;
    }

    public override Effect getEffect()
    {
        return new GemMercCampEffect();
    }

    private class GemMercCampEffect : SingleTileTargetEffect
    {
        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            if (sourcePlayer.GetActions() <= 0)
            {
                GameManager.Get().showToast("You do not have enough actions to activate this effect");
                return;
            }

            GameManager.Get().queueCardPickerEffect(sourcePlayer, sourcePlayer.hand.getAllCardsWithTag(Card.Tag.Gem), new PickReceiver(targetTile, sourcePlayer), 1, 1, "Select a Gem card to discard");
        }

        private class PickReceiver : CanReceivePickedCards
        {
            private Tile targetTile;
            private Player sourcePlayer;

            public PickReceiver(Tile targetTile, Player sourcePlayer)
            {
                this.targetTile = targetTile;
                this.sourcePlayer = sourcePlayer;
            }

            public void receiveCardList(List<Card> cardList)
            {
                foreach (Card c in cardList)
                    c.moveToCardPile(sourcePlayer.graveyard);
                CreatureCard newCreature = GameManager.Get().createCardById(GemMercenary.CARD_ID, sourcePlayer) as CreatureCard;
                GameManager.Get().createCreatureOnTile(newCreature.creature, targetTile, sourcePlayer, newCreature);
                sourcePlayer.subtractActions(1);
            }
        }

        public List<Tile> getValidTargetTiles(Player sourcePlayer, Player oppositePlayer, Tile sourceTile)
        {
            List<Tile> returnList = new List<Tile>();
            int x = sourceTile.x;
            int y = sourceTile.y;
            Board board = GameManager.Get().board;

            if (x < board.boardWidth - 1) // right
                returnList.Add(board.getTileByCoordinate(x + 1, y));
            if (x > 0) // left
                returnList.Add(board.getTileByCoordinate(x - 1, y));
            if (y < board.boardHeight - 1) // up
                returnList.Add(board.getTileByCoordinate(x, y + 1));
            if (y > 0) // down
                returnList.Add(board.getTileByCoordinate(x, y - 1));
            returnList.RemoveAll(t => t.creature != null);

            return returnList;
        }

        public bool canBeCancelled()
        {
            return true;
        }
    }
}
