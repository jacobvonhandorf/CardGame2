using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static Card;

public class Creature : Permanent, Damageable, ICanReceiveCounters
{
    [SerializeField] private CreatureStatsGetter statsScript;
    public string cardName;
    public int cardId { get; set; }
    public PermanentCardVisual CardVisual { get { return (PermanentCardVisual)SourceCard.CardVisuals; } }

    // synced variables.
    public int BaseAttack { get { return (int)Stats.StatList[StatType.BaseAttack]; } set { Stats.SetValue(StatType.BaseAttack, value); needToSync = true; } }
    public int BaseRange { get { return (int)Stats.StatList[StatType.BaseRange]; } set { Stats.SetValue(StatType.BaseRange, value); needToSync = true; } }
    public int BaseMovement { get { return (int)Stats.StatList[StatType.BaseMovement]; } set { Stats.SetValue(StatType.BaseMovement, value); needToSync = true; } }
    public int AttackStat { get { return (int)Stats.StatList[StatType.Attack]; } set { Stats.SetValue(StatType.Attack, value); needToSync = true; } }
    public int Range { get { return (int)Stats.StatList[StatType.Range]; } set { Stats.SetValue(StatType.Range, value); needToSync = true; } }
    public int Movement { get { return (int)Stats.StatList[StatType.Movement]; } set { Stats.SetValue(StatType.Movement, value); needToSync = true; } }

    public bool hasMovedThisTurn = false;
    public bool hasDoneActionThisTurn = false; // action is attack or effect
    [SerializeField] public List<EmptyHandler> activatedEffects { get; } = new List<EmptyHandler>();

    public bool canDeployFrom = false; // if new creatures can be deployed from this creature

    #region Events
    public event EventHandler E_Death;
    public void TriggerOnDeathEvents(object sender) { E_Death?.Invoke(sender, EventArgs.Empty); }

    public event EventHandler E_OnDeployed;
    public void TriggerOnDeployed(object sender) { E_OnDeployed?.Invoke(sender, EventArgs.Empty); }

    public event EventHandler<OnAttackArgs> E_OnAttack;
    public class OnAttackArgs : EventArgs { public Damageable target { get; set; } }
    public void TriggerOnAttackEvents(object sender, OnAttackArgs args) { if (E_OnAttack != null) E_OnAttack.Invoke(sender, args); }

    public event EventHandler<OnDamagedArgs> E_OnDamaged;
    public void TriggerOnDamagedEvents(object sender, OnDamagedArgs args) { if (E_OnDamaged != null) E_OnDamaged.Invoke(sender, args); }
    #endregion


    public new void Awake()
    {
        base.Awake();
        statsScript = GetComponent<CreatureStatsGetter>();
        Stats.AddType(StatType.Attack);
        Stats.AddType(StatType.BaseAttack);
        Stats.AddType(StatType.BaseHealth);
        Stats.AddType(StatType.BaseMovement);
        Stats.AddType(StatType.BaseRange);
        Stats.AddType(StatType.BaseRange);
        Stats.AddType(StatType.Health);
        Stats.AddType(StatType.Movement);
        Stats.AddType(StatType.Range);
    }

    public void ResetToBaseStats()
    {
        // only owner is resposible for resetting to base stats
        // might need to write a thing to request a base stat change depending on effects added in future
        // but for now this should not be called by card scripts
            // ex: reset a creature to their base stats. The owner would need to know they need to reset because the other player isn't allowed
        if (GameManager.gameMode == GameManager.GameMode.online && NetInterface.Get().gameSetupComplete && NetInterface.Get().localPlayer != SourceCard.owner)
        {
            return;
        }
        Health = BaseHealth;
        AttackStat = BaseAttack;
        Range = BaseRange;
        Movement = BaseMovement;
    }

    // usually used for when a creature is removed from the board
    // both clients know how to do this so no need for syncing
    public void ResetToBaseStatsWithoutSyncing()
    {
        if (GameManager.gameMode != GameManager.GameMode.online)
        {
            Debug.LogError("Do not call this unless gameMode is online");
            ResetToBaseStats(); // go ahead and call normal reset to base just to stop things from breaking
        }
        
        Health = BaseHealth;
        AttackStat = BaseAttack;
        Range = BaseRange;
        Movement = BaseMovement;
        //statsScript.updateAllStats(this);
        if (Counters != null) // counter controller will be null before awake is called (when card is created)
            Counters.Clear();
    }

    #region takeDamageAndAttacking
    public void TakeDamage(int damage, Card source)
    {
        if (damage == 0) // dealing 0 damage is illegal :)
            return;

        // check for ward on adjacent allies
        List<Tile> adjacentTiles = Tile.getAdjacentTiles();
        foreach(Tile t in adjacentTiles)
        {
            if (t.creature != null && t.creature.HasKeyword(Keyword.Ward))
            {
                t.creature.takeWardDamage(damage);
                return;
            }
        }

        // subtract armored damage
        if (HasKeyword(Keyword.Armored1))
            damage--;
        takeDamageActual(damage, source);
    }
    public void takeWardDamage(int damage)
    {
        if (HasKeyword(Keyword.Armored1))
            damage--;
        takeDamageActual(damage, null);
    }
    private void takeDamageActual(int damage, Card source)
    {
        GameManager.Get().showDamagedText(transform.position, damage);
        Health -= damage;
        TriggerOnDamagedEvents(this, new OnDamagedArgs() { source = source });
        if (Health <= 0)
            GameManager.Get().kill(this);
    }
    private Vector3 right = new Vector3(-180, -90, -90);
    private Vector3 left = new Vector3(-180, 90, -90);
    private Vector3 up = new Vector3(-90, 0, 0);
    private Vector3 down = new Vector3(-270, 0, -180);
    // stuff done before animation
    public int Attack(Damageable defender)
    {
        int attackRoll = getAttackRoll();
        Attack(defender, attackRoll);
        return attackRoll;
    }
    public void Attack(Damageable defender, int damageRoll) // used when damage roll has been determined
    {
        hasDoneActionThisTurn = true;
        if (!hasMovedThisTurn)
            Controller.subtractActions(1);
        StartCoroutine(attackAnimation(defender, damageRoll));
    }
    // stuff done after animation
    private void AttackPart2(Damageable defender, int attackRoll)
    {
        // only trigger effects if the local player is the owner
        if (NetInterface.Get().localPlayer == Controller)
            TriggerOnAttackEvents(this, new OnAttackArgs() { target = defender });
        if (NetInterface.Get().localPlayer == defender.Controller)
            defender.TriggerOnDefendEvents(this, new OnDefendArgs() { attacker = this });

        // perform damage calc
        defender.TakeDamage(attackRoll, SourceCard); // do damage text in takeDamage()
        TakeDamage(KeywordUtils.getDefenderValue(defender.SourceCard), SourceCard);

        // gray out creature to show it has already acted
        UpdateHasActedIndicators();

        // trigger after combat stuff
        // poison
        if (HasKeyword(Keyword.Poison) && defender.SourceCard is CreatureCard)
            GameManager.Get().kill((defender.SourceCard as CreatureCard).Creature);
    }
    private IEnumerator attackAnimation(Damageable defender, int attackRoll)
    {
        Vector3 direction = transform.position - defender.transform.position;
        Vector2 defenderCoords = defender.Coordinates;
        Vector3 defenderPosition = defender.transform.position;
        direction.Normalize();
        float attackAnimationSpeedAway = 10f;
        float attackAnimationSpeedTowards = 20f;

        // move away to wind up
        Vector3 returnTarget = transform.position;
        Vector3 awayTarget = transform.position;
        awayTarget += direction * .5f;
        while (Vector3.Distance(transform.position, awayTarget) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, awayTarget, attackAnimationSpeedAway * Time.deltaTime);
            yield return null;
        }

        // move in to hit
        while (Vector3.Distance(transform.position, returnTarget) > 0.05f)
        {
            transform.position =  Vector3.MoveTowards(transform.position, returnTarget, attackAnimationSpeedTowards * Time.deltaTime);
            yield return null;
        }

        // kick off particle effect
        Vector3 rotation;
        if (Tile.x > defenderCoords.x)
            rotation = left;
        else if (Tile.x < defenderCoords.x)
            rotation = right;
        else if (Tile.y > defenderCoords.y)
            rotation = down;
        else
            rotation = up;
        GameManager.Get().getOnAttackParticles(defenderPosition, rotation);

        // back to normal script
        AttackPart2(defender, attackRoll);
    }
    public int getAttackRoll()
    {
        return AttackStat;
    }
    #endregion

    #region moving
    // Use to move a creature to a tile by an effect
    public void forceMove(Tile tile, Card source)
    {
        actualMove(tile);
        NetInterface.Get().SyncCreatureCoordinates(this, this.Tile.x, this.Tile.y, source);
        GameEvents.TriggerMovedEvents(this, new GameEvents.CreatureMovedArgs() { creature = this, source = source });
    }
    public void forceMove(int x, int y, Card source)
    {
        Tile t = GameManager.Get().board.getTileByCoordinate(x, y);
        forceMove(t, source);
    }
    // Used to have a creature move itself
    public void move(Tile tile)
    {
        if (tile == this.Tile) // if the new tile is the same as the tile we're on, no moving is done
            return;
        hasMovedThisTurn = true;
        if (!hasDoneActionThisTurn)
            Controller.subtractActions(1);
        actualMove(tile);
        NetInterface.Get().SyncCreatureCoordinates(this, this.Tile.x, this.Tile.y, null);
        GameEvents.TriggerMovedEvents(this, new GameEvents.CreatureMovedArgs() { source = null, creature = this });
        UpdateHasActedIndicators();
    }
    public void move(int x, int y)
    {
        Tile t = GameManager.Get().board.getTileByCoordinate(x, y);
        move(t);
    }
    // will move a creature to a tile and nothing else
    private void actualMove(Tile tile)
    {
        this.Tile.removeCreature();
        tile.creature = this;
        setCoordinates(tile);
        this.Tile = tile;
    }
    private void setCoordinates(Tile tile)
    {
        TransformStruct ts = new TransformStruct(SourceCard.TransformManager.transform);
        ts.position = tile.transform.position;
        SourceCard.TransformManager.moveToInformativeAnimation(ts);
    }
    #endregion

    public void UpdateHasActedIndicators()
    {
        if (hasDoneActionThisTurn || hasMovedThisTurn)
            setSpriteColor(new Color(.5f, .5f, .5f));
        else
            setSpriteColor(new Color(1f, 1f, 1f));
        statsScript.updateHasActedIndicator(hasDoneActionThisTurn, hasMovedThisTurn);
    }

    public void ResetForNewTurn()
    {
        hasMovedThisTurn = false;
        hasDoneActionThisTurn = false;
        // change sprite color back to normal
        UpdateHasActedIndicators();
        // update border
        UpdateFriendOrFoeBorder();
    }
    public void addToCardViewer(CardViewer viewer)
    {
        statsScript.setCardViewer(viewer);
    }
    public void setSpriteColor(Color color)
    {
        Debug.LogError("Implement set sprite color");
        //SourceCard.setSpriteColor(color);
    }

    public void Bounce(Card source)
    {
        hasDoneActionThisTurn = false;
        hasMovedThisTurn = false;
        UpdateHasActedIndicators();
        GameManager.Get().allCreatures.Remove(this);
        Tile.creature = null;
        Debug.Log("Need to add swapping to card here");
        //statsScript.swapToCard();
        setSpriteColor(Color.white); // reset sprite color in case card is greyed out
        ResetToBaseStats();
        SourceCard.MoveToCardPile(SourceCard.owner.hand, source);
        onLeavesTheField();
        SourceCard.owner.hand.resetCardPositions();
    }

    void OnMouseUpAsButton()
    {
        if (!enabled)
            return;
        if (GameManager.gameMode == GameManager.GameMode.online)
        {
            if (Controller != NetInterface.Get().localPlayer || NetInterface.Get().localPlayer.isLocked())
                return;
        }
        else
        {
            if (Controller != GameManager.Get().activePlayer || Controller.isLocked())
                return;
        }
        // what to do if controller is trying to do something with this creature while they have no actions
        if (Controller.GetActions() <= 0)
        {
            if (hasMovedThisTurn && !hasDoneActionThisTurn)
            {
                ActionBox.instance.show(this);
                Board.instance.setAllTilesToDefault();
                Controller.heldCreature = null;
                return;
            }
            else if (!hasMovedThisTurn && !hasDoneActionThisTurn)
            {
                GameManager.Get().ShowToast("You do not have enough actions to use this unit");
                return;
            }
        }

        // if this creature was already clicked once then simulate a move in place
        if (Controller.heldCreature == this)
        {
            ActionBox.instance.show(this);
            Board.instance.setAllTilesToDefault();
            Controller.heldCreature = null;
            return;
        }

        Board.instance.setAllTilesToDefault();
        // in hot seat mode don't let a player move their opponents creatures
        if (GameManager.gameMode == GameManager.GameMode.hotseat && GameManager.Get().activePlayer != Controller)
            return;
        Controller.heldCreature = this;
        if (!hasMovedThisTurn && !hasDoneActionThisTurn)
        {
            List<Tile> validTiles = GameManager.Get().getMovableTilesForCreature(this);
            foreach (Tile t in validTiles)
            {
                t.setActive(true);
            }
        }
        else if (hasMovedThisTurn && !hasDoneActionThisTurn)
        {
            ActionBox.instance.show(this);
        }
        else
        {
            GameManager.Get().ShowToast("You can only use a creature once per turn");
        }
    }

    private bool hovered = false;
    private void OnMouseEnter()
    {
        if (!enabled)
            return;
        //SourceCard.RegisterToCardViewer(GameManager.Get().getCardViewer());
        hovered = true;
        StartCoroutine(CheckHoverForTooltips());
    }
    private void OnMouseExit()
    {
        if (!enabled)
            return;
        hovered = false;
        foreach (CardViewer cv in SourceCard.viewersDisplayingThisCard)
            cv.ClearToolTips();
    }

    // if you want to kill a creature do not call this. Call destroy creature in game manager
    public void SendToGrave()
    {
        ResetToBaseStats();
        SourceCard.MoveToCardPile(SourceCard.owner.graveyard, null);
        SourceCard.removeGraphicsAndCollidersFromScene();
    }


    public void UpdateFriendOrFoeBorder()
    {
        if (GameManager.gameMode != GameManager.GameMode.online)
            CardVisual.SetIsAlly(GameManager.Get().activePlayer == Controller);
        else
            CardVisual.SetIsAlly(NetInterface.Get().localPlayer == Controller);
    }

    #region NetworkSyncing
    // need seperate method for this so net messages don't create an infinite loop
    public void RecieveCreatureStatsFromNet(int atk, int batk, int hp, int bhp, int bmv, int brange, Player ctrl, int mv, int range)
    {
        SetStatWithoutSyncing(StatType.Attack, atk);
        SetStatWithoutSyncing(StatType.BaseAttack, batk);
        SetStatWithoutSyncing(StatType.Health, hp);
        SetStatWithoutSyncing(StatType.BaseHealth, bhp);
        SetStatWithoutSyncing(StatType.BaseMovement, bmv);
        SetStatWithoutSyncing(StatType.BaseRange, brange);
        SetStatWithoutSyncing(StatType.Movement, mv);
        SetStatWithoutSyncing(StatType.Range, range);
        Controller = ctrl;

        //updateCardViewers();
    }
    private void SyncCreatureStats()
    {
        // also sync stats in all related card viewers
        //updateCardViewers();
        needToSync = true;
    }
    #endregion
    private void Update()
    {
        if (needToSync && NetInterface.Get().gameSetupComplete)
        {
            NetInterface.Get().SyncCreatureStats(this);
            //updateCardViewers();
            needToSync = false;
        }
    }

    #region Counters
    public void OnCountersAdded(CounterType counterType, int amount)
    {
        SyncCounters(counterType);
        TriggerOnCounterAddedEvents(this, new CounterAddedArgs() { counterKind = counterType});
    }
    public void SyncCounters(CounterType counterType)
    {
        NetInterface.Get().SyncCounterPlaced(SourceCard, counterType, Counters.AmountOf(counterType));
    }
    // used by net interface for syncing
    public void RecieveCountersPlaced(CounterType counterType, int newCounters)
    {
        int currentCounters = Counters.AmountOf(counterType);
        if (currentCounters > newCounters)
            Counters.Remove(counterType, currentCounters - newCounters);
        else if (currentCounters < newCounters)
            Counters.Add(counterType, newCounters - currentCounters);
        else
            Debug.Log("Trying to set counters to a value it's already set to. This shouldn't happen under normal circumstances");
    }
    #endregion
    #region KeywordAndToolTips
    public void AddKeyword(Keyword k) => SourceCard.AddKeyword(k);
    public void RemoveKeyword(Keyword k) => SourceCard.RemoveKeyword(k);
    public bool HasKeyword(Keyword k) => SourceCard.HasKeyword(k);
    public ReadOnlyCollection<Keyword> getKeywordList()
    {
        return SourceCard.getKeywordList();
    }
    [SerializeField] private float hoverTimeForToolTips = .5f;
    private float timePassed = 0;
    IEnumerator CheckHoverForTooltips()
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
        foreach (CardViewer viewer in SourceCard.viewersDisplayingThisCard)
        {
            if (viewer != null)
            {
                viewer.ShowToolTips(SourceCard.toolTipInfos);
            }
        }
    }
    #endregion

    #region ExtendedMethods
    protected virtual bool getCanDeployFrom() { return false; }
    public virtual Effect getEffect() { return null; }
    public virtual List<Tag> getInitialTags() { return new List<Tag>(); }
    public virtual void onKillingACreature(Creature c) { }
    public virtual void onLeavesTheField() { }
    public virtual bool additionalCanBePlayedChecks() { return true; } // if some conditions need to be met before playing this creature then do them in this method. Return true if can be played
    public virtual void onInitialization() { }
    public virtual int getStartingRange() { return 1; } // 1 by default
    #endregion
}
