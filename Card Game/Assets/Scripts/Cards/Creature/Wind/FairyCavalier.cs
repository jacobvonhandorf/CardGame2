using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyCavalier : Creature
{
    public override int cardId => 51;

    public override List<Card.Tag> getTags()
    {
        List<Card.Tag> tags = new List<Card.Tag>();
        tags.Add(Card.Tag.Fairy);
        return tags;
    }

    public override void onCreation()
    {
        SingleTileTargetEffect.CreateAndQueue(GameManager.Get().getAllTilesWithCreatures(controller, true), delegate (Tile t)
        {
            t.creature.bounce(sourceCard);
        });
    }

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        Debug.Log("Cavalier effect activated");
        targetTile.creature.bounce(sourceCard);
    }

    public override List<Keyword> getInitialKeywords()
    {
        return new List<Keyword>()
        {
            Keyword.deploy
        };
    }

}
