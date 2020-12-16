using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NetInterface
{
    private static NetInterface instance;

    public Player localPlayer;
    private int lastUsedNetId = 0; // for assigning netIds to cards
    public bool gameSetupComplete = false;
    private BidirectionalMap<Card, int> cardMap = new BidirectionalMap<Card, int>();
    private BidirectionalMap<CardPile, byte> pileIdMap = new BidirectionalMap<CardPile, byte>();
    public string selectedDeckName; // gets set before game start and is used during game setup. Might move to a game setup object
    public bool opponentFinishedSendingCards = false;
    public bool finishedWithSetup = false;
    public bool LocalPlayerIsP1 { get; set; }

    public static NetInterface Get()
    {
        if (instance == null)
            instance = new NetInterface();
        return instance;
    }

    // call when a game is complete to reset it for the next game
    public static void Reset() => instance = null;

    public void SyncStartingDeck() // might neeed to move to a game setup object
    {
        List<Card> deck = DeckUtilities.getCardListFromFileName(selectedDeckName + ".dek");
        foreach (Card c in deck)
        {
            c.Owner = localPlayer;
            c.Initialize();
            SyncNewCardToOpponent(c);
        }
        RelayMessage(new Net_DoneSendingCards());
        foreach (Card c in deck)
            c.MoveToCardPile(pileIdMap.Get(LocalPlayerIsP1 ? PileId.p1Deck : PileId.p2Deck), null);
        (pileIdMap.Get(LocalPlayerIsP1 ? PileId.p1Deck : PileId.p2Deck) as Deck).Shuffle();
    }

    public void RecieveGameSetupComplete()
    {
        gameSetupComplete = true;
    }

    #region Syncing
    public void ImportNewCardFromOpponent(Net_InstantiateCard msg)
    {
        SerializeableCard sc = msg.card;
        Card newCard = ResourceManager.Get().InstantiateCardById(sc.cardId);
        newCard.Owner = msg.ownerIsP1 ? GetPlayer1() : GetPlayer2();
        cardMap.Add(newCard, sc.netId);
        newCard.removeGraphicsAndCollidersFromScene(); // remove from scene by default. Can be moved later by opponent
    }
    public void SyncNewCardToOpponent(Card c)
    {
        if (LocalPlayerIsP1)
            lastUsedNetId++;
        else
            lastUsedNetId--;
        cardMap.Add(c, lastUsedNetId);
        SerializeableCard sc = new SerializeableCard();
        sc.netId = lastUsedNetId;
        sc.cardId = c.CardId;
        Net_InstantiateCard msg = new Net_InstantiateCard();
        msg.card = sc;
        msg.ownerIsP1 = c.Owner == GetPlayer1();
        RelayMessage(msg);
    }
    public void SyncMoveCardToPile(Card c, CardPile cp, object source)
    {
        int cardId = cardMap.Get(c);
        byte cpId = pileIdMap.Get(cp);
        Net_MoveCardToPile msg = new Net_MoveCardToPile();
        msg.cardId = cardId;
        msg.cpId = cpId;
        if (source is Card)
            msg.sourceId = cardMap.Get((Card)source);
        else
            msg.sourceId = 0; // 0 is sentinal value for game mechanics/GameManager
        RelayMessage(msg);
    }
    public void RecieveMoveCardToPile(int cardId, byte cardPileId, int sourceId)
    {
        Card c = cardMap.Get(cardId);
        CardPile cp = pileIdMap.Get(cardPileId);
        Card sourceCard = null;
        if (sourceId != 0)
            sourceCard = cardMap.Get(sourceId);
        c.SyncCardMovement(cp, sourceCard);
    }
    public void SyncDeckOrder(CardPile deck)
    {
        byte deckCpId = pileIdMap.Get(deck);
        int[] cardIds = new int[deck.CardList.Count];
        for (int i = 0; i < deck.CardList.Count; i++)
        {
            cardIds[i] = cardMap.Get(deck.CardList[i]);
        }

        Net_SyncDeckOrder msg = new Net_SyncDeckOrder();
        msg.cardIds = cardIds;
        msg.deckCpId = deckCpId;
    }
    public void RecieveDeckOrder(int[] cardIds, byte deckCpId)
    {
        CardPile deck = pileIdMap.Get(deckCpId);
        List<Card> newCardList = new List<Card>();
        foreach (int cardId in cardIds)
        {
            newCardList.Add(cardMap.Get(cardId));
        }
        deck.SyncOrderFromNetwork(newCardList);
    }
    public void SyncCreatureCoordinates(Creature c, int x, int y, Card source)
    {
        int creatureCardId = cardMap.Get(c.SourceCard);
        Net_SyncCreatureCoordinates msg = new Net_SyncCreatureCoordinates();
        msg.creatureCardId = creatureCardId;
        msg.x = (byte)x;
        msg.y = (byte)y;
        if (source == null)
            msg.sourceCardId = 0;
        else
            msg.sourceCardId = cardMap.Get(source);
        RelayMessage(msg);
    }
    public void RecieveCreatureCoordinates(int creatureCardId, int x, int y, object source)
    {
        Creature c = (cardMap.Get(creatureCardId) as CreatureCard).Creature;

        if (source is Card)
            c.MoveByEffect(x, y, source as Card);
        else
            c.Move(x, y);
    }
    public void SyncAttack(Creature attacker, Damageable defender, int damageRoll)
    {
        int attackerId = cardMap.Get(attacker.SourceCard);
        int defenderId = cardMap.Get(defender.SourceCard);

        Net_SyncAttack msg = new Net_SyncAttack();
        msg.attackerId = attackerId;
        msg.defenderId = defenderId;
        msg.damageRoll = damageRoll;
        RelayMessage(msg);
    }
    public void ReceiveAttack(int attackerId, int defenderId, int damageRoll)
    {
        Creature attacker = (cardMap.Get(attackerId) as CreatureCard).Creature;
        Damageable defender;
        Card card = cardMap.Get(defenderId);
        if (card is CreatureCard)
            defender = (card as CreatureCard).Creature;
        else if (card is StructureCard)
            defender = (card as StructureCard).structure;
        else
            throw new Exception("Invalid defender for attack " + card.transform);
        attacker.Attack(defender, damageRoll);
    }
    public void SyncPlayerStats(Player p)
    {
        Net_SyncPlayerResources msg = new Net_SyncPlayerResources();
        msg.gold = p.Gold;
        msg.mana = p.Mana;
        msg.actions = p.Actions;
        msg.goldPTurn = p.GoldPerTurn;
        msg.manaPTurn = p.ManaPerTurn;
        msg.actionsPTurn = p.ActionsPerTurn;
        if (LocalPlayerIsP1)
            msg.isPlayerOne = p == localPlayer;
        else
            msg.isPlayerOne = p != localPlayer;
        RelayMessage(msg);
    }
    public void RecievePlayerStats(bool isPlayer1, int gold, int goldPTurn, int mana, int manaPTurn, int actions, int actionsPTurn)
    {
        Debug.Log("Recieving player stats");
        if (LocalPlayerIsP1 == isPlayer1)
        {
            localPlayer.SyncStats(gold, goldPTurn, mana, manaPTurn, actions, actionsPTurn);
        }
        else
        {
            Player opposingPlayer = GameManager.Instance.GetOppositePlayer(localPlayer);
            opposingPlayer.SyncStats(gold, goldPTurn, mana, manaPTurn, actions, actionsPTurn);
        }
    }
    public void SyncEndTurn()
    {
        RelayMessage(new Net_EndTurn());
    }
    public void RecieveEndTurn()
    {
        GameManager.Instance.startTurnForOnline();
    }
    public void SyncCardStats(Card c)
    {
        Net_SyncCard msg = new Net_SyncCard();
        msg.baseGoldCost = c.BaseGoldCost;
        msg.baseManaCost = c.BaseManaCost;
        msg.goldCost = c.GoldCost;
        msg.manaCost = c.ManaCost;
        msg.elementalIdentity = c.ElementalId;
        msg.sourceCardId = cardMap.Get(c);
        msg.ownerIsP1 = PlayerIsP1(c.Owner);
        RelayMessage(msg);
    }
    public void RecieveCardStats(Net_SyncCard msg)
    {
        Debug.Log("Recieving creature stats");
        Card c = cardMap.Get(msg.sourceCardId);
        c.BaseGoldCost = msg.baseGoldCost;
        c.BaseManaCost = msg.baseManaCost;
        c.GoldCost = msg.goldCost;
        c.ManaCost = msg.manaCost;
        c.ElementalId = msg.elementalIdentity;
        c.Owner = msg.ownerIsP1 ? GetPlayer1() : GetPlayer2();
    }
    public void SyncCreatureStats(Creature c)
    {
        Net_SyncCreature msg = new Net_SyncCreature();
        msg.attack = c.AttackStat;
        msg.baseAttack = c.BaseAttack;
        msg.baseHealth = c.BaseHealth;
        msg.baseMovement = c.BaseMovement;
        msg.baseRange = c.BaseRange;
        msg.controllerIsP1 = PlayerIsP1(c.Controller);
        msg.health = c.Health;
        msg.movement = c.Movement;
        msg.range = c.Range;
        msg.sourceCardId = cardMap.Get(c.SourceCard);
        RelayMessage(msg);
    }
    public void RecieveCreatureStats(Net_SyncCreature msg)
    {
        Creature c = (cardMap.Get(msg.sourceCardId) as CreatureCard).Creature;
        Player controller = msg.controllerIsP1 ? GetPlayer1() : GetPlayer2();
        c.RecieveCreatureStatsFromNet(msg.attack, msg.baseAttack, msg.health, msg.baseHealth, msg.baseMovement, msg.baseRange, controller, msg.movement, msg.range);
    }
    public void SyncStructureStats(Structure s)
    {
        Net_SyncStructure msg = new Net_SyncStructure();
        msg.baseHealth = s.BaseHealth;
        msg.controllerIsP1 = PlayerIsP1(s.Controller);
        msg.health = s.Health;
        msg.sourceCardId = cardMap.Get(s.SourceCard);
        RelayMessage(msg);
    }
    public void RecieveStructureStats(Net_SyncStructure msg)
    {
        Structure s = (cardMap.Get(msg.sourceCardId) as StructureCard).structure;
        s.recieveStatsFromNet(msg.health, msg.baseHealth, msg.controllerIsP1 ? GetPlayer1() : GetPlayer2());
    }
    // card can be creature or structure
    public void SyncPermanentPlaced(Card c, Tile t)
    {
        Net_SyncPermanentPlaced msg = new Net_SyncPermanentPlaced();
        msg.sourceCardId = cardMap.Get(c);
        msg.x = t.x;
        msg.y = t.y;
        RelayMessage(msg);
    }
    public void RecievePermanentPlaced(Net_SyncPermanentPlaced msg)
    {
        Card card = cardMap.Get(msg.sourceCardId) as Card;
        //card.setSpritesToSortingLayer(SpriteLayers.Creature); // move sprite layer down
        Tile targetTile =  GameManager.Instance.board.GetTileByCoordinate(msg.x, msg.y);
        if (card is CreatureCard)
            GameManager.Instance.syncCreateCreatureOnTile(card as CreatureCard, targetTile, card.Owner);
        else
            GameManager.Instance.syncStructureOnTile(card as StructureCard, targetTile, card.Owner);
    }
    public void SyncCounterPlaced(Card sourceCard, CounterType counterType, int amount)
    {
        Net_SyncCountersPlaced msg = new Net_SyncCountersPlaced();
        msg.amount = amount;
        msg.counterId = (int)counterType;
        msg.targetCardId = cardMap.Get(sourceCard);
        RelayMessage(msg);
    }
    public void RecieveCounterPlaced(int amount, int counterId, int targetCardId)
    {
        Card card = cardMap.Get(targetCardId);
        CounterType counterType = (CounterType)counterId;
        if (card is StructureCard)
            (card as StructureCard).structure.RecieveCountersPlaced(counterType, amount);
        else if (card is CreatureCard)
            (card as CreatureCard).Creature.RecieveCountersPlaced(counterType, amount);
        else
            Debug.LogError("Trying to place counters on a spell card");
    }
    public void SendSurrenderMessage()
    {
        Net_EndGame msg = new Net_EndGame();
        msg.endGameCode = EndGameCode.Quit;
        //relayMessage(msg);
        Client.Instance.SendServer(msg);
    }
    public void RecieveEndGameMessage(Net_EndGame msg)
    {
        if (msg.endGameCode == EndGameCode.Disconnect)
        {
            // show opp disconnected
            GameManager.Instance.showEndGamePopup("Your opponent has disconnected");
        }
        else if (msg.endGameCode == EndGameCode.Quit)
        {
            // show opp surrender
            GameManager.Instance.showEndGamePopup("Your opponent has surrendered");
        }
        else
        {
            throw new Exception("Unexcpected end game code");
        }
    }
    #endregion

    public void SignalSetupComplete() => RelayMessage(new Net_DoneWithSetup());

    private bool PlayerIsP1(Player p)
    {
        if (LocalPlayerIsP1)
            return p == localPlayer;
        else
            return p != localPlayer;
    }

    private Player GetPlayer1()
    {
        if (LocalPlayerIsP1)
            return localPlayer;
        else
            return GameManager.Instance.GetOppositePlayer(localPlayer);
    }

    private Player GetPlayer2()
    {
        if (LocalPlayerIsP1)
            return GameManager.Instance.GetOppositePlayer(localPlayer);
        else
            return localPlayer;
    }

    public void RegisterCardPile(CardPile cp, bool ownedByLocalPlayer)
    {
        byte cpId = 255;
        if (ownedByLocalPlayer) {
            if (cp is Deck) cpId = PileId.p1Deck;
            if (cp is Hand) cpId = PileId.p1Hand;
            if (cp is Graveyard) cpId = PileId.p1Graveyard;
        }
        else
        {
            if (cp is Deck) cpId = PileId.p2Deck;
            if (cp is Hand) cpId = PileId.p2Hand;
            if (cp is Graveyard) cpId = PileId.p2Graveyard;
        }
        if (cp is Board) cpId = PileId.field;

        if (cpId != 255)
            pileIdMap.Add(cp, cpId);
        else
            throw new Exception("Unknown CardPile");
    }

    private void RelayMessage(NetMsg msg)
    {
        if (GameManager.Instance!= null && GameManager.gameMode != GameManager.GameMode.online)
            return; // don't send messages if GameMode isn't online
        Net_InGameRelay relay = new Net_InGameRelay();
        relay.msg = msg;
        Client.Instance.SendServer(relay);
    }

    #region classes
    // variation of Card used for mapping Cards to their netIds
    private class NetCard
    {
        public int netId;
        public Card cardObject;
    }

    // enum for card pile Ids
    private static class PileId
    {
        public const byte p1Deck = 0;
        public const byte p2Deck = 1;
        public const byte p1Hand = 2;
        public const byte p2Hand = 3;
        public const byte p1Graveyard = 4;
        public const byte p2Graveyard = 5;
        public const byte p1Banished = 6; // banished and field are not implemented in Framework yet
        public const byte p2Banished = 7;
        public const byte field = 8;
        // id 255 is reserved as a sentinal value
    }
    #endregion
}

// variation of Card used for instantiated new cards
[Serializable]
public class SerializeableCard
{
    public int netId;
    public int cardId;
}

public class BidirectionalMap<T, U>
{
    private Dictionary<T, U> tu = new Dictionary<T, U>();
    private Dictionary<U, T> ut = new Dictionary<U, T>();

    public T Get(U u) => ut[u];
    public U Get(T t) => tu[t];

    // this add is technically not safe. It should throw an exception if ut already
    // contains u, but if used properly it will work as intended
    public void Add(T t, U u)
    {
        if (t == null || u == null)
        {
            Debug.Log("Adding null");
        }
        tu.Add(t, u);
        ut.Add(u, t);
    }

    public void Remove(T t)
    {
        U u = tu[t];
        tu.Remove(t);
        ut.Remove(u);
    }
}
