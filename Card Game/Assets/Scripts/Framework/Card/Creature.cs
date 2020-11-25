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

    // synced variables.
    public int BaseAttack { get { return (int)Stats.Stats[StatType.BaseAttack]; } set { Stats.setValue(StatType.BaseAttack, value); needToSync = true; } }
    public int BaseRange { get { return (int)Stats.Stats[StatType.BaseRange]; } set { Stats.setValue(StatType.BaseRange, value); needToSync = true; } }
    public int BaseMovement { get { return (int)Stats.Stats[StatType.BaseMovement]; } set { Stats.setValue(StatType.BaseMovement, value); needToSync = true; } }
    public int AttackStat { get { return (int)Stats.Stats[StatType.Attack]; } set { Stats.setValue(StatType.Attack, value); needToSync = true; } }
    public int Range { get { return (int)Stats.Stats[StatType.Range]; } set { Stats.setValue(StatType.Range, value); needToSync = true; } }
    public int Movement { get { return (int)Stats.Stats[StatType.Movement]; } set { Stats.setValue(StatType.Movement, value); needToSync = true; } }

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
        Stats.addType(StatType.Attack);
        Stats.addType(StatType.BaseAttack);
        Stats.addType(StatType.BaseHealth);
        Stats.addType(StatType.BaseMovement);
        Stats.addType(StatType.BaseRange);
        Stats.addType(StatType.BaseRange);
        Stats.addType(StatType.Health);
        Stats.addType(StatType.Movement);
        Stats.addType(StatType.Range);
    }

    public void resetToBaseStats()
    {
        // only owner is resposible for resetting to base stats
        // might need to write a thing to request a base stat change depending on effects added in future
        // but for now this should not be called by card scripts
            // ex: reset a creature to their base stats. The owner would need to know they need to reset because the other player isn't allowed
        if (GameManager.gameMode == GameManager.GameMode.online && NetInterface.Get().gameSetupComplete && NetInterface.Get().getLocalPlayer() != SourceCard.owner)
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
    public void resetToBaseStatsWithoutSyncing()
    {
        if (GameManager.gameMode != GameManager.GameMode.online)
        {
            Debug.LogError("Do not call this unless gameMode is online");
            resetToBaseStats(); // go ahead and call normal reset to base just to stop things from breaking
        }
        
        Health = BaseHealth;
        AttackStat = BaseAttack;
        Range = BaseRange;
        Movement = BaseMovement;
        //statsScript.updateAllStats(this);
        if (Counters != null) // counter controller will be null before awake is called (when card is created)
            Counters.clear();
    }

    #region takeDamageAndAttacking
    public void takeDamage(int damage, Card source)
    {
        if (damage == 0) // dealing 0 damage is illegal :)
            return;

        // check for ward on adjacent allies
        List<Tile> adjacentTiles = tile.getAdjacentTiles();
        foreach(Tile t in adjacentTiles)
        {
            if (t.creature != null && t.creature.hasKeyword(Keyword.Ward))
            {
                t.creature.takeWardDamage(damage);
                return;
            }
        }

        // subtract armored damage
        if (hasKeyword(Keyword.Armored1))
            damage--;
        takeDamageActual(damage, source);
    }
    public void takeWardDamage(int damage)
    {
        if (hasKeyword(Keyword.Armored1))
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
        if (NetInterface.Get().getLocalPlayer() == Controller)
            TriggerOnAttackEvents(this, new OnAttackArgs() { target = defender });
        if (NetInterface.Get().getLocalPlayer() == defender.Controller)
            defender.TriggerOnDefendEvents(this, new OnDefendArgs() { attacker = this });

        // perform damage calc
        defender.takeDamage(attackRoll, SourceCard); // do damage text in takeDamage()
        takeDamage(KeywordUtils.getDefenderValue(defender.SourceCard), SourceCard);

        // gray out creature to show it has already acted
        updateHasActedIndicators();

        // trigger after combat stuff
        // poison
        if (hasKeyword(Keyword.Poison) && defender.SourceCard is CreatureCard)
            GameManager.Get().kill((defender.SourceCard as CreatureCard).creature);
    }
    private IEnumerator attackAnimation(Damageable defender, int attackRoll)
    {
        Vector3 direction = transform.position - defender.transform.position;
        Vector2 defenderCoords = defender.getCoordinates();
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
        if (tile.x > defenderCoords.x)
            rotation = left;
        else if (tile.x < defenderCoords.x)
            rotation = right;
        else if (tile.y > defenderCoords.y)
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
        NetInterface.Get().syncCreatureCoordinates(this, this.tile.x, this.tile.y, source);
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
        if (tile == this.tile) // if the new tile is the same as the tile we're on, no moving is done
            return;
        hasMovedThisTurn = true;
        if (!hasDoneActionThisTurn)
            Controller.subtractActions(1);
        actualMove(tile);
        NetInterface.Get().syncCreatureCoordinates(this, this.tile.x, this.tile.y, null);
        GameEvents.TriggerMovedEvents(this, new GameEvents.CreatureMovedArgs() { source = null, creature = this });
        updateHasActedIndicators();
    }
    public void move(int x, int y)
    {
        Tile t = GameManager.Get().board.getTileByCoordinate(x, y);
        move(t);
    }
    // will move a creature to a tile and nothing else
    private void actualMove(Tile tile)
    {
        this.tile.removeCreature();
        tile.creature = this;
        setCoordinates(tile);
        this.tile = tile;
    }
    private void setCoordinates(Tile tile)
    {
        TransformStruct ts = new TransformStruct(SourceCard.TransformManager.transform);
        ts.position = tile.transform.position;
        SourceCard.TransformManager.moveToInformativeAnimation(ts);
    }
    #endregion

    public void updateHasActedIndicators()
    {
        if (hasDoneActionThisTurn || hasMovedThisTurn)
            setSpriteColor(new Color(.5f, .5f, .5f));
        else
            setSpriteColor(new Color(1f, 1f, 1f));
        statsScript.updateHasActedIndicator(hasDoneActionThisTurn, hasMovedThisTurn);
    }

    public void resetForNewTurn()
    {
        hasMovedThisTurn = false;
        hasDoneActionThisTurn = false;
        // change sprite color back to normal
        updateHasActedIndicators();
        // update border
        updateFriendOrFoeBorder();
    }
    public void addToCardViewer(CardViewer viewer)
    {
        statsScript.setCardViewer(viewer);
    }
    public void setSpriteColor(Color color)
    {
        SourceCard.setSpriteColor(color);
    }

    public void bounce(Card source)
    {
        hasDoneActionThisTurn = false;
        hasMovedThisTurn = false;
        updateHasActedIndicators();
        GameManager.Get().allCreatures.Remove(this);
        tile.creature = null;
        statsScript.swapToCard();
        setSpriteColor(Color.white); // reset sprite color in case card is greyed out
        resetToBaseStats();
        SourceCard.moveToCardPile(SourceCard.owner.hand, source);
        onLeavesTheField();
        SourceCard.owner.hand.resetCardPositions();
    }

    void OnMouseUpAsButton()
    {
        if (!enabled)
            return;
        if (GameManager.gameMode == GameManager.GameMode.online)
        {
            if (Controller != NetInterface.Get().getLocalPlayer() || NetInterface.Get().getLocalPlayer().isLocked())
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
                GameManager.Get().showToast("You do not have enough actions to use this unit");
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
            GameManager.Get().showToast("You can only use a creature once per turn");
        }
    }

    private bool hovered = false;
    private void OnMouseEnter()
    {
        if (!enabled)
            return;
        SourceCard.addToCardViewer(GameManager.Get().getCardViewer());
        hovered = true;
        StartCoroutine(checkHoverForTooltips());
    }
    private void OnMouseExit()
    {
        if (!enabled)
            return;
        hovered = false;
        foreach (CardViewer cv in SourceCard.viewersDisplayingThisCard)
            cv.clearToolTips();
    }

    // if you want to kill a creature do not call this. Call destroy creature in game manager
    public void sendToGrave()
    {
        resetToBaseStats();
        SourceCard.moveToCardPile(SourceCard.owner.graveyard, null);
        SourceCard.removeGraphicsAndCollidersFromScene();
    }


    // Returns true if this card has the the tag passed to this method
    public bool hasTag(Tag tag)
    {
        return SourceCard.Tags.Contains(tag);
    }

    // returns true if this card is the type passed to it
    public bool isType(CardType type)
    {
        return SourceCard.isType(type);
    }

    public Vector2 getCoordinates()
    {
        return new Vector2(tile.x, tile.y);
    }
    public void updateFriendOrFoeBorder()
    {
        if (GameManager.gameMode != GameManager.GameMode.online)
            statsScript.setAsAlly(GameManager.Get().activePlayer == Controller);
        else
            statsScript.setAsAlly(NetInterface.Get().getLocalPlayer() == Controller);
    }

    #region NetworkSyncing
    // need seperate method for this so net messages don't create an infinite loop
    public void recieveCreatureStatsFromNet(int atk, int batk, int hp, int bhp, int bmv, int brange, Player ctrl, int mv, int range)
    {
        setStatWithoutSyncing(StatType.Attack, atk);
        setStatWithoutSyncing(StatType.BaseAttack, batk);
        setStatWithoutSyncing(StatType.Health, hp);
        setStatWithoutSyncing(StatType.BaseHealth, bhp);
        setStatWithoutSyncing(StatType.BaseMovement, bmv);
        setStatWithoutSyncing(StatType.BaseRange, brange);
        setStatWithoutSyncing(StatType.Movement, mv);
        setStatWithoutSyncing(StatType.Range, range);
        Controller = ctrl;

        updateCardViewers();
    }
    private void syncCreatureStats()
    {
        // also sync stats in all related card viewers
        updateCardViewers();
        needToSync = true;
    }
    #endregion
    private void Update()
    {
        if (needToSync && NetInterface.Get().gameSetupComplete)
        {
            NetInterface.Get().syncCreatureStats(this);
            updateCardViewers();
            needToSync = false;
        }
    }

    public void updateCardViewers()
    {
        List<CardViewer> tempList = new List<CardViewer>(); // need a temp list because we might end up removing from the original list
        foreach (CardViewer cv in SourceCard.viewersDisplayingThisCard) { tempList.Add(cv); }
        foreach (CardViewer cv in tempList)
        {
            if (cv != null) // make sure the card viewer hasn't been destroyed
                cv.setCard(SourceCard);
            else // if viewer has been destroyed remove it from the view for future use
                SourceCard.viewersDisplayingThisCard.Remove(cv);
        }
    }

    #region Counters
    public void OnCountersAdded(CounterType counterType, int amount)
    {
        syncCounters(counterType);
        TriggerOnCounterAddedEvents(this, new CounterAddedArgs() { counterKind = counterType});
    }
    public void syncCounters(CounterType counterType)
    {
        NetInterface.Get().syncCounterPlaced(SourceCard, counterType, Counters.amountOf(counterType));
    }
    // used by net interface for syncing
    public void recieveCountersPlaced(CounterType counterType, int newCounters)
    {
        int currentCounters = Counters.amountOf(counterType);
        if (currentCounters > newCounters)
            Counters.remove(counterType, currentCounters - newCounters);
        else if (currentCounters < newCounters)
            Counters.add(counterType, newCounters - currentCounters);
        else
            Debug.Log("Trying to set counters to a value it's already set to. This shouldn't happen under normal circumstances");
    }
    #endregion
    #region KeywordAndToolTips
    public void addKeyword(Keyword k)
    {
        SourceCard.addKeyword(k);
    }
    public void removeKeyword(Keyword k)
    {
        SourceCard.removeKeyword(k);
    }
    public bool hasKeyword(Keyword k)
    {
        return SourceCard.hasKeyword(k);
    }
    public ReadOnlyCollection<Keyword> getKeywordList()
    {
        return SourceCard.getKeywordList();
    }
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
        foreach (CardViewer viewer in SourceCard.viewersDisplayingThisCard)
        {
            if (viewer != null)
            {
                viewer.showToolTips(SourceCard.toolTipInfos);
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
