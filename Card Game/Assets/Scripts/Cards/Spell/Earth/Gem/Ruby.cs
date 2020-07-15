using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruby : SpellCard
{
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
        E_AddedToCardPile += Ruby_E_AddedToCardPile;
    }
    private void OnDestroy()
    {
        E_AddedToCardPile -= Ruby_E_AddedToCardPile;
    }

    private void Ruby_E_AddedToCardPile(object sender, AddedToCardPileArgs e)
    {
        if (e.newCardPile is Hand)
        {
            showInEffectsView();
            doEffect();
        }
    }

    protected override List<Tag> getTags()
    {
        List<Tag> tags = new List<Tag>();
        tags.Add(Tag.Gem);
        return tags;
    }

    private void doEffect()
    {
        SingleTileTargetEffect.CreateAndQueue(GameManager.Get().getAllTilesWithCreatures(owner), delegate (Tile t)
        {
            t.creature.addAttack(1);
            t.creature.addHealth(1);
        });
    }

    public override int getCardId()
    {
        return 21;
    }
}
