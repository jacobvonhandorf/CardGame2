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

    public override List<Tile> ValidTiles => getValidTiles();
    public override bool CanBePlayed => card.Owner.ControlledCreatures.FindAll(c => c.HasTag(Tag.Arcane)).Count > 0;

    public List<Tile> getValidTiles()
    {
        Debug.Log(Board.Instance);
        Debug.Log(card);
        Debug.Log(card.Owner);

        return Board.Instance.GetAllTilesWithCreatures(card.Owner, true);
    }

    public override void DoEffect(Tile t)
    {
        Creature targetCreature = t.creature;
        // add last breath effect
        targetCreature.SourceCard.E_AddedToCardPile += SourceCard_E_AddedToCardPile;
        targetCreature.AddKeyword(Keyword.LastBreath);

        if (card.Owner.ExtraStats[ExtraStatsKey.NumRingOfEternityPlayed] >= FIRST_THRESHOLD)
        {
            targetCreature.AttackStat += FIRST_ATK_BONUS;
            targetCreature.Health += FIRST_HP_BONUS;
        }
        if (card.Owner.ExtraStats[ExtraStatsKey.NumRingOfEternityPlayed] >= SECOND_THRESHOLD)
        {
            card.Owner.DrawCard();
        }
        if (card.Owner.ExtraStats[ExtraStatsKey.NumRingOfEternityPlayed] >= SECOND_THRESHOLD)
        {
            card.Owner.Mana += THIRD_MANA_TO_ADD;
        }
    }

    private void SourceCard_E_AddedToCardPile(object sender, AddedToCardPileArgs e)
    {
        if (e.previousCardPile is Board && e.newCardPile is Graveyard)
        {
            // last breath effect
            Creature effectCreature = (sender as CreatureCard).Creature;
            Player effectOwner = effectCreature.Controller;
            foreach (Card c in effectOwner.Graveyard.CardList)
            {
                if (c.CardId == (int)CardIds.RingOfEternity)
                {
                    c.MoveToCardPile(effectOwner.Hand, effectCreature.SourceCard);
                    break;
                }
            }

            // make sure to remove effect and keyword
            effectCreature.RemoveKeyword(Keyword.LastBreath);
            effectCreature.SourceCard.E_AddedToCardPile -= SourceCard_E_AddedToCardPile;
        }
    }

    public override EmptyHandler OnInitilization => delegate ()
    {
        if (!card.Owner.ExtraStats.ContainsKey(ExtraStatsKey.NumRingOfEternityPlayed))
            card.Owner.ExtraStats.Add(ExtraStatsKey.NumRingOfEternityPlayed, 0);
    };

}
