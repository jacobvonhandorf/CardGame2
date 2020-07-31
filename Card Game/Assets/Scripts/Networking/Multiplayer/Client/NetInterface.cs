using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NetInterface
{
    private static NetInterface instance;

    private Player localPlayer;
    private bool isPlayer1;
    private int lastUsedNetId = 0; // for assigning netIds to cards
    public bool gameSetupComplete = false;
    private BiDirectioncalCardMap cardMap = new BiDirectioncalCardMap();
    private BidirectionalMap<CardPile, byte> pileIdMap = new BidirectionalMap<CardPile, byte>();
    public string selectedDeckName; // gets set before game start and is used during game setup. Might move to a game setup object
    public bool opponentFinishedSendingCards = false;
    public bool finishedWithSetup = false;

    public static NetInterface Get()
    {
        if (instance == null)
            instance = new NetInterface();
        return instance;
    }
    public static void Reset() // call when a game is complete to reset it for the next game
    {
        instance = null;
    }

    public void setIsPlayer1(bool isPlayer1)
    {
        this.isPlayer1 = isPlayer1;
    }

    public bool localPlayerIsP1()
    {
        return isPlayer1;
    }

    public Player getLocalPlayer()
    {
        return localPlayer;
    }
    public void setLocalPlayer(Player p)
    {
        localPlayer = p;
    }

    public void syncStartingDeck() // might neeed to move to a game setup object
    {
        List<Card> deck = DeckUtilities.getCardListFromFileName(selectedDeckName + ".dek");
        foreach (Card c in deck)
        {
            c.owner = localPlayer;
            c.initialize();
            syncNewCardToOpponent(c);
        }
        relayMessage(new Net_DoneSendingCards());
        foreach (Card c in deck)
            c.moveToCardPile(pileIdMap.get(isPlayer1 ? PileId.p1Deck : PileId.p2Deck), null);
        (pileIdMap.get(isPlayer1 ? PileId.p1Deck : PileId.p2Deck) as Deck).shuffle();
    }

    public void recieveGameSetupComplete()
    {
        gameSetupComplete = true;
    }

    #region Syncing
    public void importNewCardFromOpponent(Net_InstantiateCard msg)
    {
        SerializeableCard sc = msg.card;
        Card newCard = ResourceManager.Get().instantiateCardById(sc.cardId);
        newCard.owner = msg.ownerIsP1 ? getPlayer1() : getPlayer2();
        cardMap.add(newCard, sc.netId);
        newCard.removeGraphicsAndCollidersFromScene(); // remove from scene by default. Can be moved later by opponent
    }
    public void syncNewCardToOpponent(Card c)
    {
        if (isPlayer1)
            lastUsedNetId++;
        else
            lastUsedNetId--;
        cardMap.add(c, lastUsedNetId);
        SerializeableCard sc = new SerializeableCard();
        sc.netId = lastUsedNetId;
        sc.cardId = c.cardId;
        Net_InstantiateCard msg = new Net_InstantiateCard();
        msg.card = sc;
        msg.ownerIsP1 = c.owner == getPlayer1();
        relayMessage(msg);
    }
    public void syncMoveCardToPile(Card c, CardPile cp, object source)
    {
        int cardId = cardMap.get(c).netId;
        byte cpId = pileIdMap.get(cp);
        Net_MoveCardToPile msg = new Net_MoveCardToPile();
        msg.cardId = cardId;
        msg.cpId = cpId;
        if (source is Card)
            msg.sourceId = cardMap.get((Card)source).netId;
        else
            msg.sourceId = 0; // 0 is sentinal value for game mechanics/GameManager
        relayMessage(msg);
    }
    public void recieveMoveCardToPile(int cardId, byte cardPileId, int sourceId)
    {
        Card c = cardMap.get(cardId).cardObject;
        CardPile cp = pileIdMap.get(cardPileId);
        Card sourceCard = null;
        if (sourceId != 0)
            sourceCard = cardMap.get(sourceId).cardObject;
        c.syncCardMovement(cp, sourceCard);
    }
    public void syncDeckOrder(CardPile deck)
    {
        byte deckCpId = pileIdMap.get(deck);
        List<Card> cardList = deck.getCardList();
        int[] cardIds = new int[cardList.Count];
        for (int i = 0; i < cardList.Count; i++)
        {
            cardIds[i] = cardMap.get(cardList[i]).netId;
        }

        Net_SyncDeckOrder msg = new Net_SyncDeckOrder();
        msg.cardIds = cardIds;
        msg.deckCpId = deckCpId;
    }
    public void recieveDeckOrder(int[] cardIds, byte deckCpId)
    {
        CardPile deck = pileIdMap.get(deckCpId);
        List<Card> newCardList = new List<Card>();
        foreach (int cardId in cardIds)
        {
            newCardList.Add(cardMap.get(cardId).cardObject);
        }
        deck.syncOrderFromNetwork(newCardList);
    }
    public void syncCreatureCoordinates(Creature c, int x, int y, Card source)
    {
        int creatureCardId = cardMap.get(c.SourceCard).netId;
        Net_SyncCreatureCoordinates msg = new Net_SyncCreatureCoordinates();
        msg.creatureCardId = creatureCardId;
        msg.x = (byte)x;
        msg.y = (byte)y;
        if (source == null)
            msg.sourceCardId = 0;
        else
            msg.sourceCardId = cardMap.get(source).netId;
        relayMessage(msg);
    }
    public void recieveCreatureCoordinates(int creatureCardId, int x, int y, object source)
    {
        Creature c = (cardMap.get(creatureCardId).cardObject as CreatureCard).creature;

        if (source is Card)
            c.forceMove(x, y, source as Card);
        else
            c.move(x, y);
    }
    public void syncAttack(Creature attacker, Damageable defender, int damageRoll)
    {
        int attackerId = cardMap.get(attacker.SourceCard).netId;
        int defenderId = cardMap.get(defender.SourceCard).netId;

        Net_SyncAttack msg = new Net_SyncAttack();
        msg.attackerId = attackerId;
        msg.defenderId = defenderId;
        msg.damageRoll = damageRoll;
        relayMessage(msg);
    }
    public void receiveAttack(int attackerId, int defenderId, int damageRoll)
    {
        Creature attacker = (cardMap.get(attackerId).cardObject as CreatureCard).creature;
        Damageable defender;
        Card card = cardMap.get(defenderId).cardObject;
        if (card is CreatureCard)
            defender = (card as CreatureCard).creature;
        else if (card is StructureCard)
            defender = (card as StructureCard).structure;
        else
            throw new Exception("Invalid defender for attack " + card.transform);
        attacker.Attack(defender, damageRoll);
    }
    public void syncPlayerStats(Player p)
    {
        Net_SyncPlayerResources msg = new Net_SyncPlayerResources();
        msg.gold = p.getGold();
        msg.mana = p.getMana();
        msg.actions = p.GetActions();
        msg.goldPTurn = p.getGoldPerTurn();
        msg.manaPTurn = p.getManaPerTurn();
        msg.actionsPTurn = p.getActionsPerTurn();
        if (isPlayer1)
            msg.isPlayerOne = p == localPlayer;
        else
            msg.isPlayerOne = p != localPlayer;
        relayMessage(msg);
    }
    public void recievePlayerStats(bool isPlayer1, int gold, int goldPTurn, int mana, int manaPTurn, int actions, int actionsPTurn)
    {
        Debug.Log("Recieving player stats");
        if (this.isPlayer1 == isPlayer1)
        {
            localPlayer.syncStats(gold, goldPTurn, mana, manaPTurn, actions, actionsPTurn);
        }
        else
        {
            Player opposingPlayer = GameManager.Get().getOppositePlayer(localPlayer);
            opposingPlayer.syncStats(gold, goldPTurn, mana, manaPTurn, actions, actionsPTurn);
        }
    }
    public void syncEndTurn()
    {
        relayMessage(new Net_EndTurn());
    }
    public void recieveEndTurn()
    {
        GameManager.Get().startTurnForOnline();
    }
    public void syncCardStats(Card c)
    {
        Net_SyncCard msg = new Net_SyncCard();
        msg.baseGoldCost = c.BaseGoldCost;
        msg.baseManaCost = c.BaseManaCost;
        msg.goldCost = c.GoldCost;
        msg.manaCost = c.ManaCost;
        msg.elementalIdentity = c.ElementalId;
        msg.sourceCardId = cardMap.get(c).netId;
        msg.ownerIsP1 = playerIsP1(c.owner);
        relayMessage(msg);
    }
    public void recieveCardStats(Net_SyncCard msg)
    {
        Debug.Log("Recieving creature stats");
        Card c = cardMap.get(msg.sourceCardId).cardObject;
        c.BaseGoldCost = msg.baseGoldCost;
        c.BaseManaCost = msg.baseManaCost;
        c.GoldCost = msg.goldCost;
        c.ManaCost = msg.manaCost;
        c.setElementIdentity(msg.elementalIdentity);
        c.owner = msg.ownerIsP1 ? getPlayer1() : getPlayer2();
    }
    public void syncCreatureStats(Creature c)
    {
        Net_SyncCreature msg = new Net_SyncCreature();
        msg.attack = c.AttackStat;
        msg.baseAttack = c.BaseAttack;
        msg.baseHealth = c.BaseHealth;
        msg.baseMovement = c.BaseMovement;
        msg.baseRange = c.BaseRange;
        msg.controllerIsP1 = playerIsP1(c.Controller);
        msg.health = c.Health;
        msg.movement = c.Movement;
        msg.range = c.Range;
        msg.sourceCardId = cardMap.get(c.SourceCard).netId;
        relayMessage(msg);
    }
    public void recieveCreatureStats(Net_SyncCreature msg)
    {
        Creature c = (cardMap.get(msg.sourceCardId).cardObject as CreatureCard).creature;
        Player controller = msg.controllerIsP1 ? getPlayer1() : getPlayer2();
        c.recieveCreatureStatsFromNet(msg.attack, msg.baseAttack, msg.health, msg.baseHealth, msg.baseMovement, msg.baseRange, controller, msg.movement, msg.range);
    }
    public void syncStructureStats(Structure s)
    {
        Net_SyncStructure msg = new Net_SyncStructure();
        msg.baseHealth = s.BaseHealth;
        msg.controllerIsP1 = playerIsP1(s.Controller);
        msg.health = s.Health;
        msg.sourceCardId = cardMap.get(s.SourceCard).netId;
        relayMessage(msg);
    }
    public void recieveStructureStats(Net_SyncStructure msg)
    {
        Structure s = (cardMap.get(msg.sourceCardId).cardObject as StructureCard).structure;
        s.recieveStatsFromNet(msg.health, msg.baseHealth, msg.controllerIsP1 ? getPlayer1() : getPlayer2());
    }
    // card can be creature or structure
    public void syncPermanentPlaced(Card c, Tile t)
    {
        Net_SyncPermanentPlaced msg = new Net_SyncPermanentPlaced();
        msg.sourceCardId = cardMap.get(c).netId;
        msg.x = t.x;
        msg.y = t.y;
        relayMessage(msg);
    }
    public void recievePermanentPlaced(Net_SyncPermanentPlaced msg)
    {
        Card card = cardMap.get(msg.sourceCardId).cardObject as Card;
        card.setSpritesToSortingLayer(SpriteLayers.Creature); // move sprite layer down
        Tile targetTile =  GameManager.Get().board.getTileByCoordinate(msg.x, msg.y);
        if (card is CreatureCard)
            GameManager.Get().syncCreateCreatureOnTile(card as CreatureCard, targetTile, card.owner);
        else
            GameManager.Get().syncStructureOnTile(card as StructureCard, targetTile, card.owner);
    }
    public void syncCounterPlaced(Card sourceCard, CounterType counterType, int amount)
    {
        Net_SyncCountersPlaced msg = new Net_SyncCountersPlaced();
        msg.amount = amount;
        msg.counterId = (int)counterType;
        msg.targetCardId = cardMap.get(sourceCard).netId;
        relayMessage(msg);
    }
    public void recieveCounterPlaced(int amount, int counterId, int targetCardId)
    {
        Card card = cardMap.get(targetCardId).cardObject;
        CounterType counterType = (CounterType)counterId;
        if (card is StructureCard)
            (card as StructureCard).structure.recieveCountersPlaced(counterType, amount);
        else if (card is CreatureCard)
            (card as CreatureCard).creature.recieveCountersPlaced(counterType, amount);
        else
            Debug.LogError("Trying to place counters on a spell card");
    }
    public void sendSurrenderMessage()
    {
        Net_EndGame msg = new Net_EndGame();
        msg.endGameCode = EndGameCode.Quit;
        //relayMessage(msg);
        Client.Instance.SendServer(msg);
    }
    public void recieveEndGameMessage(Net_EndGame msg)
    {
        if (msg.endGameCode == EndGameCode.Disconnect)
        {
            // show opp disconnected
            GameManager.Get().showEndGamePopup("Your opponent has disconnected");
        }
        else if (msg.endGameCode == EndGameCode.Quit)
        {
            // show opp surrender
            GameManager.Get().showEndGamePopup("Your opponent has surrendered");
        }
        else
        {
            throw new Exception("Unexcpected end game code");
        }
    }
    #endregion

    public void signalSetupComplete()
    {
        relayMessage(new Net_DoneWithSetup());
    }

    private bool playerIsP1(Player p)
    {
        if (isPlayer1)
            return p == localPlayer;
        else
            return p != localPlayer;
    }

    private Player getPlayer1()
    {
        if (isPlayer1)
            return localPlayer;
        else
            return GameManager.Get().getOppositePlayer(localPlayer);
    }

    private Player getPlayer2()
    {
        if (isPlayer1)
            return GameManager.Get().getOppositePlayer(localPlayer);
        else
            return localPlayer;
    }

    public void registerCardPile(CardPile cp, bool ownedByLocalPlayer)
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
            pileIdMap.add(cp, cpId);
        else
            throw new Exception("Unknown CardPile");
    }

    private void relayMessage(NetMsg msg)
    {
        if (GameManager.Get() != null && GameManager.gameMode != GameManager.GameMode.online)
        {
            return; // don't send messages if GameMode isn't online
        }
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

    private class BiDirectioncalCardMap
    {
        Dictionary<Card, NetCard> cardToId = new Dictionary<Card, NetCard>();
        Dictionary<int, NetCard> idToCard = new Dictionary<int, NetCard>();

        public NetCard get(int netId) => idToCard[netId];
        public NetCard get(Card card) => cardToId[card];

        public void add(Card c, int id)
        {
            NetCard n = new NetCard();
            n.cardObject = c;
            n.netId = id;
            cardToId.Add(c, n);
            idToCard.Add(id, n);
        }
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
[System.Serializable]
public class SerializeableCard
{
    public int netId;
    public int cardId;
}

public class BidirectionalMap<T, U>
{
    private Dictionary<T, U> tu = new Dictionary<T, U>();
    private Dictionary<U, T> ut = new Dictionary<U, T>();

    public T get(U u) => ut[u];
    public U get(T t) => tu[t];

    // this add is technically not safe. It should throw an exception if ut already
    // contains u, but if used properly it will work as intended
    public void add(T t, U u)
    {
        tu.Add(t, u);
        ut.Add(u, t);
    }

    public void remove(T t)
    {
        U u = tu[t];
        tu.Remove(t);
        ut.Remove(u);
    }
}
