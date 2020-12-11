using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTileTargetEffect : IQueueableCommand
{
    public TileHandler handler;
    private List<Tile> validTargets;

    public bool IsFinished => finished;
    public bool finished;

    public SingleTileTargetEffect(List<Tile> validTargets, TileHandler handler)
    {
        this.handler = handler;
        this.validTargets = validTargets;
    }

    public void Execute()
    {
        if (validTargets.Count == 0)
        {
            GameManager.Get().ShowToast("No valid targets for effect");
            finished = true;
            return;
        }
        foreach (Tile t in validTargets)
            t.SetEffectable(this);
    }

    #region UtilityBuilders
    public static void CreateAndQueue(List<Tile> validTargets, TileHandler handler)
    {
        InformativeAnimationsQueue.Instance.AddAnimation(new SingleTileTargetEffect(validTargets, handler));
    }
    public static IQueueableCommand CreateCommand(List<Tile> validTargets, TileHandler handler)
    {
        return new SingleTileTargetEffect(validTargets, handler);
    }
    #endregion
}
