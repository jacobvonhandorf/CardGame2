﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : SpellCard
{
    public override int cardId => 25;

    public override List<Tile> getLegalTargetTiles()
    {
        return new List<Tile>();
    }

    protected override Effect getEffect()
    {
        return null;
    }

    public override void onInitialization()
    {
        E_AddedToCardPile += Diamond_E_AddedToCardPile;
    }

    private void OnDestroy()
    {
        E_AddedToCardPile -= Diamond_E_AddedToCardPile;
    }

    private void Diamond_E_AddedToCardPile(object sender, AddedToCardPileArgs e)
    {
        if (e.newCardPile is Hand && e.source != null)
        {
            owner.drawCard();
            showInEffectsView();
        }
    }

    protected override List<Tag> getTags() => new List<Tag>() { Tag.Gem };
}
