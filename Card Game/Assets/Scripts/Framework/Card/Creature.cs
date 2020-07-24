using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static Card;

public abstract class Creature : MonoBehaviour, Damageable
{
    [SerializeField] private CreatureStatsGetter statsScript;
    public Card sourceCard { get; private set; } // card associated with this creature
    public string cardName;

    // synced variables. Serialized for debugging purposes
    [SerializeField] public int baseHealth;
    [SerializeField] public int baseAttack;
    [SerializeField] public int baseRange;
    [SerializeField] public int baseMovement;
    [SerializeField] private int currentHealth;
    [SerializeField] private int attack;
    [SerializeField] public int range;
    [SerializeField] private int movement;

    public Tile currentTile;
    public Player controller;
    public bool hasMovedThisTurn = false;
    public bool hasDoneActionThisTurn = false; // action is attack or effect
    [SerializeField] public List<CreatureActivatedEffect> activatedEffects { get; } = new List<CreatureActivatedEffect>();

    internal bool canDeployFrom = false; // if new creatures can be deployed from this creature
    private bool initialized = false;

    private CounterController counterController;

    #region Events
    public event EventHandler E_Death;
    public void TriggerOnDeathEvents(object sender) { if (E_Death != null) E_Death.Invoke(sender, EventArgs.Empty); }

    public event EventHandler E_OnDeployed;
    public void TriggerOnDeployed(object sender) { if (E_OnDeployed != null) E_OnDeployed.Invoke(sender, EventArgs.Empty); }

    public event EventHandler<OnAttackArgs> E_OnAttack;
    public class OnAttackArgs : EventArgs { public Damageable target { get; set; } }
    public delegate void onAttackHandler(OnAttackArgs e);
    public void TriggerOnAttackEvents(object sender, OnAttackArgs args) { if (E_OnAttack != null) E_OnAttack.Invoke(sender, args); }

    public event EventHandler<OnDefendArgs> E_OnDefend;
    public void TriggerOnDefendEvents(object sender, OnDefendArgs args) { if (E_OnDefend != null) E_OnDefend.Invoke(sender, args); }

    public event EventHandler<OnDamagedArgs> E_OnDamaged;
    public class OnDamagedArgs : EventArgs { public Card source { get; set; } }
    public delegate void onDamagedHandler(OnDamagedArgs e);
    public void TriggerOnDamagedEvents(object sender, OnDamagedArgs args) { if (E_OnDamaged != null) E_OnDamaged.Invoke(sender, args); }
    #endregion

    protected void Awake()
    {
        sourceCard = GetComponent<CreatureCard>();
        statsScript = GetComponent<CreatureStatsGetter>();
        counterController = GetComponentInChildren<CounterController>();
        enabled = false;
    }

    public void initialize()
    {
        if (initialized)
            return;

        statsScript.setCreatureStats(this);
        range = getStartingRange();
        baseRange = range;
        canDeployFrom = getCanDeployFrom();

        initialized = true;
        onInitialization();
    }

    public void resetToBaseStats()
    {
        // only owner is resposible for resetting to base stats
        // might need to write a thing to request a base stat change depending on effects added in future
        // but for now this should not be called by card scripts
            // ex: reset a creature to their base stats. The owner would need to know they need to reset because the other player isn't allowed
        if (GameManager.gameMode == GameManager.GameMode.online && NetInterface.Get().gameSetupComplete && NetInterface.Get().getLocalPlayer() != sourceCard.owner)
        {
            return;
        }
        setHealth(baseHealth);
        setAttack(baseAttack);
        //setArmor(baseDefense);
        range = baseRange;
        //defense = baseDefense;
        movement = baseMovement;
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
        
        currentHealth = baseHealth;
        attack = baseAttack;
        range = baseRange;
        movement = baseMovement;
        statsScript.updateAllStats(this);
        if (counterController != null) // counter controller will be null before awake is called (when card is created)
            counterController.clearAll();
    }

    #region takeDamageAndAttacking
    public void takeDamage(int damage, Card source)
    {
        if (damage == 0) // dealing 0 damage is illegal :)
            return;

        // check for ward on adjacent allies
        List<Tile> adjacentTiles = currentTile.getAdjacentTiles();
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
        setHealthWithoutKilling(currentHealth - damage);
        TriggerOnDamagedEvents(this, new OnDamagedArgs() { source = source });
        if (currentHealth <= 0)
            GameManager.Get().destroyCreature(this);
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
            controller.subtractActions(1);
        StartCoroutine(attackAnimation(defender, damageRoll));
    }
    // stuff done after animation
    private void AttackPart2(Damageable defender, int attackRoll)
    {
        // only trigger effects if the local player is the owner
        if (NetInterface.Get().getLocalPlayer() == controller)
            TriggerOnAttackEvents(this, new OnAttackArgs() { target = defender });
        if (NetInterface.Get().getLocalPlayer() == defender.getController())
            defender.TriggerOnDefendEvents(this, new OnDefendArgs() { attacker = this });

        // perform damage calc
        defender.takeDamage(attackRoll, sourceCard); // do damage text in takeDamage()
        takeDamage(KeywordUtils.getDefenderValue(defender.sourceCard), sourceCard);

        // gray out creature to show it has already acted
        updateHasActedIndicators();

        // trigger after combat stuff
        // poison
        if (hasKeyword(Keyword.Poison) && defender.sourceCard is CreatureCard)
            GameManager.Get().destroyCreature((defender.sourceCard as CreatureCard).creature);
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
        if (currentTile.x > defenderCoords.x)
            rotation = left;
        else if (currentTile.x < defenderCoords.x)
            rotation = right;
        else if (currentTile.y > defenderCoords.y)
            rotation = down;
        else
            rotation = up;
        GameManager.Get().getOnAttackParticles(defenderPosition, rotation);

        // back to normal script
        AttackPart2(defender, attackRoll);
    }
    public int getAttackRoll()
    {
        int dieRoll = UnityEngine.Random.Range(0, 6);
        if (dieRoll == 0)
            if (attack == 0) // if your attack is 0 always hit for 0
                return 0;
            else
                return Math.Max(1, attack - 1); // if attack is greater than 0 never return 0
        else if (dieRoll == 5)
            return attack + 1;
        else return attack;
    }
    #endregion

    #region moving
    // Use to move a creature to a tile by an effect
    public void forceMove(Tile tile, Card source)
    {
        actualMove(tile);
        NetInterface.Get().syncCreatureCoordinates(this, currentTile.x, currentTile.y, source);
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
        if (tile == currentTile) // if the new tile is the same as the tile we're on, no moving is done
            return;
        hasMovedThisTurn = true;
        if (!hasDoneActionThisTurn)
            controller.subtractActions(1);
        actualMove(tile);
        NetInterface.Get().syncCreatureCoordinates(this, currentTile.x, currentTile.y, null);
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
        currentTile.removeCreature();
        tile.creature = this;
        setCoordinates(tile);
        currentTile = tile;
    }
    private void setCoordinates(Tile tile)
    {
        TransformStruct ts = new TransformStruct(sourceCard.transformManager.transform);
        ts.position = tile.transform.position;
        sourceCard.transformManager.moveToInformativeAnimation(ts);
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
        sourceCard.setSpriteColor(color);
    }

    public void addEffect(CreatureActivatedEffect effect)
    {
        activatedEffects.Add(effect);
    }
    public void removeEffect(CreatureActivatedEffect effect)
    {
        activatedEffects.Remove(effect);
    }

    public void bounce(Card source)
    {
        hasDoneActionThisTurn = false;
        hasMovedThisTurn = false;
        updateHasActedIndicators();
        GameManager.Get().allCreatures.Remove(this);
        currentTile.creature = null;
        statsScript.swapToCard();
        setSpriteColor(Color.white); // reset sprite color in case card is greyed out
        resetToBaseStats();
        sourceCard.moveToCardPile(sourceCard.owner.hand, source);
        onLeavesTheField();
        sourceCard.owner.hand.resetCardPositions();
    }

    void OnMouseUpAsButton()
    {
        if (!enabled)
            return;
        if (GameManager.gameMode == GameManager.GameMode.online)
        {
            if (controller != NetInterface.Get().getLocalPlayer() || NetInterface.Get().getLocalPlayer().isLocked())
                return;
        }
        else
        {
            if (controller != GameManager.Get().activePlayer || controller.isLocked())
                return;
        }
        // what to do if controller is trying to do something with this creature while they have no actions
        if (controller.GetActions() <= 0)
        {
            if (hasMovedThisTurn && !hasDoneActionThisTurn)
            {
                ActionBox.instance.show(this);
                Board.instance.setAllTilesToDefault();
                controller.heldCreature = null;
                return;
            }
            else if (!hasMovedThisTurn && !hasDoneActionThisTurn)
            {
                GameManager.Get().showToast("You do not have enough actions to use this unit");
                return;
            }
        }

        // if this creature was already clicked once then simulate a move in place
        if (controller.heldCreature == this)
        {
            ActionBox.instance.show(this);
            Board.instance.setAllTilesToDefault();
            controller.heldCreature = null;
            return;
        }

        Board.instance.setAllTilesToDefault();
        // in hot seat mode don't let a player move their opponents creatures
        if (GameManager.gameMode == GameManager.GameMode.hotseat && GameManager.Get().activePlayer != controller)
            return;
        controller.heldCreature = this;
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
        sourceCard.addToCardViewer(GameManager.Get().getCardViewer());
        hovered = true;
        StartCoroutine(checkHoverForTooltips());
    }
    private void OnMouseExit()
    {
        if (!enabled)
            return;
        hovered = false;
        foreach (CardViewer cv in sourceCard.viewersDisplayingThisCard)
            cv.clearToolTips();
    }

    // if you want to kill a creature do not call this. Call destroy creature in game manager
    public void sendToGrave()
    {
        resetToBaseStats();
        sourceCard.moveToCardPile(sourceCard.owner.graveyard, null);
        sourceCard.removeGraphicsAndCollidersFromScene();
    }

    #region basicStatsGettersAndSetters
    // health
    public int getHealth()
    {
        return currentHealth;
    }
    public void setHealth(int value)
    {
        if (value == currentHealth)
            return;
        currentHealth = value;
        statsScript.setHealth(value, baseHealth);
        syncCreatureStats();
        if (currentHealth <= 0)
            GameManager.Get().destroyCreature(this);
    }
    private void setHealthWithoutKilling(int value)
    {
        currentHealth = value;
        statsScript.setHealth(value, baseHealth);
    }
    public void addHealth(int value)
    {
        setHealth(currentHealth + value);
    }
    // attack
    public int getAttack()
    {
        return attack;
    }
    public void setAttack(int value)
    {
        if (attack == value)
            return;
        attack = value;
        statsScript.setAttack(attack, baseAttack);
        syncCreatureStats();
    }
    public void addAttack(int value)
    {
        setAttack(attack + value);
    }
    // movement
    public void setMovement(int value)
    {
        if (movement == value)
            return;
        movement = value;
        statsScript.setMovement(value, baseMovement);
        syncCreatureStats();
    }
    public int getMovement()
    {
        return movement;
    }
    #endregion

    // Returns true if this card has the the tag passed to this method
    public bool hasTag(Tag tag)
    {
        return sourceCard.hasTag(tag);
    }

    // returns true if this card is the type passed to it
    public bool isType(CardType type)
    {
        return sourceCard.isType(type);
    }

    public Vector2 getCoordinates()
    {
        return new Vector2(currentTile.x, currentTile.y);
    }
    public Player getController()
    {
        return controller;
    }
    public void updateFriendOrFoeBorder()
    {
        if (GameManager.gameMode != GameManager.GameMode.online)
            statsScript.setAsAlly(GameManager.Get().activePlayer == controller);
        else
            statsScript.setAsAlly(NetInterface.Get().getLocalPlayer() == controller);
    }

    public Card getSourceCard()
    {
        return sourceCard;
    }

    #region NetworkSyncing
    // need seperate method for this so net messages don't create an infinite loop
    public void recieveCreatureStatsFromNet(int atk, int batk, int hp, int bhp, int bmv, int brange, Player ctrl, int mv, int range)
    {
        attack = atk;
        baseAttack = batk;
        currentHealth = hp;
        baseHealth = bhp;
        baseMovement = bmv;
        baseRange = brange;
        controller = ctrl;
        movement = mv;
        this.range = range;

        statsScript.updateAllStats(this);
        updateCardViewers();
    }
    private void syncCreatureStats()
    {
        // also sync stats in all related card viewers
        updateCardViewers();
        needToSync = true;
    }
    private bool needToSync = false;
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
        List<CardViewer> tempLlist = new List<CardViewer>(); // need a temp list because we might end up removing from the original list
        foreach (CardViewer cv in sourceCard.viewersDisplayingThisCard) { tempLlist.Add(cv); }
        foreach (CardViewer cv in tempLlist)
        {
            if (cv != null) // make sure the card viewer hasn't been destroyed
                cv.setCard(sourceCard);
            else // if viewer has been destroyed remove it from the view for future use
                sourceCard.viewersDisplayingThisCard.Remove(cv);
        }
    }

    #region Counters
    public void addCounters(CounterClass counterType, int amount)
    {
        counterController.addCounters(counterType, amount);
        syncCounters(counterType);
    }
    public void removeCounters(CounterClass counterType, int amount)
    {
        counterController.removeCounters(counterType, amount);
        syncCounters(counterType);
    }
    public int hasCounter(CounterClass counterType)
    {
        return counterController.hasCounter(counterType);
    }
    public void syncCounters(CounterClass counterType)
    {
        NetInterface.Get().syncCounterPlaced(sourceCard, counterType, counterController.hasCounter(counterType));
    }
    // used by net interface for syncing
    public void recieveCountersPlaced(CounterClass counterType, int newCounters)
    {
        int currentCounters = counterController.hasCounter(counterType);
        if (currentCounters > newCounters)
            counterController.removeCounters(counterType, currentCounters - newCounters);
        else if (currentCounters < newCounters)
            counterController.addCounters(counterType, newCounters - currentCounters);
        else
            Debug.LogError("Trying to set counters to a value it's already set to. This shouldn't happen under normal circumstances");
    }
    #endregion

    #region KeywordAndToolTips
    public void addKeyword(Keyword k)
    {
        sourceCard.addKeyword(k);
    }
    public void removeKeyword(Keyword k)
    {
        sourceCard.removeKeyword(k);
    }
    public bool hasKeyword(Keyword k)
    {
        return sourceCard.hasKeyword(k);
    }
    public ReadOnlyCollection<Keyword> getKeywordList()
    {
        return sourceCard.getKeywordList();
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
        //Debug.Log("Telling viewers to show tooltips: " + sourceCard.viewersDisplayingThisCard.Count);
        foreach (CardViewer viewer in sourceCard.viewersDisplayingThisCard)
        {
            //Debug.Log("in foreach viewer");
            if (viewer != null)
            {
                //Debug.Log("Viewer is not null");
                viewer.showToolTips(sourceCard.toolTipInfos);
            }
        }
    }
    #endregion

    #region ExtendedMethods
    public abstract int cardId { get; }
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
