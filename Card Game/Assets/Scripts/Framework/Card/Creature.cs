using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static Card;

public abstract class Creature : MonoBehaviour, Damageable
{
    [SerializeField] private CreatureStatsGetter statsScript;
    //[SerializeField] private ParticleSystem onAttackParticles;
    public CreatureCard sourceCard; // card associated with this creature
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

    public int effectActionCost = 1; // CURRENTLY DISABLED can be set to 0 in child class if effect shouldn't consume an action
    // public Effect activeEffect;
    public Tile currentTile;
    public Player owner;
    public Player controller;
    public bool hasMovedThisTurn = false;
    public bool hasDoneActionThisTurn = false; // action is attack or effect

    internal bool canDeployFrom = false; // if new creatures can be deployed from this creature
    private bool initialized = false;

    private CounterController counterController;


    protected void Awake()
    {
        Debug.Log("Creature awake");
        counterController = sourceCard.getCounterController();
        if (!initialized)
            initialize();
    }

    private void Start()
    {
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
    }

    public void resetToBaseStats()
    {
        // only owner is resposible for resetting to base stats
        // might need to write a thing to request a base stat change depending on effects added in future
        // but for now this should not be called by card scripts
            // ex: reset a creature to their base stats. The owner would need to know they need to reset because the other player isn't allowed
        if (GameManager.gameMode == GameManager.GameMode.online && NetInterface.Get().gameSetupComplete && NetInterface.Get().getLocalPlayer() != owner)
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
    }

    public void takeDamage(int damage)
    {
        GameManager.Get().showDamagedText(getRootTransform().position, damage);
        setHealthWithoutKilling(currentHealth - damage);
        onDamaged();
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
    public void Attack(Damageable defender, int damageRoll) // used when damage roll is fixed
    {
        hasDoneActionThisTurn = true;
        if (!hasMovedThisTurn)
            controller.subtractActions(1);
        StartCoroutine(attackAnimation(defender, damageRoll));
    }
    // stuff done after animation
    private void AttackPart2(Damageable defender, int attackRoll)
    {
        // triggered effects methods
        onAttack();
        defender.onDefend();

        // perform damage calc
        defender.takeDamage(attackRoll); // do damage text in takeDamage()

        // gray out creature to show it has already acted
        updateHasActedIndicators();
    }
    private IEnumerator attackAnimation(Damageable defender, int attackRoll)
    {
        Vector3 direction = getRootTransform().position - defender.getRootTransform().position;
        Vector2 defenderCoords = defender.getCoordinates();
        Vector3 defenderPosition = defender.getRootTransform().position;
        Transform root = getRootTransform();
        direction.Normalize();
        float attackAnimationSpeedAway = 10f;
        float attackAnimationSpeedTowards = 20f;

        // move away to wind up
        Vector3 returnTarget = getRootTransform().position;
        Vector3 awayTarget = root.position;
        awayTarget += direction * .5f;
        while (Vector3.Distance(root.position, awayTarget) > 0.05f)
        {
            root.position = Vector3.MoveTowards(root.position, awayTarget, attackAnimationSpeedAway * Time.deltaTime);
            yield return null;
        }


        // move in to hit
        while (Vector3.Distance(root.position, returnTarget) > 0.05f)
        {
            root.position =  Vector3.MoveTowards(root.position, returnTarget, attackAnimationSpeedTowards * Time.deltaTime);
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

    // Use to move a creature to a tile by an effect
    public void forceMove(Tile tile)
    {
        actualMove(tile);
        NetInterface.Get().syncCreatureCoordinates(this, currentTile.x, currentTile.y, true);
        onForceMoved();
    }
    public void forceMove(int x, int y)
    {
        Tile t = GameManager.Get().board.getTileByCoordinate(x, y);
        forceMove(t);
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
        NetInterface.Get().syncCreatureCoordinates(this, currentTile.x, currentTile.y, false);
        onMove();
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
        tile.addCreature(this);
        setCoordinates(tile);
        currentTile = tile;
    }
    private void setCoordinates(Tile tile)
    {
        Vector2 tileCoordinates = tile.transform.position;
        statsScript.cardRoot.position = tileCoordinates;
    }

    public void updateHasActedIndicators()
    {
        if (hasDoneActionThisTurn || hasMovedThisTurn)
            setSpriteColor(new Color(.5f, .5f, .5f));
        else
            setSpriteColor(new Color(1f, 1f, 1f));
        statsScript.updateHasActedIndicator(hasDoneActionThisTurn, hasMovedThisTurn);
    }

    // if overriding this you must call the base class
    public virtual void resetForNewTurn()
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

    public void bounce()
    {
        hasDoneActionThisTurn = false;
        hasMovedThisTurn = false;
        updateHasActedIndicators();
        GameManager.Get().allCreatures.Remove(this);
        currentTile.creature = null;
        statsScript.swapToCard(sourceCard);
        setSpriteColor(Color.white); // reset sprite color in case card is greyed out
        resetToBaseStats();
        owner.addCardToHandByEffect(sourceCard);
        onLeavesTheField();
    }

    void OnMouseUpAsButton()
    {
        if (GameManager.gameMode == GameManager.GameMode.online)
        {
            if (controller != NetInterface.Get().getLocalPlayer() || NetInterface.Get().getLocalPlayer().locked)
                return;
        }
        else
        {
            if (controller != GameManager.Get().activePlayer || controller.locked)
                return;
        }
        // what to do if controller is trying to do something with this creature while they have no actions
        if (controller.GetActions() <= 0)
        {
            if (hasMovedThisTurn && !hasDoneActionThisTurn)
            {
                GameManager.Get().createActionBox(this);
                GameManager.Get().setAllTilesToDefault();
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
            GameManager.Get().createActionBox(this);
            GameManager.Get().setAllTilesToDefault();
            controller.heldCreature = null;
            return;
        }

        GameManager.Get().setAllTilesToDefault();
        // in hot seat mode don't let a player move their opponents creatures
        if (GameManager.gameMode == GameManager.GameMode.hotseat && GameManager.Get().activePlayer != controller)
            return;
        controller.heldCreature = this;
        if (!hasMovedThisTurn)
        {
            List<Tile> validTiles = GameManager.Get().getMovableTilesForCreature(this);
            foreach (Tile t in validTiles)
            {
                t.setActive(true);
            }
        } else
        {
            GameManager.Get().createActionBox(this);
        }
    }

    private void OnMouseEnter()
    {
        statsScript.setCardViewer(GameManager.Get().getCardViewer());
    }

    // if you want to kill a creature do not call this. Call destroy creature in game manager
    public void sendToGrave()
    {
        sourceCard.isCreature = false;
        resetToBaseStats();
        sourceCard.moveToCardPile(owner.graveyard);
        sourceCard.removeGraphicsAndCollidersFromScene();
    }

    public Transform getRootTransform() => statsScript.cardRoot;

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
        Debug.Log("Setting attack " + value);
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

    /*
     * Returns true if this card has the the tag passed to this method
     */
    public bool hasTag(Tag tag)
    {
        return sourceCard.hasTag(tag);
    }

    /*
     * returns true if this card is the type passed to it
     */
    public bool isType(CardType type)
    {
        return sourceCard.isType(type);
    }

    public bool hasKeyword(CardKeywords keyword)
    {
        return sourceCard.hasKeyword(keyword);
    }

    public void addKeyword(CardKeywords keyword)
    {
        sourceCard.addKeyword(keyword);
    }

    public Vector2 getCoordinates()
    {
        return new Vector2(currentTile.x, currentTile.y);
    }

    public void updateFriendOrFoeBorder()
    {
        if (GameManager.gameMode != GameManager.GameMode.online)
        {
            statsScript.setAsAlly(GameManager.Get().activePlayer == controller);
        }
        else
        {
            statsScript.setAsAlly(NetInterface.Get().getLocalPlayer() == controller);
        }
    }

    public Card getSourceCard()
    {
        return sourceCard;
    }

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
    private void Update()
    {
        if (needToSync && NetInterface.Get().gameSetupComplete)
        {
            NetInterface.Get().syncCreatureStats(this);
            needToSync = false;
        }
    }

    public void updateCardViewers()
    {
        Debug.Log("Updating card viewers");
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

    public void addCounters(CounterClass counterType, int amount)
    {
        Debug.Log("Trying to add counters");
        counterController.addCounters(counterType, amount);
    }
    public void removeCounters(CounterClass counterType, int amount)
    {
        counterController.removeCounters(counterType, amount);
    }
    public int hasCounter(CounterClass counterType)
    {
        return counterController.hasCounter(counterType);
    }

    // ABSTRACT METHODS
    public abstract int getStartingRange();
    public abstract int getCardId();

    #region ExtendedMethods
    protected virtual bool getCanDeployFrom() { return false; }
    public virtual Effect getEffect() { return null; }
    public virtual List<Tag> getTags() { return new List<Tag>(); }
    public virtual void onForceMoved() { } // When moved by others
    public virtual void onMove() { } // When moved by self or others
    public virtual void onCreation() { } //ETBs
    public virtual void onDeath() { }
    public virtual void onAttack() { }
    public virtual void onDefend() { }
    public virtual void onDamaged() { }
    public virtual void onCardDrawn() { }
    public virtual void onCardAddedToHandByEffect() { }
    public virtual void onSentToGrave() { } // Whenever added to grave. death, mill or discard
    public virtual void onAnyCreatureDeath(Creature c) { }
    public virtual void onKillingACreature(Creature c) { }
    public virtual void onLeavesTheField() { }
    public virtual void onAnyCreaturePlayed(Creature c) { }
    public virtual bool additionalCanBePlayedChecks() { return true; } // if some conditions need to be met before playing this creature then do them in this method. Return true if can be played
    public virtual void onAnySpellCast(SpellCard spell) { }
    #endregion

}
