using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneApprentice : Creature, Effect, CanRecieveXPick
{
    public const int EFFECT_RANGE = 2;

    public override int getStartingRange()
    {
        return 1;
    }

    public override Effect getEffect()
    {
        return this;
    }

    public override List<Card.Tag> getTags()
    {
        List<Card.Tag> tags = new List<Card.Tag>();
        tags.Add(Card.Tag.Arcane);
        return tags;
    }

    public override int getCardId()
    {
        return 66;
    }

    public override void onAnySpellCast(SpellCard spell)
    {
        if (sourceCard.isCreature && spell.owner == controller)
            addCounters(Counters.arcane, 1);
    }

    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        if (hasDoneActionThisTurn)
        {
            GameManager.Get().showToast("You have already acted with this creature this turn");
            return;
        }
        GameManager.Get().queueXPickerEffect(this, "How many counters to remove?", 1, hasCounter(Counters.arcane), false, controller);
    }

    public void receiveXPick(int value)
    {
        GameManager.Get().setUpSingleTileTargetEffect(new ApprenticeEff(value), controller, currentTile, this, null, "Choose target to deal " + value + " damage", true);
    }

    private class ApprenticeEff : SingleTileTargetEffect
    {
        private int value;

        public ApprenticeEff(int value)
        {
            this.value = value;
        }

        public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
        {
            sourceCreature.removeCounters(Counters.arcane, value);
            sourceCreature.hasDoneActionThisTurn = true;

            targetCreature.takeDamage(value);
        }

        public bool canBeCancelled()
        {
            return true;
        }

        public List<Tile> getValidTargetTiles(Player sourcePlayer, Player oppositePlayer, Tile sourceTile)
        {
            Debug.LogError("Opposite = " + oppositePlayer);
            List<Tile> returnList = GameManager.Get().getAllTilesWithCreatures(oppositePlayer);
            returnList.RemoveAll(t => t.getDistanceTo(sourceTile) > EFFECT_RANGE);
            return returnList;
        }
    }

    /* 
     * old code
     *      public override void onCreation()
            {
                GameManager.Get().queueCardPickerEffect(controller, controller.deck.getAllCardWithTagAndType(Card.Tag.Arcane, Card.CardType.Spell), this, 1, 1, "Select a card to add to your hand");
            }

            public void receiveCardList(List<Card> cardList)
            {
                controller.hand.addCardByEffect(cardList[0]);
            }

     */
}
