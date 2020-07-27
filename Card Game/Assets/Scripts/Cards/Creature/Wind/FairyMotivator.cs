using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyMotivator : Creature
{
    public override int cardId => 48;
    public override List<Card.Tag> getInitialTags() => new List<Card.Tag>() { Card.Tag.Fairy };

    public override void onInitialization()
    {
        E_OnDeployed += FairyMotivator_E_OnDeployed;
    }
    private void OnDestroy()
    {
        E_OnDeployed -= FairyMotivator_E_OnDeployed;
    }

    private void FairyMotivator_E_OnDeployed(object sender, System.EventArgs e)
    {
        foreach (Card c in controller.hand.getAllCardsWithType(Card.CardType.Creature))
        {
            (c as CreatureCard).creature.addHealth(1);
            (c as CreatureCard).creature.addAttack(1);
        }
    }
}
