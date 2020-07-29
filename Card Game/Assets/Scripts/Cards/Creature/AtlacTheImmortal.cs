using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class AtlacTheImmortal : Creature
{
    public override int cardId => 28;

    public override void onInitialization()
    {
        sourceCard.E_AddedToCardPile += SourceCard_E_AddedToCardPile;
    }
    private void OnDestroy()
    {
        sourceCard.E_AddedToCardPile -= SourceCard_E_AddedToCardPile;
    }
    private void SourceCard_E_AddedToCardPile(object sender, Card.AddedToCardPileArgs e)
    {
        if (e.newCardPile is Graveyard)
            onSentToGrave();
    }

    public void onSentToGrave()
    {
        BaseAttack += 3;
        BaseHealth += 3;
        setHealth(BaseHealth);
        setAttack(BaseAttack);

        sourceCard.moveToCardPile(sourceCard.owner.deck, sourceCard);
        sourceCard.owner.deck.shuffle();
    }
}
*/