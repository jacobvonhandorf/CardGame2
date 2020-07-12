using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyCavalier : Creature, SingleTileTargetEffect
{
    public override List<Card.Tag> getTags()
    {
        List<Card.Tag> tags = new List<Card.Tag>();
        tags.Add(Card.Tag.Fairy);
        return tags;
    }

    public override void onCreation()
    {
        GameManager.Get().setUpSingleTileTargetEffect(this, controller, currentTile, this, null, "Select a creature to return to bounce", false);
    }

    public override int getStartingRange()
    {
        return 1;
    }

    public override int getCardId()
    {
        return 51;
    }

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        Debug.Log("Cavalier effect activated");
        targetTile.creature.bounce(sourceCard);
    }

    public List<Tile> getValidTargetTiles(Player sourcePlayer, Player oppositePlayer, Tile sourceTile)
    {
        Debug.Log(sourcePlayer);
        Debug.Log(GameManager.Get().getAllTilesWithCreatures(sourcePlayer).Count);
        List<Tile> returnList = GameManager.Get().getAllTilesWithCreatures(sourcePlayer);
        returnList.RemoveAll(t => t.creature is Engineer); // can't bounce engineers
        return GameManager.Get().getAllTilesWithCreatures(sourcePlayer);
    }

    public bool canBeCancelled()
    {
        return false;
    }

    public override List<Keyword> getInitialKeywords()
    {
        return new List<Keyword>()
        {
            Keyword.deploy
        };
    }

}
