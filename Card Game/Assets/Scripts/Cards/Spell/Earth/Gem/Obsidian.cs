using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obsidian : SpellCard
{
    public const int CARD_ID = 81;
    private const int DAMAGE_AMOUNT = 2;

    public override int cardId => CARD_ID;

    private List<Tile> emptyList = new List<Tile>();
    public override List<Tile> getLegalTargetTiles()
    {
        return emptyList;
    }

    protected override Effect getEffect()
    {
        return null;
    }

    public override void onInitialization()
    {
        E_AddedToCardPile += Obsidian_E_AddedToCardPile;
    }
    private void OnDestroy()
    {
        E_AddedToCardPile -= Obsidian_E_AddedToCardPile;
    }

    private void Obsidian_E_AddedToCardPile(object sender, AddedToCardPileArgs e)
    {
        if (e.newCardPile is Hand)
            doEffect();
    }

    private void doEffect()
    {
        owner.drawCard();
        SingleTileTargetEffect.CreateAndQueue(GameManager.Get().getAllTilesWithCreatures(GameManager.Get().getOppositePlayer(owner), false), delegate (Tile t)
        {
            t.creature.takeDamage(DAMAGE_AMOUNT);
        });
    }

    protected override List<Tag> getTags() => new List<Tag>() { Tag.Gem };
}
