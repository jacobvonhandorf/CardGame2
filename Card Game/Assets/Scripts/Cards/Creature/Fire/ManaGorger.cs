using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaGorger : Creature
{
    public override int cardId => 63;

    public override void onCreation()
    {
        int manaSpent = controller.getMana();
        controller.addMana(-manaSpent);

        addAttack(manaSpent);
        addHealth(manaSpent);
        if (manaSpent >= 3)
            controller.drawCard();
    }

    public override List<Card.Tag> getTags() => new List<Card.Tag>() { Card.Tag.Arcane };
    public override List<Keyword> getInitialKeywords() => new List<Keyword>() { Keyword.deploy };
}
