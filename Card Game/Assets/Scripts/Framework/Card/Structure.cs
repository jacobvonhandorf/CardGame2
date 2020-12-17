using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

// abstract class for structures as they exist on the field
public class Structure : Permanent, Damageable, ICanReceiveCounters, IScriptStructure
{
    #region Events
    public event EventHandler E_OnLeavesField;
    public void TriggerOnLeavesField(object sender) { E_OnLeavesField?.Invoke(sender, EventArgs.Empty); }
    #endregion

    public new void Awake()
    {
        base.Awake();
        Stats.AddType(StatType.Health);
        Stats.AddType(StatType.BaseHealth);
        CanDeployFrom = true;
    }

    public void resetToBaseStats()
    {
        Health = BaseHealth;
        Controller = SourceCard.Owner;
    }
    public void resetToBaseStatsWithoutSyncing()
    {
        SetStatWithoutSyncing(StatType.Health, BaseHealth);
    }
    public void RecieveStatsFromNet(int hp, int bhp, Player ctrl)
    {
        SetStatWithoutSyncing(StatType.Health, hp);
        SetStatWithoutSyncing(StatType.BaseHealth, bhp);
        Controller = ctrl;
    }

    public void CreateOnTile(Tile tile)
    {
        CreateOnTileActual(tile);
        if (GameManager.gameMode == GameManager.GameMode.online)
            NetInterface.Get().SyncPermanentPlaced(SourceCard, tile);
        // trigger ETBS
        E_OnDeployed.Invoke();
    }

    private void CreateOnTileActual(Tile tile)
    {
        // move card from player's hand and parent it to the board
        if (Board.Instance != null)
            SourceCard.MoveToCardPile(Board.Instance, null);

        // resize structure and stop treating it as a card and start treating is as a structure
        (SourceCard as StructureCard).SwapToStructure(tile);
        
        tile.Structure = this;
        Tile = tile;

        // turn on FoF border
        UpdateFriendOrFoeBorder();
    }

    public void SyncCreateOnTile(Tile tile)
    {
        //InformativeAnimationsQueue.Instance.AddAnimation(new ShowCardCmd(card, true, this));
        CreateOnTileActual(tile);
    }


    private void OnMouseUpAsButton()
    {
        if (!enabled)
            return;
        if (GameManager.Instance.ActivePlayer != Controller || Controller.IsLocked())
            return;
        if (getEffect() == null)
            return;
        GameManager.Instance.setUpStructureEffect(this);
    }

    public override void ResetForNewTurn()
    {
        UpdateFriendOrFoeBorder();
    }
    #region Overideable
    // MAY BE OVERWRITTEN
    public virtual void onAnyStructurePlayed(Structure s) { }
    public virtual void onAnyCreaturePlayed(Structure s) { }
    public virtual void onAnyCreatureDeath(Creature c) { }
    public virtual void onAnyStructureDeath(Structure s) { }
    public virtual void onPlaced() { }
    public virtual void onRemoved() { }
    public virtual void onDefend() { }
    public virtual void onDamaged() { }
    public virtual void onTurnStart() { }
    public virtual Effect getEffect() { return null; }
    public virtual bool additionalCanBePlayedChecks() { return true; } // if some conditions need to be met before playing this structure then do them in this method. Return true if can be played
    public virtual List<Tag> getTags() { return new List<Tag>(); }
    public virtual List<KeywordData> getInitialKeywords() { return new List<KeywordData>(); }
    #endregion
}
