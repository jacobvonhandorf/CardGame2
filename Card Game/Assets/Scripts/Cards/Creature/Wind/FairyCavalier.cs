using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyCavalier : Creature
{
    public override int cardId => 51;
    public override List<Card.Tag> getInitialTags() => new List<Card.Tag>() { Card.Tag.Fairy };

    public override void onInitialization()
    {
        E_OnDeployed += FairyCavalier_E_OnDeployed;
    }
    private void OnDestroy()
    {
        E_OnDeployed -= FairyCavalier_E_OnDeployed;
    }

    private void FairyCavalier_E_OnDeployed(object sender, System.EventArgs e)
    {
        SingleTileTargetEffect.CreateAndQueue(GameManager.Get().getAllTilesWithCreatures(controller, true), delegate (Tile t)
        {
            t.creature.bounce(SourceCard);
        });
    }
}
