using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Topaz : SpellCard
{
    public override bool additionalCanBePlayedChecks()
    {
        return false;
    }

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
        E_AddedToCardPile += Topaz_E_AddedToCardPile;
    }
    private void OnDestroy()
    {
        E_AddedToCardPile -= Topaz_E_AddedToCardPile;
    }

    private void Topaz_E_AddedToCardPile(object sender, AddedToCardPileArgs e)
    {
        if (e.newCardPile is Hand && e.source != null)
        {
            owner.addGold(1);
            owner.addMana(1);
            showInEffectsView();
        }
    }

    protected override List<Tag> getTags()
    {
        List<Tag> tags = new List<Tag>();
        tags.Add(Tag.Gem);
        return tags;
    }

    public override int getCardId()
    {
        return 20;
    }
}
