using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VivianTheUntouchable : Creature
{
    public override List<Keyword> getInitialKeywords()
    {
        return new List<Keyword>() { Keyword.quick };
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

    public override void onInitialization()
    {
        sourceCard.E_AddedToCardPile += SourceCard_E_AddedToCardPile;
    }
    private void OnDestroy()
    {
        sourceCard.E_AddedToCardPile -= SourceCard_E_AddedToCardPile;
    }

    private void SourceCard_E_AddedToCardPile(object sender, Card.AddedToCardPileArgs e)
    {
        if (e.newCardPile is Hand && e.source != null)
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
            GameManager.Get().createCreatureOnTile(vivian, targetTile, vivian.sourceCard.owner, vivian.sourceCard);
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
