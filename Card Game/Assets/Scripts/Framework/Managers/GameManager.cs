using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameEvents;
/*
* Master game object that controls the flow of the game
* and holds references to other game objects
*/
public class GameManager : MonoBehaviour
{
    private const int STARTING_HAND_SIZE = 5;
    private const int MAX_HAND_SIZE = 10;

    // game manager is singleton and accessible by everyone
    private static GameManager gameManager;
    public static GameManager Get() { return gameManager; }
    public static string p1DeckName;
    public static string p2DeckName;

    [SerializeField] private Player player1; // for online p1 is the local player and p2 is opponent
    [SerializeField] private Player player2; 
    public Player activePlayer;
    public Player nonActivePlayer;

    public Board board;
    public List<Creature> allCreatures;
    public List<Structure> allStructures;

    // singletone objects that GameManager is responsible for handling
    [SerializeField] private CardViewer cardPreview;
    [SerializeField] private Toaster toaster;
    [SerializeField] private GameObject glassBackground;
    [SerializeField] private TextMeshPro activePlayerText;
    [SerializeField] private MyButton cancelEffectButton;
    [SerializeField] private MyButton endTurnButton;
    [SerializeField] private DamageText damageText;
    [SerializeField] private EndGamePopUp endGamePopUp;

    // prefabs to instantiate later
    [SerializeField] public CardPicker cardPickerPrefab;
    [SerializeField] private OptionButton optionButton;
    [SerializeField] public OptionSelectBox optionSelectBoxPrefab;
    //[SerializeField] public XPickerBox xPickerPrefab;
    [SerializeField] private GameObject headquartersPrefab;

    [SerializeField] private ParticleSystem onAttackParticles;

    public bool gameInProgress = true;

    // game mode for hotseat vs online. Testing allows cards to be cast without their costs
    public enum GameMode { hotseat, online, testing}
    public static GameMode gameMode = GameMode.hotseat; // hard coded to hotseat as of now
    
    void Awake()
    {
        // set gameManager to singleton instance
        if (gameManager != null)
            throw new Exception("More than one game manager detected. Game manager should be singleton");
        gameManager = this;

        allCreatures = new List<Creature>();
    }

    // Game start
    private void Start()
    {
        switch (gameMode)
        {
            case GameMode.hotseat:
                hotSeatSetup();
                break;
            case GameMode.online:
                StartCoroutine(onlineGameSetupCoroutine());
                break;
            case GameMode.testing:
                Debug.LogError("Not implemented");
                break;
        }
    }

    private void hotSeatSetup()
    {
        throw new NotImplementedException();
        /*
        // see if decks need to be loaded and if so load them
        Debug.Log(p1DeckName);
        Debug.Log(p2DeckName);
        if (p1DeckName != null && p2DeckName != null)
            loadDecks();

        player1.drawCards(5);
        player2.drawCards(5);

        // create engineers for each player
        Tile t1 = board.getTileByCoordinate(1, 1);
        Tile t2 = board.getTileByCoordinate(6, 6);
        CreatureCard engineer1 = Instantiate(engineerPrefab).GetComponentInChildren<CreatureCard>();
        CreatureCard engineer2 = Instantiate(engineerPrefab).GetComponentInChildren<CreatureCard>();
        engineer1.owner = player1;
        engineer2.owner = player2;
        engineer1.play(t1);
        engineer2.play(t2);

        // create HQ for each player
        Tile p1HQTile = board.getTileByCoordinate(0, 0);
        Tile p2HQTile = board.getTileByCoordinate(board.boardWidth - 1, board.boardHeight - 1);
        StructureCard headquartersCard1 = Instantiate(headquartersPrefab).GetComponentInChildren<StructureCard>();
        StructureCard headquartersCard2 = Instantiate(headquartersPrefab).GetComponentInChildren<StructureCard>();
        headquartersCard1.owner = player1;
        headquartersCard2.owner = player2;
        headquartersCard1.play(p1HQTile);
        headquartersCard2.play(p2HQTile);
        (headquartersCard1.structure as Headquarters).setHeroPower(new Recharge()); // hero powers are currently hard coded. When deck lists are added change this to pull from those
        (headquartersCard2.structure as Headquarters).setHeroPower(new Recharge());

        player1.setToActivePlayer();
        player2.setToNonActivePlayer();
        // add borders to engineers indicating if they are friend or foe
        engineer1.creature.updateFriendOrFoeBorder();
        engineer2.creature.updateFriendOrFoeBorder();
        headquartersCard1.structure.updateFriendOrFoeBorder();
        headquartersCard2.structure.updateFriendOrFoeBorder();

        activePlayerText.text = activePlayer.getPlayerName() + "'s turn";
        */
    }

    private object activePlayerLock = new object();
    IEnumerator onlineGameSetupCoroutine()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        Player localPlayer = NetInterface.Get().localPlayerIsP1() ? player1 : player2;
        NetInterface.Get().setLocalPlayer(localPlayer);
        // disable end turn button if going second
        endTurnButton.gameObject.SetActive(NetInterface.Get().localPlayerIsP1());
        // lock local player if they are going second
        if (!NetInterface.Get().localPlayerIsP1())
            localPlayer.addLock(activePlayerLock);

        // send opponent starting deck
        NetInterface.Get().syncStartingDeck();
        stopwatch.Stop();
        Debug.Log("Finished instantiating cards " + stopwatch.ElapsedMilliseconds + "ms");
        // wait for opponent to finish sending their deck
        while (!NetInterface.Get().opponentFinishedSendingCards)
        {
            yield return null;
        }

        // place starting engi
        int engiCoord = NetInterface.Get().localPlayerIsP1() ? 1 : 6;
        Tile engiTile = board.getTileByCoordinate(engiCoord, engiCoord);
        Card engineer = createCardById(Engineer.CARD_ID, localPlayer);
        engineer.play(engiTile);
        (engineer as CreatureCard).creature.Counters.add(CounterType.Build, 3);

        // place HQ
        int hqCoord = NetInterface.Get().localPlayerIsP1() ? 0 : board.boardWidth - 1;
        Tile hqTile = board.getTileByCoordinate(hqCoord, hqCoord);
        Card hq = createCardById(Headquarters.CARD_ID, localPlayer);
        hq.owner = NetInterface.Get().getLocalPlayer();
        //hq.removeGraphicsAndCollidersFromScene();
        hq.play(hqTile);
        // draw starting hand
        Deck localPlayerDeck = localPlayer.deck;

        //localPlayer.drawCards(STARTING_HAND_SIZE);
        NetInterface.Get().signalSetupComplete();

        // network setup is done so wait for opponent to catch up if needed
        while (!NetInterface.Get().finishedWithSetup) { yield return null; }
        // lock local player if they are going second
        NetInterface.Get().gameSetupComplete = true;

        // starting mulligan
        List<Card> mullList = localPlayerDeck.getCardList().GetRange(0, STARTING_HAND_SIZE);
        CardPicker.CreateAndQueue(mullList, 0, STARTING_HAND_SIZE, "Select cards to return to deck", localPlayer, delegate (List<Card> cardList) 
        {
            // remove selected cards from list
            mullList.RemoveAll(c => cardList.Contains(c));
            // add more cards equal to the number mulled away
            List<Card> deckList = localPlayer.deck.getCardList();
            mullList.AddRange(deckList.GetRange(STARTING_HAND_SIZE, cardList.Count));
            foreach (Card c in mullList)
            {
                c.moveToCardPile(localPlayer.hand, null);
            }
            localPlayer.deck.shuffle();
        });
    }

    private void loadDecks()
    {
        TextMeshPro p1DeckText = player1.deck.cardCountText;
        TextMeshPro p2DeckText = player2.deck.cardCountText;
        Destroy(player1.deck.gameObject);
        Destroy(player2.deck.gameObject);

        Deck newP1Deck = DeckUtilities.getDeckFromFileName(p1DeckName + ".dek");
        Deck newP2Deck = DeckUtilities.getDeckFromFileName(p2DeckName + ".dek");

        newP1Deck.transform.parent = player1.transform;
        newP2Deck.transform.parent = player2.transform;
        player1.deck = newP1Deck;
        player2.deck = newP2Deck;
        newP1Deck.cardCountText = p1DeckText;
        newP2Deck.cardCountText = p2DeckText;
        newP1Deck.deckOwner = player1;
        newP2Deck.deckOwner = player2;
        foreach (Card c in newP1Deck.getCardList())
            c.owner = player1;
        foreach (Card c in newP2Deck.getCardList())
            c.owner = player2;
        newP1Deck.shuffle();
        newP2Deck.shuffle();
    }

    private void OnDestroy()
    {
        clearEvents();
    }

    public Card createCardById(int id, Player owner)
    {
        Card newCard = ResourceManager.Get().instantiateCardById(id);
        newCard.owner = owner;
        newCard.initialize();
        if (gameMode == GameMode.online)
            NetInterface.Get().syncNewCardToOpponent(newCard);
        return newCard;
    }

    public void destroyCard(Card c)
    {
        Debug.Log("Destroy card not implemented yet");
        Destroy(c.gameObject);
        // needs to sync card destruction if gameMode is online
    }

    // GAME FLOW METHODS
    private void takeTurn()
    {
        activePlayer.startOfTurn();
        nonActivePlayer.startOfTurn();
        beginningOfTurnEffects();
        activePlayer.doIncome();
        activePlayer.drawCard();
    }

    #region TriggeredEffects
    private void beginningOfTurnEffects()
    {
        TriggerTurnStartEvents(this);
    }
    private void endOfTurnEffects()
    {
        TriggerTurnEndEvents(this);
    }
    public void onSpellCastEffects(SpellCard spell)
    {
        TriggerSpellCastEvents(this, new SpellCastArgs() { spell = spell });
    }
    #endregion

    private List<Card> getAllCardsNotInPlay()
    {
        List<Card> allCards = new List<Card>(); // all cards not in play
        allCards.AddRange(player1.hand.getCardList());
        allCards.AddRange(player2.hand.getCardList());
        allCards.AddRange(player1.graveyard.getCardList());
        allCards.AddRange(player2.graveyard.getCardList());
        allCards.AddRange(player1.deck.getCardList());
        allCards.AddRange(player2.deck.getCardList());
        return allCards;
    }

    public void doAttackOn(Damageable defender)
    {
        Creature attacker = activePlayer.heldCreature;
        int damageRoll = attacker.Attack(defender);
        NetInterface.Get().syncAttack(attacker, defender, damageRoll);
        Board.instance.setAllTilesToDefault();
    }

    private void switchActivePlayer()
    {
        Player tempPlayer = activePlayer;
        activePlayer = nonActivePlayer;
        nonActivePlayer = tempPlayer;
        activePlayer.isActivePlayer = true;
        nonActivePlayer.isActivePlayer = false;

        activePlayer.setToActivePlayer();
        nonActivePlayer.setToNonActivePlayer();

        activePlayerText.text = activePlayer.getPlayerName() + "'s turn";
    }

    public void endTurn()
    {
        if (gameMode == GameMode.online)
        {
            if (activePlayer.isLocked())
            {
                showToast("Must finish resolving effects before ending turn");
                return;
            }
            // called when button is pressed
            // disable button
            endTurnButton.gameObject.SetActive(false);
            // lock local player
            NetInterface.Get().getLocalPlayer().addLock(activePlayerLock);
            // reset player for new turn
            NetInterface.Get().getLocalPlayer().startOfTurn();
            // trigger effects
            endOfTurnEffects();
            switchActivePlayer();
            foreach (Creature c in allCreatures)
                c.resetForNewTurn();
            foreach (Structure s in allStructures)
                s.resetForNewTurn();
            ActionBox.instance.gameObject.SetActive(false);
            // send opponent message
            NetInterface.Get().syncEndTurn();
            return;
        }

        if (activePlayer.isLocked())
        {
            showToast("Must finish resolving effects before ending turn");
            return;
        }
        endOfTurnEffects();
        switchActivePlayer();
        foreach (Creature c in allCreatures)
            c.resetForNewTurn();
        foreach (Structure s in allStructures)
            s.resetForNewTurn();
        ActionBox.instance.gameObject.SetActive(false);
        takeTurn();
    }

    // called when recieving end turn message
    public void startTurnForOnline()
    {
        if (gameMode != GameMode.online)
            throw new Exception("Only call this for online play");
        // trigger end turn effects
        TriggerTurnEndEvents(this);
        // re enable end turn
        endTurnButton.gameObject.SetActive(true);
        // do income and trigger effects
        switchActivePlayer();
        Player localPlayer = NetInterface.Get().getLocalPlayer();
        localPlayer.startOfTurn();
        localPlayer.doIncome();
        localPlayer.drawCard();
        beginningOfTurnEffects();
        // unlock local player
        NetInterface.Get().getLocalPlayer().removeLock(activePlayerLock);
    }

    public List<Tile> getMovableTilesForCreature(Creature creature)
    {
        return board.getAllMovableTiles(creature);
    }

    // Returns a list of all tiles that a creature could be deployed to
    public List<Tile> getAllDeployableTiles(Player player)
    {
        List<Tile> returnList = new List<Tile>();
        foreach(Tile tile in board.allTiles)
        {
            if (tile.creature != null)
                continue;

            int x1 = tile.x + 1;
            int y1 = tile.y;
            if (canDeployFrom(board.getTileByCoordinate(x1, y1), player))
                if (!returnList.Contains(tile))
                    returnList.Add(tile);

            int x2 = tile.x - 1;
            int y2 = tile.y;
            if (canDeployFrom(board.getTileByCoordinate(x2, y2), player))
                if (!returnList.Contains(tile))
                    returnList.Add(tile);

            int x3 = tile.x;
            int y3 = tile.y + 1;
            if (canDeployFrom(board.getTileByCoordinate(x3, y3), player))
                if (!returnList.Contains(tile))
                    returnList.Add(tile);

            int x4 = tile.x;
            int y4 = tile.y - 1;
            if (canDeployFrom(board.getTileByCoordinate(x4, y4), player))
                if (!returnList.Contains(tile))
                    returnList.Add(tile);
        }

        // remove tiles that are already occupied
        returnList = returnList.Except(getAllTilesWithCreatures(true)).ToList();
        returnList = returnList.Except(getAllTilesWithStructures()).ToList();

        return returnList;
    }
    public List<Tile> getLegalStructurePlacementTiles(Player player)
    {
        Debug.Log("Getting placement tiles for " + player);
        List<Tile> returnList = new List<Tile>();
        List<Tile> invalidTiles = new List<Tile>();
        // add all possible placement tiles to the list
        foreach (Structure structure in board.getAllStructures(player))
        {
            returnList.AddRange(board.getAllTilesWithinExactRangeOfTile(structure.tile, 2));
            // also get a list of already used tiles
            invalidTiles.Add(structure.tile);
            // also get a list of tiles adjacent to structures
            invalidTiles.AddRange(board.getAllTilesWithinExactRangeOfTile(structure.tile, 1));
        }

        // remove all invalid tiles
        returnList = returnList.Except(invalidTiles).ToList();
        returnList = returnList.Except(getAllTilesWithCreatures(true)).ToList();
        returnList = returnList.Except(board.getAllTilesOnRow(3)).ToList();
        returnList = returnList.Except(board.getAllTilesOnRow(4)).ToList();

        return returnList;
    }
    // returns true if the tile is something that can be returned from
    private bool canDeployFrom(Tile tile, Player player)
    {
        if (tile == null)
            return false;
        if (tile.structure != null && tile.structure.controller == player && tile.structure.canDeployFrom())
            return true;
        if (tile.creature != null && tile.creature.controller == player && tile.creature.canDeployFrom)
            return true;
        return false;
    }

    // places teh creature passed to it on the tile passed
    public void createCreatureOnTile(Creature creature, Tile tile, Player owner)
    {
        //creature.initialize();
        createCreatureActual(tile, owner, creature);
        // sync creature creation
        NetInterface.Get().syncPermanentPlaced(creature.SourceCard, tile);

        // trigger effects that need to be triggered
        creature.TriggerOnDeployed(this);
        TriggerCreaturePlayedEvents(this, new CreaturePlayedArgs() { creature = creature });
    }

    private void createCreatureActual(Tile tile, Player owner, Creature creature)
    {
        // resize creature, stop treating it as a card and start treating it as a creature
        (creature.SourceCard as CreatureCard).swapToCreature(tile);

        // place creature in correct location
        Vector3 newPostion = creature.transform.position;
        newPostion.z = 1; // change z so that the card is always above tile and can be clicked
        creature.transform.position = newPostion;
        creature.SourceCard.TransformManager.enabled = true;

        // set owner if it hasn't been set already
        creature.controller = owner;

        // set creature to has moved and acted unless it is quick
        if (!creature.hasKeyword(Keyword.Quick))
        {
            creature.hasMovedThisTurn = true;
            creature.hasDoneActionThisTurn = true;
            creature.updateHasActedIndicators();
        }

        tile.creature = creature;
        creature.tile = tile;
        // add creature to all creatures
        allCreatures.Add(creature);

        creature.updateFriendOrFoeBorder();
    }

    public void syncCreateCreatureOnTile(CreatureCard card, Tile tile, Player owner)
    {
        Debug.Log("Syncing creature placement");
        // animate showing card
        InformativeAnimationsQueue.instance.addAnimation(new ShowCardCmd(card, true, this));
        createCreatureActual(tile, owner, card.creature);
    }
    private bool showCardFinished = false;
    private class ShowCardCmd : QueueableCommand
    {
        Card card;
        bool fromTop;
        GameManager gameManager;

        public ShowCardCmd(Card card, bool fromTop, GameManager gameManager)
        {
            this.card = card;
            this.fromTop = fromTop;
            this.gameManager = gameManager;
        }

        public override bool isFinished => Get().showCardFinished;

        public override void execute()
        {
            gameManager.StartCoroutine(gameManager.showCardCrt(card, fromTop));
        }
    }
    IEnumerator showCardCrt(Card card, bool fromTop)
    {
        showCardFinished = false;
        TransformManager tm = card.TransformManager;
        tm.clearQueue();
        tm.Lock();
        // set starting location
        if (fromTop)
            card.transform.position = new Vector3(0, 10);
        else
            card.transform.position = new Vector3(0, -10);

        // move to center of screen
        Transform cardTransform = card.TransformManager.transform;
        Vector3 target = new Vector3(0, 0);
        while (Vector3.Distance(target, cardTransform.position) > 0.02f)
        {
            cardTransform.position = Vector3.Lerp(cardTransform.position, target, 15f * Time.deltaTime);
            yield return null;
        }
        // TODO play animation/sound

        // pause
        yield return new WaitForSeconds(.3f);

        // flip bool to signal finished
        tm.UnLock();
        showCardFinished = true;
    }

    public void createStructureOnTile(Structure structure, Tile tile, Player controller, StructureCard sourceCard)
    {
        createStructureOnTileActual(structure, tile, controller);
        NetInterface.Get().syncPermanentPlaced(sourceCard, tile);
        // trigger ETBS
        structure.TriggerOnDeployEvents(this);
    }

    private void createStructureOnTileActual(Structure structure, Tile tile, Player controller)
    {
        Card sourceCard = structure.SourceCard;

        // resize structure and stop treating it as a card and start treating is as a structure
        (sourceCard as StructureCard).swapToStructure(tile);

        // place structure in correct location
        Vector3 newPosition = structure.transform.position;
        newPosition.z = 1;
        structure.transform.position = newPosition;

        // parent structure to board
        structure.transform.SetParent(board.transform);

        // move card from player's hand and parent it to the board
        sourceCard.moveToCardPile(board, null);

        tile.structure = structure;
        structure.tile = tile;
        structure.controller = controller;
        if (sourceCard.owner != null) // normal cards will already have an owner
            structure.SourceCard.owner = sourceCard.owner;
        else // "token" cards will not have an owner at this point so just use the controller
        {
            structure.SourceCard.owner = controller;
            sourceCard.owner = controller;
        }

        // turn on FoF border
        structure.updateFriendOrFoeBorder();

        // add structure to all structures
        allStructures.Add(structure);
    }

    public void syncStructureOnTile(StructureCard card, Tile tile, Player owner)
    {
        InformativeAnimationsQueue.instance.addAnimation(new ShowCardCmd(card, true, this));
        createStructureOnTileActual(card.structure, tile, owner);
    }

    /*
     * When a creatures health drops below 1 this method sends it to grave and is responsible for triggering all effects
     */
    public void kill(Permanent permanent)
    {
        switch (permanent)
        {
            case Creature creature:
                kill(creature);
                break;
            case Structure structure:
                kill(structure);
                break;
            default:
                throw new Exception("Unexcepted permanent type");
        }
    }

    public void kill(Creature creature)
    {
        Debug.Log("Destroying creature");
        if (creature.tile != null) // this check is needed to make some online stuff work
        {
            creature.tile.creature = null;
            creature.tile = null;
        }
        TriggerCreatureDeathEvents(this, new CreatureDeathArgs() { creature = creature });

        allCreatures.Remove(creature);
        creature.TriggerOnDeathEvents(this);
        creature.onLeavesTheField();
        creature.sendToGrave();
    }

    public void kill(Structure structure)
    {
        Debug.Log("Destroying structure");
        structure.tile.structure = null;
        structure.tile = null;
        allStructures.Remove(structure);
        structure.sendToGrave(null);
        structure.onRemoved();
    }

    public Player getOppositePlayer(Player player)
    {
        if (player == activePlayer)
            return nonActivePlayer;
        else
            return activePlayer;
    }

    public void makePlayerLose(Player player)
    {
        showToast("Player " + getOppositePlayer(player).name + " wins!");
    }

    public void makePlayerWin(Player player)
    {
        showToast("Player " + player.name + " wins!");
    }

    public void surrender()
    {
        NetInterface.Get().sendSurrenderMessage();
        // probably want to start a coroutine here that handles end of match
        NetInterface.Reset();
        SceneManager.LoadScene(ScenesEnum.MMDeckSelect);
    }

    public void setUpCreatureEffect(Creature creature)
    {
        // check for creature not having an effect
        if (creature.activatedEffects.Count == 0)
        {
            showToast("This creature has no effect");
            return;
        }
        else if (creature.activatedEffects.Count > 1)
        {
            throw new Exception("Not implemented");
        }
        else
        {
            // there is only 1 effect so just activate it
            // creature.activatedEffects[0].doEffect(this);
            creature.activatedEffects[0].Invoke();
        }
    }

    public void setUpStructureEffect(Structure structure)
    {
        Debug.Log("Setting up structure effect");
        Effect effect = structure.getEffect();
        if (effect == null)
        {
            showToast("This structure has no effect");
            return;
        }
        else
        {
            List<string> options = new List<string>();
            options.Add("Yes");
            options.Add("No");
            OptionSelectBox.CreateAndQueue(options, "Are you sure you want to activate the effect of " + structure.getCardName(), structure.controller, delegate (int selectedIndex, string selectedOption)
            {
                if (selectedIndex == 0)
                    effect.activate(structure.controller, Get().getOppositePlayer(structure.controller), structure.tile, null, null, null);
            });
        }
    }

    // When the active player is about to perform an attack
    public void setUpCreatureAttack(Creature creature)
    {
        List<Tile> tiles = getAttackableTilesFor(creature);
        bool validAttack = false;
        foreach (Tile tile in tiles)
        {
            tile.setAttackable(true);
            validAttack = true;
        }

        if (validAttack)
        {
            activePlayer.heldCreature = creature;
        }
        else
        {
            showToast("No valid attacks");
            activePlayer.heldCreature = null;
        }
    }

    private List<Tile> getAttackableTilesFor(Creature creature)
    {
        List<Tile> returnList = new List<Tile>();
        Tile creaturesTile = creature.tile;
        foreach (Tile tile in board.allTiles)
        {
            int xDiff = Math.Abs(creaturesTile.x - tile.x); // 0
            int yDiff = Math.Abs(creaturesTile.y - tile.y); // 1
            int distance = xDiff + yDiff; // 1
            if (distance != 0 && distance <= creature.Range) // true
                if (tile.creature != null && tile.creature.controller != creature.controller)
                    returnList.Add(tile);
                else if (tile.structure != null && tile.structure.controller != creature.controller)
                    returnList.Add(tile);
        }
        return returnList;
    }

    public List<Tile> getAllTilesWithCreatures(bool includeUntargetable)
    {
        return board.getAllTilesWithCreatures(includeUntargetable);
    }
    public List<Tile> getAllTilesWithCreatures(Player controller, bool includeUntargetable)
    {
        return board.getAllTilesWithCreatures(controller, includeUntargetable);
    }
    public List<Tile> getAllTilesWithStructures()
    {
        return board.getAllTilesWithStructures();
    }
    public List<Tile> getAllTilesWithStructures(Player controller)
    {
        return board.getAllTilesWithStructures(controller);
    }
    
    public List<Creature> getAllCreaturesControlledBy(Player controller)
    {
        List<Creature> creatureList = new List<Creature>();
        foreach (Creature c in allCreatures)
        {
            if (c.controller == controller)
                creatureList.Add(c);
        }
        return creatureList;
    }

    // called when a player tries to draw a card when there are no cards left
    // haven't decided what to do in this situation yet
    public void playerHasDrawnOutDeck(Player player)
    {
        Debug.Log(player.name + " has drawn out of cards");
        showToast("Someone is out of cards and this hasn't been coded yet :)");
    }

    public void showEndGamePopup(string message)
    {
        glassBackground.SetActive(true);
        EndGamePopUp egp = Instantiate(endGamePopUp, Vector3.zero, Quaternion.identity);
        egp.setup(message);
    }

    public void showToast(string message)
    {
        Toaster.instance.doToast(message);
    }

    public CardViewer getCardViewer()
    {
        return cardPreview;
    }

    public void setPopUpGlassActive(bool value)
    {
        glassBackground.SetActive(value);
    }

    public void getOnAttackParticles(Vector3 position, Vector3 rotation)
    {
        ParticleSystem particleSystem =  Instantiate(onAttackParticles);
        particleSystem.transform.localEulerAngles = rotation;
        particleSystem.transform.position = position;
    }

    public OptionButton getOptionButtonPrefab()
    {
        return optionButton;
    }

    public void showDamagedText(Vector3 position, int damage)
    {
        position.x += .2f; // offset so number is above health value
        damageText.showText(damage, position);
    }
}
