using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlaveToMana : Creature
{
    public const int CARD_ID = 76;
    private const string YES = "Yes";
    private const string NO = "No";

    public override int cardId => CARD_ID;
    public override List<Card.Tag> getInitialTags() => new List<Card.Tag>() { Card.Tag.Arcane };

    public override void onInitialization()
    {
        GameEvents.E_SpellCast += GameEvents_E_SpellCast;
        GameEvents.E_TurnStart += GameEvents_E_TurnStart;
    }
    private void OnDestroy()
    {
        GameEvents.E_SpellCast -= GameEvents_E_SpellCast;
        GameEvents.E_TurnStart -= GameEvents_E_TurnStart;
    }

    private static bool effectTriggeredThisTurn = false;


    private void GameEvents_E_TurnStart(object sender, System.EventArgs e)
    {
        effectTriggeredThisTurn = false;
    }
    private void GameEvents_E_SpellCast(object sender, GameEvents.SpellCastArgs e)
    {
        if (e.spell.owner == SourceCard.owner && SourceCard.getCardPile() is Deck && SourceCard.owner.numSpellsCastThisTurn == 3 && !effectTriggeredThisTurn)
        {
            effectTriggeredThisTurn = true;
            List<string> options = new List<string>()
            {
                YES,
                NO
            };
            bool deploy = false;
            QueueableCommand optionCmd = OptionSelectBox.CreateCommand(options, "Deploy " + cardName + " from your deck?", SourceCard.owner, delegate (int selectedIndex, string selectedOption)
            {
                if (selectedIndex == 0)
                    deploy = true;
            });
            QueueableCommand deployCmd = SingleTileTargetEffect.CreateCommand(GameManager.Get().getAllDeployableTiles(SourceCard.owner), delegate (Tile t)
            {
                if (deploy)
                {
                    //sourceCard.moveToCardPile(sourceCard.owner.hand, sourceCard); // this is jank. Could cause errors in the future
                                                                                  // for now it's to stop bugs because "play" bugs out if the card is in deck
                    SourceCard.play(t);
                }
            });
            new CompoundQueueableCommand.Builder().addCommand(optionCmd).addCommand(deployCmd).BuildAndQueue();
        }
    }

}
