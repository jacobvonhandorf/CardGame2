using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Creature : Permanent, Damageable, ICanReceiveCounters, IScriptCreature
{
    // synced variables.
    public int BaseAttack { get { return (int)Stats.StatList[StatType.BaseAttack]; } set { Stats.SetValue(StatType.BaseAttack, value); needToSync = true; } }
    public int BaseRange { get { return (int)Stats.StatList[StatType.BaseRange]; } set { Stats.SetValue(StatType.BaseRange, value); needToSync = true; } }
    public int BaseMovement { get { return (int)Stats.StatList[StatType.BaseMovement]; } set { Stats.SetValue(StatType.BaseMovement, value); needToSync = true; } }
    public int AttackStat { get { return (int)Stats.StatList[StatType.Attack]; } set { Stats.SetValue(StatType.Attack, value); needToSync = true; } }
    public int Range { get { return (int)Stats.StatList[StatType.Range]; } set { Stats.SetValue(StatType.Range, value); needToSync = true; } }
    public int Movement { get { return (int)Stats.StatList[StatType.Movement]; } set { Stats.SetValue(StatType.Movement, value); needToSync = true; } }

    public int CardId { get { return SourceCard.CardId; } }
    public bool MoveAvailable { get; set; } = true;
    public bool ActionAvailable { get; set; } = true;

    #region Events
    public UnityEvent E_Death = new UnityEvent();

    public event EventHandler<OnAttackArgs> E_OnAttack;
    public class OnAttackArgs : EventArgs { public Damageable target { get; set; } }
    public void TriggerOnAttackEvents(object sender, OnAttackArgs args) { if (E_OnAttack != null) E_OnAttack.Invoke(sender, args); }
    #endregion

    public new void Awake()
    {
        base.Awake();
        Stats.AddType(StatType.Attack);
        Stats.AddType(StatType.BaseAttack);
        Stats.AddType(StatType.BaseHealth);
        Stats.AddType(StatType.BaseMovement);
        Stats.AddType(StatType.BaseRange);
        Stats.AddType(StatType.BaseRange);
        Stats.AddType(StatType.Health);
        Stats.AddType(StatType.Movement);
        Stats.AddType(StatType.Range);
        CanDeployFrom = false;
    }

    public void ResetToBaseStats()
    {
        // only owner is resposible for resetting to base stats
        // might need to write a thing to request a base stat change depending on effects added in future
        // but for now this should not be called by card scripts
            // ex: reset a creature to their base stats. The owner would need to know they need to reset because the other player isn't allowed
        if (GameManager.gameMode == GameManager.GameMode.online && NetInterface.Get().gameSetupComplete && NetInterface.Get().localPlayer != SourceCard.Owner)
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
        if (Counters != null) // counter controller will be null before awake is called (when card is created)
            Counters.Clear();
    }

    #region Attacking
    public List<Tile> AttackableTiles
        => Board.Instance.GetAllTilesWithinRangeOfTile(Tile, Range).Where(t => t.Permanent != null && t.Permanent is Damageable && t.Permanent.Controller != Controller).ToList();
    private Vector3 right = new Vector3(-180, -90, -90);
    private Vector3 left = new Vector3(-180, 90, -90);
    private Vector3 up = new Vector3(-90, 0, 0);
    private Vector3 down = new Vector3(-270, 0, -180);
    // stuff done before animation
    public int Attack(Damageable defender)
    {
        int attackRoll = GetAttackRoll();
        NetInterface.Get().SyncAttack(this, defender, attackRoll);
        Attack(defender, attackRoll);
        return attackRoll;
    }
    public void Attack(Damageable defender, int damageRoll) // used when damage roll has been determined
    {
        ActionAvailable = false;
        if (!MoveAvailable)
            Controller.Actions -= 1;
        StartCoroutine(AttackAnimation(defender, damageRoll));
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
        defender.TakeDamage(attackRoll, SourceCard);
        TakeDamage(KeywordUtils.GetDefenderValue(defender.SourceCard), SourceCard);

        // gray out creature to show it has already acted
        UpdateHasActedIndicators();

        // trigger after combat stuff
        // poison
        if (HasKeyword(Keyword.Poison) && defender.SourceCard is CreatureCard)
            GameManager.Instance.kill((defender.SourceCard as CreatureCard).Creature);
    }
    private IEnumerator AttackAnimation(Damageable defender, int attackRoll)
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
        if (Tile.X > defenderCoords.x)
            rotation = left;
        else if (Tile.X < defenderCoords.x)
            rotation = right;
        else if (Tile.Y > defenderCoords.y)
            rotation = down;
        else
            rotation = up;
        GameManager.Instance.PlayOnAttackParticles(defenderPosition, rotation);

        // back to normal script
        AttackPart2(defender, attackRoll);
    }
    public int GetAttackRoll()
    {
        return AttackStat;
    }
    #endregion

    #region moving
    // Use to move a creature to a tile by an effect
    public void MoveByEffect(Tile tile, Card source)
    {
        ActualMove(tile);
        NetInterface.Get().SyncCreatureCoordinates(this, this.Tile.X, this.Tile.Y, source);
        GameEvents.TriggerMovedEvents(this, new GameEvents.CreatureMovedArgs() { creature = this, source = source });
    }
    public void MoveByEffect(int x, int y, Card source)
    {
        Tile t = GameManager.Instance.board.GetTileByCoordinate(x, y);
        MoveByEffect(t, source);
    }
    // Used to have a creature move itself
    public void Move(Tile newTile)
    {
        if (newTile == Tile) // if the new tile is the same as the tile we're on, no moving is done
            return;
        MoveAvailable = false;
        if (!ActionAvailable)
            Controller.Actions -= 1;
        ActualMove(newTile);
        if (GameManager.gameMode == GameManager.GameMode.online)
            NetInterface.Get().SyncCreatureCoordinates(this, Tile.X, Tile.Y, null);
        GameEvents.TriggerMovedEvents(this, new GameEvents.CreatureMovedArgs() { source = null, creature = this });
        UpdateHasActedIndicators();
    }
    public void Move(int x, int y)
    {
        Tile t = Board.Instance.GetTileByCoordinate(x, y);
        Move(t);
    }
    public void SyncMove(Tile tile) => ActualMove(tile);
    // will move a creature to a tile and nothing else
    private void ActualMove(Tile newTile)
    {
        if (Tile != null)
            Tile.Creature = null;
        newTile.Creature = this;
        SetCoordinates(newTile);
        Tile = newTile;
    }
    #endregion

    // places the creature passed to it on the tile passed
    public void CreateOnTile(Tile tile)
    {
        CreateCreatureActual(tile);

        // sync creature creation
        if (GameManager.gameMode == GameManager.GameMode.online)
            NetInterface.Get().SyncPermanentPlaced(SourceCard, tile);

        // trigger effects that need to be triggered
        E_OnDeployed.Invoke();
        GameEvents.E_CreaturePlayed.Invoke(this);
    }

    public void SynCreatureOnTile(Tile t)
    {
        // animate showing card
        //InformativeAnimationsQueue.Instance.AddAnimation(new ShowCardCmd(card, true, this));
        CreateCreatureActual(t);
    }

    private void CreateCreatureActual(Tile tile)
    {
        // resize creature, stop treating it as a card and start treating it as a creature
        (SourceCard as CreatureCard).SwapToCreature(tile);

        // set creature to has moved and acted unless it is quick
        if (!HasKeyword(Keyword.Quick))
        {
            MoveAvailable = true;
            ActionAvailable = true;
            UpdateHasActedIndicators();
        }

        tile.Creature = this;
        Tile = tile;

        UpdateFriendOrFoeBorder();
    }

    public void UpdateHasActedIndicators()
    {
        if (!ActionAvailable || !MoveAvailable)
            CardVisual.SetColor(new Color(.5f, .5f, .5f));
        else
            CardVisual.SetColor(new Color(1, 1, 1));
    }

    public override void ResetForNewTurn()
    {
        MoveAvailable = true;
        ActionAvailable = true;
        UpdateHasActedIndicators();
        // update border
        UpdateFriendOrFoeBorder();
    }
    public void SetSpriteColor(Color color) => SourceCard.CardVisuals.SetColor(color);

    public void Bounce(Card source)
    {
        ActionAvailable = true;
        MoveAvailable = true;
        UpdateHasActedIndicators();
        Tile.Creature = null;
        SetSpriteColor(Color.white); // reset sprite color in case card is greyed out
        ResetToBaseStats();
        SourceCard.MoveToCardPile(SourceCard.Owner.Hand, source);
    }

    private void OnEnable()
    {
        GetComponentInChildren<OnMouseClickEvents>().OnMouseClick.AddListener(OnClicked);
        GetComponentInChildren<OnMouseOverEvents>().OnMouseOver.AddListener(OnHovered);
    }
    private void OnDisable()
    {
        GetComponentInChildren<OnMouseClickEvents>().OnMouseClick.RemoveListener(OnClicked);
        GetComponentInChildren<OnMouseOverEvents>().OnMouseOver.RemoveListener(OnHovered);
    }

    public void OnClicked()
    {
        if (IgnoreClickCheck())
            return;

        // "move" in place
        if (CreatureMoveControl.CurrentCreature == this)
        {
            CreatureMoveControl.Cancel();
            ActionBox.instance.Show(this);
            return;
        }

        // Clicking creature that has already moved
        if (!MoveAvailable && ActionAvailable)
        {
            ActionBox.instance.Show(this);
            return;
        }

        // Clicking creature that's done it's thing for the turn
        if (!MoveAvailable && !ActionAvailable)
        {
            Toaster.Instance.DoToast("You can only use a creature once per turn");
            return;
        }

        // Clicking a creature when nothing else is going on
        if (MoveAvailable && Controller.Actions > 0)
        {
            CreatureMoveControl.Setup(this);
            return;
        }
    }

    // ignore click if the player doesn't control the creature or the controller is locked
    private bool IgnoreClickCheck()
        => GameManager.gameMode == GameManager.GameMode.online && (Controller != NetInterface.Get().localPlayer)
                    || GameManager.gameMode == GameManager.GameMode.hotseat && GameManager.Instance.ActivePlayer != Controller
                    || Controller.IsLocked();

    public void OnHovered()
    {
        UIEvents.PermanentHovered.Invoke(SourceCard);
    }

    #region NetworkSyncing
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
    }
    private void SyncCreatureStats() => needToSync = true;
    #endregion
    private void Update()
    {
        if (needToSync && NetInterface.Get().gameSetupComplete)
        {
            NetInterface.Get().SyncCreatureStats(this);
            needToSync = false;
        }
    }

    #region ExtendedMethods
    public virtual void onKillingACreature(Creature c) { }
    public virtual bool additionalCanBePlayedChecks() { return true; } // if some conditions need to be met before playing this creature then do them in this method. Return true if can be played
    public virtual void onInitialization() { }
    #endregion
}
