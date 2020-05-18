using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneGrandmaster : Creature
{
    private const int effRange = 3;
    private const int numCardsNeededForBonusDamage = 4;

    public override int getStartingRange()
    {
        return 1;
    }

    public override Effect getEffect()
    {
        return new Eff();
    }

    private class Eff : SingleTileTargetEffect
    {
        public override void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            int damage;
            if (sourcePlayer.graveyard.getAllCardsWithTag(Card.Tag.Arcane).Count >= numCardsNeededForBonusDamage)
                damage = 2;
            else
                damage = 1;

            Board board = GameManager.Get().board;
            damageTile(targetTile, damage);
            damageTile(board.getTileByCoordinate(targetTile.x, targetTile.y + 1), damage); // up
            damageTile(board.getTileByCoordinate(targetTile.x + 1, targetTile.y), damage); // right
            damageTile(board.getTileByCoordinate(targetTile.x, targetTile.y - 1), damage); // down
            damageTile(board.getTileByCoordinate(targetTile.x - 1, targetTile.y), damage); // left
        }

        private void damageTile(Tile t, int damage)
        {
            if (t == null)
                return;
            if (t.creature != null)
                t.creature.takeDamage(damage);
            if (t.structure != null)
                t.structure.takeDamage(damage);
        }

        public override List<Tile> getValidTargetTiles(Player sourcePlayer, Player oppositePlayer, Tile sourceTile)
        {
            return GameManager.Get().board.getAllTilesWithinRangeOfTile(sourceTile, effRange);
        }
    }

    public override List<Card.Tag> getTags()
    {
        List<Card.Tag> tags = new List<Card.Tag>();
        tags.Add(Card.Tag.Arcane);
        return tags;
    }

    public override int getCardId()
    {
        return 65;
    }
}
