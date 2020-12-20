using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IScriptPlayer
{
    public Hand Hand => hand;
    public Deck Deck => deck;
    public Graveyard Graveyard => graveyard;
    public Player OppositePlayer => GameManager.Instance.GetOppositePlayer(this);
    public StatsContainer Stats { get; } = new StatsContainer();
    public List<Creature> ControlledCreatures => GameManager.Instance.getAllCreaturesControlledBy(this);
    public Dictionary<ExtraStatsKey, int> ExtraStats { get; } = new Dictionary<ExtraStatsKey, int>(); // use when scripting cards to store other stats that are attached to a player
    public Effect heldEffect;
    [HideInInspector] public bool isActivePlayer = false;
    [HideInInspector] public bool canDraw = true;
    [HideInInspector] public Creature heldCreature; // If the player clicks a creature a reference to that creature is held here (so it can be moved or attacked with)

    [SerializeField] private Hand hand;
    [SerializeField] private Deck deck;
    [SerializeField] private Graveyard graveyard;
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private StatChangePropogator statChangePropogator;
    
    public int Gold { get { return (int)Stats.GetValue(StatType.Gold); } set { SyncPlayerStats(); Stats.SetValue(StatType.Gold, value); } }
    public int Mana { get { return (int)Stats.GetValue(StatType.Mana); } set { SyncPlayerStats(); Stats.SetValue(StatType.Mana, value); } }
    public int Actions { get { return (int)Stats.GetValue(StatType.Actions); } set { SyncPlayerStats(); Stats.SetValue(StatType.Actions, value); } }
    public int GoldPerTurn { get { return (int)Stats.GetValue(StatType.GoldPerTurn); } set { SyncPlayerStats(); Stats.SetValue(StatType.GoldPerTurn, value); } }
    public int ManaPerTurn { get { return (int)Stats.GetValue(StatType.ManaPerTurn); } set { SyncPlayerStats(); Stats.SetValue(StatType.ManaPerTurn, value); } }
    public int ActionsPerTurn { get { return (int)Stats.GetValue(StatType.ActionsPerTurn); } set { SyncPlayerStats(); Stats.SetValue(StatType.ActionsPerTurn, value); } }
    public int NumSpellsCastThisTurn { get; set; } = 0;
    public int NumCreaturesThisTurn { get; set; } = 0;
    public int NumStructuresThisTurn { get; set; } = 0;

    IScriptPile IScriptPlayer.Hand => hand;
    IScriptPile IScriptPlayer.Graveyard => graveyard;
    IScriptDeck IScriptPlayer.Deck => deck;


    private void Start()
    {
        Gold = gameConfig.StartingPlayerGold;
        Mana = gameConfig.StartingPlayerMana;
        Actions = gameConfig.StartingPlayerActions;
        GoldPerTurn = gameConfig.StartingPlayerGoldIncome;
        ManaPerTurn = gameConfig.StartingPlayerManaIncome;
        ActionsPerTurn = gameConfig.StartingPlayerActionsIncome;
        statChangePropogator.Source = Stats;
    }

    public void DrawCard()
    {
        if (canDraw)
        {
            if (Deck.CardList.Count > 0)
            {
                Card cardToAdd = Deck.CardList[0];
                cardToAdd.MoveToCardPile(Hand, null);
            }
            else
                GameManager.Instance.playerHasDrawnOutDeck(this);
        }
    }

    public void DrawCards(int amount)
    {
        for (int i = 0; i < amount; i++)
            DrawCard();
    }

    public void MakeLose()
    {
        GameManager.Instance.makePlayerLose(this);
    }

    #region Locking
    private List<object> locks = new List<object>();
    public void AddLock(object newLock) => locks.Add(newLock);
    public void RemoveLock(object lockToRemove) => locks.Remove(lockToRemove);
    public bool IsLocked() => locks.Count > 0;
    #endregion

    public void StartOfTurn()
    {
        NumCreaturesThisTurn = 0;
        NumSpellsCastThisTurn = 0;
        NumStructuresThisTurn = 0;
    }

    public void DoIncome()
    {
        Gold += GoldPerTurn;
        Mana += ManaPerTurn;
        Actions = ActionsPerTurn;
        DoPowerTileIncome();
    }

    public void DoPowerTileIncome()
    {
        int powerTilesControlled = GameManager.Instance.board.getPowerTileCount(this);
        int opponentPowerTilesControlled = GameManager.Instance.board.getPowerTileCount(GameManager.Instance.GetOppositePlayer(this));
        int additionalPowerTilesControlled = powerTilesControlled - opponentPowerTilesControlled;

        if (additionalPowerTilesControlled == 1)
        {
            Gold += 1;
        }
        else if (additionalPowerTilesControlled == 2)
        {
            Gold += 1;
            Mana += 1;
            Actions += 1;
        }
        else if (additionalPowerTilesControlled == 3)
        {
            Gold += 2;
            Mana += 2;
            Actions += 1;
        }
        else if (additionalPowerTilesControlled == 4)
        {
            Gold += 3;
            Mana += 3;
            Actions += 2;
        }
    }

    public void SyncStats(int gold, int gpTurn, int mana, int mpTurn, int actions, int apTurn)
    {
        Stats.SetValue(StatType.Gold, gold);
        Stats.SetValue(StatType.GoldPerTurn, gpTurn);
        Stats.SetValue(StatType.Mana, mana);
        Stats.SetValue(StatType.ManaPerTurn, mpTurn);
        Stats.SetValue(StatType.Actions, actions);
        Stats.SetValue(StatType.ActionsPerTurn, apTurn);
    }

    private void SyncPlayerStats() => needToSyncStats = true;
    private bool needToSyncStats = false;
    private void Update()
    {
        if (needToSyncStats && NetInterface.Get().gameSetupComplete)
        {
            needToSyncStats = false;
            NetInterface.Get().SyncPlayerStats(this);
        }
    }
}
