using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VivianTheUntouchable : Creature
{
    private void Awake()
    {
        addKeyword(Card.CardKeywords.Quick);
    }

    public override int getStartingRange()
    {
        return 1;
    }

    public override List<Card.Tag> getTags()
    {
        List<Card.Tag> tags = new List<Card.Tag>();
        tags.Add(Card.Tag.Fairy);
        return tags;
    }

    public override void onCardAddedToHandByEffect()
    {
        Debug.Log("Card added to hand by effect");
        GameManager.Get().setUpSingleTileTargetEffect(new OnAddedToHandEffect(this), owner, null, this, null, "Select a tile to deploy Vivian");
    }

    public override int getCardId()
    {
        return 45;
    }

    private class OnAddedToHandEffect : SingleTileTargetEffect
    {
        Creature vivian;

        public OnAddedToHandEffect(Creature creature)
        {
            vivian = creature;
        }

        public override void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            GameManager.Get().createCreatureOnTile(vivian, targetTile, sourcePlayer, vivian.sourceCard);
        }

        public override List<Tile> getValidTargetTiles(Player sourcePlayer, Player oppositePlayer, Tile sourceTile)
        {
            Debug.Log(sourcePlayer);
            return GameManager.Get().getAllDeployableTiles(sourcePlayer);
        }

        public override bool canBeCancelled()
        {
            return false;
        }
    }
}
