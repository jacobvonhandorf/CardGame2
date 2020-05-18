using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrindToDust : SpellCard
{
    public override int getCardId()
    {
        return 19;
    }

    public override List<Tile> getLegalTargetTiles()
    {
        List<Tile> returnList = GameManager.Get().getAllTilesWithCreatures(GameManager.Get().getOppositePlayer(owner));
        returnList.RemoveAll(t => t.creature.sourceCard.getCardName().Equals("Engineer"));
        return returnList;
    }

    protected override Effect getEffect()
    {
        return new GTDEffect();
    }

    private class GTDEffect : Effect
    {
        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            targetCreature.takeDamage(sourcePlayer.graveyard.getAllCardsWithTag(Tag.Gem).Count);
        }
    }
}
