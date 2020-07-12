using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneGrandmaster : Creature, SingleTileTargetEffect
{
    //private const int effRange = 3;
    //private const int numCardsNeededForBonusDamage = 4;

    private const int FIRST_THRESHOLD = 3;
    private const int SECOND_THRESHOLD = 6;
    private const int THIRD_THRESHOLD = 10;

    public override int getStartingRange()
    {
        return 1;
    }

    public override int getCardId()
    {
        return 65;
    }

    public override List<Keyword> getInitialKeywords()
    {
        return new List<Keyword>() { Keyword.deploy };
    }

    public override void onAnySpellCast(SpellCard spell)
    {
        // if is in hand and the spell was cast by the same owner
        if (sourceCard.getCardPile() is Hand && spell.owner == sourceCard.owner)
        {
            addAttack(1);
        }
    }

    public override void onCreation()
    {
        if (getAttack() >= FIRST_THRESHOLD)
        {
            List<Card> arcaneCards = controller.deck.getAllCardsWithTag(Card.Tag.Arcane);
            int index = Random.Range(0, arcaneCards.Count);
            arcaneCards[index].moveToCardPile(controller.hand, sourceCard);
        }
        if (getAttack() >= SECOND_THRESHOLD)
        {
            List<Card> arcaneCards = controller.deck.getAllCardsWithTag(Card.Tag.Arcane);
            int index = Random.Range(0, arcaneCards.Count);
            arcaneCards[index].moveToCardPile(controller.hand, sourceCard);
        }
        if (getAttack() >= THIRD_THRESHOLD)
        {
            if (GameManager.Get().getAllTilesWithCreatures(GameManager.Get().getOppositePlayer(controller)).Count > 0)
                GameManager.Get().setUpSingleTileTargetEffect(this, controller, currentTile, this, null, "Select a creature to destroy", true);
        }
    }

    public List<Tile> getValidTargetTiles(Player sourcePlayer, Player oppositePlayer, Tile sourceTile)
    {
        return GameManager.Get().getAllTilesWithCreatures(oppositePlayer);
    }

    public bool canBeCancelled()
    {
        return true;
    }

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        GameManager.Get().destroyCreature(targetCreature);
    }

    /* old effect
    private class Eff : SingleTileTargetEffect
    {
        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
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

        public List<Tile> getValidTargetTiles(Player sourcePlayer, Player oppositePlayer, Tile sourceTile)
        {
            return GameManager.Get().board.getAllTilesWithinRangeOfTile(sourceTile, effRange);
        }

        public bool canBeCancelled()
        {
            return true;
        }
    }

    public override List<Card.Tag> getTags()
    {
        List<Card.Tag> tags = new List<Card.Tag>();
        tags.Add(Card.Tag.Arcane);
        return tags;
    }*/
}
