using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTileTargetEffect : QueueableCommand
{
    public TileHandler handler;
    private List<Tile> validTargets;

    public override bool IsFinished => finished;
    public bool finished;

    public SingleTileTargetEffect(List<Tile> validTargets, TileHandler handler)
    {
        this.handler = handler;
        this.validTargets = validTargets;
    }

    public override void Execute()
    {
        if (validTargets.Count == 0)
        {
            GameManager.Get().ShowToast("No valid targets for effect");
            finished = true;
            return;
        }
        foreach (Tile t in validTargets)
            t.setEffectable(this);
    }

    #region UtilityBuilders
    public static void CreateAndQueue(List<Tile> validTargets, TileHandler handler)
    {
        InformativeAnimationsQueue.Instance.addAnimation(new SingleTileTargetEffect(validTargets, handler));
    }
    public static QueueableCommand CreateCommand(List<Tile> validTargets, TileHandler handler)
    {
        return new SingleTileTargetEffect(validTargets, handler);
    }
    #endregion
}
