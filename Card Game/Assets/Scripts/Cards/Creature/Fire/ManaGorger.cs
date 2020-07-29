using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class ManaGorger : Creature
{
    public override int cardId => 63;
    public override List<Card.Tag> getInitialTags() => new List<Card.Tag>() { Card.Tag.Arcane };

    public override void onInitialization()
    {
        E_OnDeployed += ManaGorger_E_OnDeployed;
    }

    private void ManaGorger_E_OnDeployed(object sender, System.EventArgs e)
    {
        int manaSpent = controller.getMana();
        controller.addMana(-manaSpent);

        addAttack(manaSpent);
        addHealth(manaSpent);
        if (manaSpent >= 3)
            controller.drawCard();
    }
}
*/