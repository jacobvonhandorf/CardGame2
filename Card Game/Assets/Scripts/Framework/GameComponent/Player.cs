using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Hand hand;
    public Deck deck;
    public Graveyard graveyard;
    public bool isActivePlayer = false;
    public bool canDraw = true;
    public Creature heldCreature; // If the player clicks a creature a reference to that creature is held here (so it can be moved or attacked with)

    public List<Creature> controlledCreatures = new List<Creature>();
    public List<Structure> controlledStructures = new List<Structure>();

    [SerializeField] private TextMeshPro actionsText;
    [SerializeField] private TextMeshPro goldText;
    [SerializeField] private TextMeshPro manaText;
    [SerializeField] private TextMeshPro actionsPerTurnText;
    [SerializeField] private TextMeshPro goldPerTurnText;
    [SerializeField] private TextMeshPro manaPerTurnText;
    [SerializeField] private TextMeshPro playerNameText;
    [SerializeField] private int actionsPerTurn;
    [SerializeField] private int gold;
    [SerializeField] private int mana;
    [SerializeField] private int goldPerTurn;
    [SerializeField] private int manaPerTurn;
    [SerializeField] private int actions;

    public int numSpellsCastThisTurn = 0;
    public int numCreaturesThisTurn = 0;
    public int numStructuresThisTurn = 0;

    private State state = State.Default;
    public Effect heldEffect;
    public bool locked;

    enum State {Default, Attacking, UsingEfect} // enum for defining what action the player is in the process of doing (ex: declaring attack, declaring targets for effect)
                                                    // not used right now. Might not ever use

    private void Start()
    {
        actionsText.text = "Actions: " + actions;
        goldText.text = "Gold: " + gold;
        manaText.text = "Mana: " + mana;

        actionsPerTurnText.text = "+" + actionsPerTurn;
        goldPerTurnText.text = "+" + goldPerTurn;
        manaPerTurnText.text = "+" + manaPerTurn;
    }

    public void drawCard()
    {
        if (canDraw)
        {
            if (deck.getCardList().Count > 0)
            {
                Card cardToAdd = deck.getTopCard();
                cardToAdd.moveToCardPile(hand, null);
            }
            else
                GameManager.Get().playerHasDrawnOutDeck(this);
            //Card cardToAdd = deck.draw();
            //hand.addCard(cardToAdd);
            //cardToAdd.onCardDrawn();
        }
    }

    public void drawCards(int amount)
    {
        if (canDraw)
        {
            for (int i = 0; i < amount; i++)
                drawCard();
        }
        hand.resetCardPositions();
    }

    public void setToActivePlayer()
    {
        hand.show();
    }

    public void setToNonActivePlayer()
    {
        hand.hide();
    }

    public void startOfTurn()
    {
        numCreaturesThisTurn = 0;
        numSpellsCastThisTurn = 0;
    }

    public void doIncome()
    {
        doGoldIncome();
        doManaIncome();
        doActionIncome();
        doPowerTileIncome();
    }

    public void doPowerTileIncome()
    {
        int powerTilesControlled = GameManager.Get().board.getPowerTileCount(this);
        int opponentPowerTilesControlled = GameManager.Get().board.getPowerTileCount(GameManager.Get().getOppositePlayer(this));
        int additionalPowerTilesControlled = powerTilesControlled - opponentPowerTilesControlled;

        if (additionalPowerTilesControlled == 1)
        {
            addGold(1);
        }
        else if (additionalPowerTilesControlled == 2)
        {
            addGold(1);
            addMana(1);
            addActions(1);
        }
        else if (additionalPowerTilesControlled == 3)
        {
            addGold(2);
            addMana(2);
            addActions(1);
        }
        else if (additionalPowerTilesControlled == 4)
        {
            addGold(3);
            addMana(3);
            addActions(2);
        }

    }

    public void doGoldIncome()
    {
        addGold(goldPerTurn);
    }

    public void doManaIncome()
    {
        addMana(manaPerTurn);
    }

    public void doActionIncome()
    {
        actions = actionsPerTurn;
        actionsText.text = "Actions: " + actions;
    }

    public List<Creature> getAllControlledCreatures()
    {
        return GameManager.Get().getAllCreaturesControlledBy(this);
    }

    public void syncStats(int gold, int gpTurn, int mana, int mpTurn, int actions, int apTurn)
    {
        this.gold = gold;
        this.goldPerTurn = gpTurn;
        this.mana = mana;
        this.manaPerTurn = mpTurn;
        this.actions = actions;
        this.actionsPerTurn = apTurn;

        goldText.text = "Gold: " + gold;
        goldPerTurnText.text = "+" + goldPerTurn;
        manaText.text = "Mana: " + mana;
        manaPerTurnText.text = "+" + manaPerTurn;
        actionsText.text = "Actions: " + actions;
        actionsPerTurnText.text = "+" + actionsPerTurn;
    }

    public void addActions(int amount)
    {
        actions += amount;
        actionsText.text = "Actions: " + actions;
        syncPlayerStats();
    }
    public void addGold(int amount)
    {
        gold += amount;
        goldText.text = "Gold: " + gold;
        syncPlayerStats();
    }
    public void addMana(int amount)
    {
        mana += amount;
        manaText.text = "Mana: " + mana;
        syncPlayerStats();
    }
    public void increaseActionsPerTurn(int amount)
    {
        actionsPerTurn += amount;
        actionsPerTurnText.text = "+" + actionsPerTurn;
        syncPlayerStats();
    }
    public void increaseGoldPerTurn(int amount)
    {
        goldPerTurn += amount;
        goldPerTurnText.text = "+" + goldPerTurn;
        syncPlayerStats();
    }
    public void increaseManaPerTurn(int amount)
    {
        manaPerTurn += amount;
        manaPerTurnText.text = "+" + manaPerTurn;
        syncPlayerStats();
    }
    public void subtractActions(int num)
    {
        actions -= num;
        actionsText.text = "Actions: " + actions;
        syncPlayerStats();
    }

    public int getGold()
    {
        return gold;
    }
    public int getMana()
    {
        return mana;
    }
    public int GetActions() { return actions; }
    public int getActionsPerTurn() { return actionsPerTurn; }
    public int getGoldPerTurn() { return goldPerTurn; }
    public int getManaPerTurn() { return manaPerTurn; }
    public string getPlayerName()
    {
        return playerNameText.text;
    }

    public void readyAttack()
    {
        state = State.Attacking;
    }
    internal void readyEffect()
    {
        state = State.UsingEfect;
    }


    private void syncPlayerStats()
    {
        needToSyncStats = true;
    }
    private bool needToSyncStats = false;
    private void Update()
    {
        if (needToSyncStats && NetInterface.Get().gameSetupComplete)
        {
            needToSyncStats = false;
            NetInterface.Get().syncPlayerStats(this);
        }
    }
}
