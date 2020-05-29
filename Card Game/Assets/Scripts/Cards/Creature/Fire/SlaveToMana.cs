using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlaveToMana : Creature, OptionBoxHandler, SingleTileTargetEffect
{
    public const int CARD_ID = 76;
    private const string YES = "Yes";
    private const string NO = "No";

    private static bool effectTriggeredThisTurn = false;

    // SingleTileTargetEffect
    public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
    {
        sourceCard.moveToCardPile(sourceCard.owner.hand);
        sourceCard.play(targetTile);
    }
    public bool canBeCancelled()
    {
        return true;
    }
    public List<Tile> getValidTargetTiles(Player sourcePlayer, Player oppositePlayer, Tile sourceTile)
    {
        return GameManager.Get().getAllDeployableTiles(sourcePlayer);
    }

    public override int getCardId()
    {
        return CARD_ID;
    }

    public override int getStartingRange()
    {
        return 1;
    }

    public override void onAnySpellCast(SpellCard spell)
    {
        Debug.Log("Spell cast #" + sourceCard.owner.numSpellsCastThisTurn);
        if (spell.owner == sourceCard.owner && sourceCard.getCardPile() is Deck && sourceCard.owner.numSpellsCastThisTurn == 3 && !effectTriggeredThisTurn)
        {
            Debug.Log("Inside if");
            effectTriggeredThisTurn = true;
            List<string> options = new List<string>()
            {
                YES,
                NO
            };
            GameManager.Get().queueOptionSelectBoxEffect(options, this, "Would you like to play " + sourceCard.getCardName() + " from your deck?", false);
        }
    }

    public override void resetForNewTurn()
    {
        base.resetForNewTurn();
        effectTriggeredThisTurn = false;
    }

    public override List<Card.Tag> getTags()
    {
        return new List<Card.Tag>()
        {
            Card.Tag.Arcane
        };
    }

    // OptionBoxHandler
    public void receiveOptionBoxSelection(int selectedOptionIndex, string selectedOption)
    {
        if (selectedOption == YES)
        {
            GameManager.Get().setUpSingleTileTargetEffect(this, sourceCard.owner, null, this, null, sourceCard.getCardName() + "'s Effect", true);
        }
    }
}
