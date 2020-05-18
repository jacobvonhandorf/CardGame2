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
    //private Dictionary<CardPile, byte> pileIdMap = new Dictionary<CardPile, byte>();
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
            c.moveToCardPile(pileIdMap.get(isPlayer1 ? PileId.p1Deck : PileId.p2Deck));
        (pileIdMap.get(isPlayer1 ? PileId.p1Deck : PileId.p2Deck) as Deck).shuffle();
    }

    public void recieveGameSetupComplete()
    {
        gameSetupComplete = true;
    }

    public void setLocalPlayer(Player p)
    {
        localPlayer = p;
    }

    // syncing
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
        sc.cardId = c.getCardId();
        Net_InstantiateCard msg = new Net_InstantiateCard();
        msg.card = sc;
        msg.ownerIsP1 = c.owner == getPlayer1();
        relayMessage(msg);
    }
    public void syncMoveCardToPile(Card c, CardPile cp)
    {
        int cardId = cardMap.get(c).netId;
        byte cpId = pileIdMap.get(cp);
        Net_MoveCardToPile msg = new Net_MoveCardToPile();
        msg.cardId = cardId;
        msg.cpId = cpId;
        relayMessage(msg);
    }
    public void recieveMoveCardToPile(int cardId, byte cardPileId)
    {
        Card c = cardMap.get(cardId).cardObject;
        CardPile cp = pileIdMap.get(cardPileId);
        c.syncCardMovement(cp);
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
    public void syncCreatureCoordinates(Creature c, int x, int y, bool wasForceMove)
    {
        int creatureCardId = cardMap.get(c.sourceCard).netId;
        Net_SyncCreatureCoordinates msg = new Net_SyncCreatureCoordinates();
        msg.creatureCardId = creatureCardId;
        msg.x = (byte)x;
        msg.y = (byte)y;
        msg.wasForceMove = wasForceMove;
        relayMessage(msg);
    }
    public void recieveCreatureCoordinates(int creatureCardId, int x, int y, bool wasForceMove)
    {
        Creature c = (cardMap.get(creatureCardId).cardObject as CreatureCard).creature;
        if (wasForceMove)
            c.forceMove(x, y);
        else
            c.move(x, y);
    }
    public void syncAttack(Creature attacker, Damageable defender, int damageRoll)
    {
        int attackerId = cardMap.get(attacker.sourceCard).netId;
        int defenderId = cardMap.get(defender.getSourceCard()).netId;

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
            throw new Exception("Invalid defender for attack " + card.getRootTransform());
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
        msg.baseGoldCost = c.getBaseGoldCost();
        msg.baseManaCost = c.getBaseManaCost();
        msg.goldCost = c.getGoldCost();
        msg.manaCost = c.getManaCost();
        msg.elementalIdentity = c.getElementIdentity();
        msg.sourceCardId = cardMap.get(c).netId;
        msg.ownerIsP1 = playerIsP1(c.owner);
        relayMessage(msg);
    }
    public void recieveCardStats(Net_SyncCard msg)
    {
        Debug.Log("Recieving creature stats");
        Card c = cardMap.get(msg.sourceCardId).cardObject;
        c.setBaseGoldCost(msg.baseGoldCost);
        c.setBaseManaCost(msg.baseManaCost);
        c.setGoldCost(msg.goldCost);
        c.setManaCost(msg.manaCost);
        c.setElementIdentity(msg.elementalIdentity);
        c.owner = msg.ownerIsP1 ? getPlayer1() : getPlayer2();
    }
    public void syncCreatureStats(Creature c)
    {
        Net_SyncCreature msg = new Net_SyncCreature();
        msg.attack = c.getAttack();
        msg.baseAttack = c.baseAttack;
        msg.baseHealth = c.baseHealth;
        msg.baseMovement = c.baseMovement;
        msg.baseRange = c.baseRange;
        msg.controllerIsP1 = playerIsP1(c.controller);
        msg.health = c.getHealth();
        msg.movement = c.getMovement();
        msg.range = c.range;
        msg.sourceCardId = cardMap.get(c.sourceCard).netId;
        relayMessage(msg);
    }
    public void recieveCreatureStats(Net_SyncCreature msg)
    {
        Debug.Log("Recieve creature stats atk:" + msg.attack);
        Creature c = (cardMap.get(msg.sourceCardId).cardObject as CreatureCard).creature;
        Player controller = msg.controllerIsP1 ? getPlayer1() : getPlayer2();
        c.recieveCreatureStatsFromNet(msg.attack, msg.baseAttack, msg.health, msg.baseHealth, msg.baseMovement, msg.baseRange, controller, msg.movement, msg.range);
        /*
        c.setAttack(msg.attack);
        c.baseAttack = msg.baseAttack;
        c.baseHealth = msg.baseHealth;
        c.baseMovement = msg.movement;
        c.baseRange = msg.baseRange;
        c.controller = msg.controllerIsP1 ? getPlayer1() : getPlayer2();
        c.setHealth(msg.health);
        c.setMovement(msg.movement);
        c.range = msg.range;
        */
    }
    public void syncStructureStats(Structure s)
    {
        Net_SyncStructure msg = new Net_SyncStructure();
        msg.baseHealth = s.getBaseHealth();
        msg.controllerIsP1 = playerIsP1(s.controller);
        msg.health = s.getHealth();
        msg.sourceCardId = cardMap.get(s.sourceCard).netId;
        relayMessage(msg);
    }
    public void recieveStructureStats(Net_SyncStructure msg)
    {
        Structure s = (cardMap.get(msg.sourceCardId).cardObject as StructureCard).structure;
        s.setBaseHealth(msg.baseHealth);
        s.controller = msg.controllerIsP1 ? getPlayer1() : getPlayer2();
        s.setHealth(msg.health);
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
        Tile targetTile =  GameManager.Get().board.getTileByCoordinate(msg.x, msg.y);
        if (card is CreatureCard)
            GameManager.Get().syncCreateCreatureOnTile(card as CreatureCard, targetTile, card.owner);
        else
            GameManager.Get().syncStructureOnTile(card as StructureCard, targetTile, card.owner);
    }
    public void syncCounterPlaced(Card sourceCard, CounterClass counterType, int amount)
    {
        Net_SyncCountersPlaced msg = new Net_SyncCountersPlaced();
        msg.amount = amount;
        msg.counterId = counterType.id();
        msg.targetCardId = cardMap.get(sourceCard).netId;
        relayMessage(msg);
    }
    public void recieveCounterPlaced(int amount, int counterId, int targetCardId)
    {
        Card card = cardMap.get(targetCardId).cardObject;
        CounterClass counterType = Counters.counterMap[counterId];
        if (card is StructureCard)
            (card as StructureCard).structure.recieveCountersPlaced(counterType, amount);
        else if (card is CreatureCard)
            (card as CreatureCard).creature.recieveCountersPlaced(counterType, amount);
        else
            Debug.LogError("Trying to place counters on a spell card");
    }


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
