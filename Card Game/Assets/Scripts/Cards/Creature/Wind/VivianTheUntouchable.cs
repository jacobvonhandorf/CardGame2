using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VivianTheUntouchable : Creature
{
    private void Awake()
    {
        base.Awake();
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
        GameManager.Get().setUpSingleTileTargetEffect(new OnAddedToHandEffect(this), sourceCard.owner, null, this, null, "Select a tile to deploy Vivian", false);
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

        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            GameManager.Get().createCreatureOnTile(vivian, targetTile, sourcePlayer, vivian.sourceCard);
        }

        public List<Tile> getValidTargetTiles(Player sourcePlayer, Player oppositePlayer, Tile sourceTile)
        {
            Debug.Log(sourcePlayer);
            return GameManager.Get().getAllDeployableTiles(sourcePlayer);
        }

        public bool canBeCancelled()
        {
            return false;
        }
    }
}
