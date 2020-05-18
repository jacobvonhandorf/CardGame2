using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaGorger : Creature
{
    public override int getStartingRange()
    {
        return 1;
    }

    public override void onCreation()
    {
        int manaSpent = controller.getMana();
        controller.addMana(-manaSpent);

        addAttack(manaSpent);
        addHealth(manaSpent);
        if (manaSpent >= 3)
            controller.drawCard();
    }

    public override List<Card.Tag> getTags()
    {
        List<Card.Tag> tags = new List<Card.Tag>();
        tags.Add(Card.Tag.Arcane);
        return tags;
    }

    public override int getCardId()
    {
        return 63;
    }
}
