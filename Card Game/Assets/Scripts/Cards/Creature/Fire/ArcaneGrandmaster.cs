﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class ArcaneGrandmaster : Creature
{
    private const int FIRST_THRESHOLD = 3;
    private const int SECOND_THRESHOLD = 6;
    private const int THIRD_THRESHOLD = 10;

    public override int cardId => 65;

    public override int getStartingRange()
    {
        return 1;
    }

    public override void onInitialization()
    {
        GameEvents.E_SpellCast += GameEvents_E_SpellCast;
        E_OnDeployed += ArcaneGrandmaster_E_OnDeployed;
    }
    private void OnDestroy()
    {
        GameEvents.E_SpellCast -= GameEvents_E_SpellCast;
        E_OnDeployed -= ArcaneGrandmaster_E_OnDeployed;
    }

    private void GameEvents_E_SpellCast(object sender, GameEvents.SpellCastArgs e)
    {
        if (sourceCard.getCardPile() is Hand && e.spell.owner == sourceCard.owner)
            addAttack(1);
    }
    private void ArcaneGrandmaster_E_OnDeployed(object sender, System.EventArgs e)
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
            SingleTileTargetEffect.CreateAndQueue(GameManager.Get().getAllTilesWithCreatures(controller.getOppositePlayer(), false), delegate (Tile t)
            {
                GameManager.Get().destroyCreature(t.creature);
            });
        }
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
    }
}
*/