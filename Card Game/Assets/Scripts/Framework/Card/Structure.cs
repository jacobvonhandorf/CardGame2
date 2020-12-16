using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

// abstract class for structures as they exist on the field
public class Structure : Permanent, Damageable, ICanReceiveCounters
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
    }

    public void resetToBaseStats()
    {
        Health = BaseHealth;
    }
    public void resetToBaseStatsWithoutSyncing()
    {
        SetStatWithoutSyncing(StatType.Health, BaseHealth);
    }
    public void recieveStatsFromNet(int hp, int bhp, Player ctrl)
    {
        SetStatWithoutSyncing(StatType.Health, hp);
        SetStatWithoutSyncing(StatType.BaseHealth, bhp);

        Controller = ctrl;
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

    private bool hovered = false;
    [SerializeField] private float hoverTimeForToolTips = .5f;
    private float timePassed = 0;
    IEnumerator checkHoverForTooltips()
    {
        while (timePassed < hoverTimeForToolTips)
        {
            timePassed += Time.deltaTime;
            if (!hovered)
                yield break;
            else
                yield return null;
        }
        timePassed = 0;
        // if we get here then enough time has passed so tell cardviewers to display tooltips
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
    public virtual bool canDeployFrom() { return true; }
    #endregion
}
