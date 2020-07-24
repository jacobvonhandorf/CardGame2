using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingOfEternity : SpellCard
{
    public const int CARD_ID = 85;
    public override int cardId => CARD_ID;

    private const int FIRST_THRESHOLD = 1;
    private const int FIRST_ATK_BONUS = 1;
    private const int FIRST_HP_BONUS = 1;
    private const int SECOND_THRESHOLD = 3;
    private const int THIRD_THRESHOLD = 6;
    private const int THIRD_MANA_TO_ADD = 1;

    public override List<Tile> legalTargetTiles => GameManager.Get().getAllTilesWithCreatures(owner, true);

    public override void onInitialization()
    {
        if (!owner.extraStats.ContainsKey(ExtraStatsKey.NumRingOfEternityPlayed))
            owner.extraStats.Add(ExtraStatsKey.NumRingOfEternityPlayed, 0);
    }

    protected override void doEffect(Tile t)
    {
        Creature targetCreature = t.creature;
        // add last breath effect
        targetCreature.sourceCard.E_AddedToCardPile += SourceCard_E_AddedToCardPile;
        targetCreature.addKeyword(Keyword.LastBreath);

        if (owner.extraStats[ExtraStatsKey.NumRingOfEternityPlayed] >= FIRST_THRESHOLD)
        {
            targetCreature.addAttack(FIRST_ATK_BONUS);
            targetCreature.addHealth(FIRST_HP_BONUS);
        }
        if (owner.extraStats[ExtraStatsKey.NumRingOfEternityPlayed] >= SECOND_THRESHOLD)
        {
            owner.drawCard();
        }
        if (owner.extraStats[ExtraStatsKey.NumRingOfEternityPlayed] >= SECOND_THRESHOLD)
        {
            owner.addMana(THIRD_MANA_TO_ADD);
        }
    }

    private void SourceCard_E_AddedToCardPile(object sender, AddedToCardPileArgs e)
    {
        if (e.previousCardPile is Board && e.newCardPile is Graveyard)
        {
            // last breath effect
            Creature effectCreature = (sender as CreatureCard).creature;
            Player effectOwner = effectCreature.controller;
            foreach (Card c in effectOwner.graveyard.getCardList())
            {
                if (c.cardId == CARD_ID)
                {
                    c.moveToCardPile(effectOwner.hand, effectCreature.sourceCard);
                    break;
                }
            }

            // make sure to remove effect and tool tip
            effectCreature.removeKeyword(Keyword.LastBreath);
            effectCreature.sourceCard.E_AddedToCardPile -= SourceCard_E_AddedToCardPile;
        }
    }

    protected override List<Tag> getInitialTags() => new List<Tag>() { Tag.Arcane };
}
