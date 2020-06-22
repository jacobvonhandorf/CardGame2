using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruby : SpellCard, CanReceivePickedCards
{
    public override List<Tile> getLegalTargetTiles()
    {
        return new List<Tile>();
    }

    protected override Effect getEffect()
    {
        return null;
    }

    public override void onCardAddedByEffect()
    {
        doEffect();
    }

    public override void onCardDrawn()
    {
        doEffect();
    }

    protected override List<Tag> getTags()
    {
        List<Tag> tags = new List<Tag>();
        tags.Add(Tag.Gem);
        return tags;
    }

    private void doEffect()
    {
        GameManager.Get().setUpSingleTileTargetEffect(new RubyEff(), owner, null, null, null, "Ruby's Effect", false);
    }

    private class RubyEff : SingleTileTargetEffect
    {
        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            targetCreature.addAttack(1);
            targetCreature.addHealth(1);
        }

        public bool canBeCancelled()
        {
            return true;
        }

        public List<Tile> getValidTargetTiles(Player sourcePlayer, Player oppositePlayer, Tile sourceTile)
        {
            List<Tile> pickableTiles = new List<Tile>();
            foreach (Creature c in GameManager.Get().getAllCreaturesControlledBy(sourcePlayer))
                pickableTiles.Add(c.currentTile);
            return pickableTiles;
        }
    }

    public void receiveCardList(List<Card> cardList)
    {
        foreach (Card c in cardList)
        {
            (c as CreatureCard).creature.addAttack(1);
            (c as CreatureCard).creature.addHealth(1);
        }
    }

    public override int getCardId()
    {
        return 21;
    }
}
