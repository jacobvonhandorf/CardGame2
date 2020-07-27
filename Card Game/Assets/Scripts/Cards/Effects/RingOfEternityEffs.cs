using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Card;

public class RingOfEternityEffs : SpellEffects
{
    private const int FIRST_THRESHOLD = 1;
    private const int FIRST_ATK_BONUS = 1;
    private const int FIRST_HP_BONUS = 1;
    private const int SECOND_THRESHOLD = 3;
    private const int THIRD_THRESHOLD = 6;
    private const int THIRD_MANA_TO_ADD = 1;

    public override List<Tile> validTiles => getValidTiles();
    public override bool canBePlayed => card.owner.getAllControlledCreatures().FindAll(c => c.hasTag(Tag.Arcane)).Count > 0;

    public List<Tile> getValidTiles()
    {
        Debug.Log(Board.instance);
        Debug.Log(card);
        Debug.Log(card.owner);

        return Board.instance.getAllTilesWithCreatures(card.owner, true);
    }

    public override void doEffect(Tile t)
    {
        Creature targetCreature = t.creature;
        // add last breath effect
        targetCreature.sourceCard.E_AddedToCardPile += SourceCard_E_AddedToCardPile;
        targetCreature.addKeyword(Keyword.LastBreath);

        if (card.owner.extraStats[ExtraStatsKey.NumRingOfEternityPlayed] >= FIRST_THRESHOLD)
        {
            targetCreature.addAttack(FIRST_ATK_BONUS);
            targetCreature.addHealth(FIRST_HP_BONUS);
        }
        if (card.owner.extraStats[ExtraStatsKey.NumRingOfEternityPlayed] >= SECOND_THRESHOLD)
        {
            card.owner.drawCard();
        }
        if (card.owner.extraStats[ExtraStatsKey.NumRingOfEternityPlayed] >= SECOND_THRESHOLD)
        {
            card.owner.addMana(THIRD_MANA_TO_ADD);
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
                if (c.cardId == (int)CardIds.RingOfEternity)
                {
                    c.moveToCardPile(effectOwner.hand, effectCreature.sourceCard);
                    break;
                }
            }

            // make sure to remove effect and keyword
            effectCreature.removeKeyword(Keyword.LastBreath);
            effectCreature.sourceCard.E_AddedToCardPile -= SourceCard_E_AddedToCardPile;
        }
    }

    public override EmptyHandler onInitilization => delegate ()
    {
        if (!card.owner.extraStats.ContainsKey(ExtraStatsKey.NumRingOfEternityPlayed))
            card.owner.extraStats.Add(ExtraStatsKey.NumRingOfEternityPlayed, 0);
    };

}
