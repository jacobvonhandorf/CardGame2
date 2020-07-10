using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
/*
* Master game object that controls the flow of the game
* and contains utility methods for other classes to call
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
    public List<EffectActuator> beginningOfTurnEffectsList = new List<EffectActuator>();
    public List<EffectActuator> endOfTurnEffectsList = new List<EffectActuator>();

    //public List<EffectActuator> activateASAPEffectsList = new List<EffectActuator>();
    //public List<EffectActuator> onSpellCastEffectsList = new List<EffectActuator>();
    //public List<EffectActuator> afterSpellCastEffectsList = new List<EffectActuator>();
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
    [SerializeField] private AfterMoveBox afterMoveBox;
    [SerializeField] private DamageText damageText;
    [SerializeField] private EndGamePopUp endGamePopUp;

    // prefabs to instantiate later
    [SerializeField] private CardPicker cardPickerPrefab;
    [SerializeField] private OptionButton optionButton;
    [SerializeField] private OptionSelectBox optionSelectBoxPrefab;
    [SerializeField] private XPickerBox xPickerPrefab;
    [SerializeField] private GameObject engineerPrefab;
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
    }

    IEnumerator onlineGameSetupCoroutine()
    {
        Player localPlayer = NetInterface.Get().localPlayerIsP1() ? player1 : player2;
        NetInterface.Get().setLocalPlayer(localPlayer);
        // disable end turn button if going second
        endTurnButton.gameObject.SetActive(NetInterface.Get().localPlayerIsP1());
        // lock local player if they are going second
        localPlayer.locked = !NetInterface.Get().localPlayerIsP1();

        // send opponent starting deck
        NetInterface.Get().syncStartingDeck();

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
        (engineer as CreatureCard).creature.addCounters(Counters.build, 3);

        // place HQ
        int hqCoord = NetInterface.Get().localPlayerIsP1() ? 0 : board.boardWidth - 1;
        Tile hqTile = board.getTileByCoordinate(hqCoord, hqCoord);
        Card hq = createCardById(Headquarters.CARD_ID, localPlayer);
        hq.owner = NetInterface.Get().getLocalPlayer();
        hq.play(hqTile);
        // draw starting hand
        Deck localPlayerDeck = localPlayer.deck;
        List<Card> mullList = localPlayerDeck.getCardList().GetRange(0, STARTING_HAND_SIZE);
        queueCardPickerEffect(localPlayer, mullList, new MulliganReceiver(mullList), 0, STARTING_HAND_SIZE, false, "Select cards to return to deck");

        //localPlayer.drawCards(STARTING_HAND_SIZE);
        NetInterface.Get().signalSetupComplete();

        // network setup is done so wait for opponent to catch up if needed
        while (!NetInterface.Get().finishedWithSetup)
        {
            yield return null;
        }
        // lock local player if they are going second
        //localPlayer.locked = !NetInterface.Get().localPlayerIsP1();
        Debug.Log("Setup complete");
        NetInterface.Get().gameSetupComplete = true;
    }

    private class MulliganReceiver : CanReceivePickedCards
    {
        private List<Card> mullList;

        public MulliganReceiver(List<Card> mullList)
        {
            this.mullList = mullList;
        }

        public void receiveCardList(List<Card> cardList)
        {
            Player localPlayer = NetInterface.Get().getLocalPlayer();
            // remove selected cards from list
            mullList.RemoveAll(c => cardList.Contains(c));
            // add more cards equal to the number mulled away
            List<Card> deckList = localPlayer.deck.getCardList();
            mullList.AddRange(deckList.GetRange(STARTING_HAND_SIZE, cardList.Count));
            foreach (Card c in mullList)
            {
                c.moveToCardPile(localPlayer.hand, false);
            }
        }
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
        Destroy(c.getRootTransform().gameObject);
        // needs to sync card destruction if gameMode is online
    }

    // GAME FLOW METHODS
    private void takeTurn()
    {
        activePlayer.startOfTurn();
        nonActivePlayer.startOfTurn();
        activePlayer.doIncome();
        beginningOfTurnEffects();
        activePlayer.drawCard();
    }

    private void beginningOfTurnEffects()
    {
        GameEvents.TriggerTurnStartEvents(this);
        /*
        EffectActuator[] tempList = new EffectActuator[beginningOfTurnEffectsList.Count];
        beginningOfTurnEffectsList.CopyTo(tempList);
        beginningOfTurnEffectsList.Clear();
        foreach (EffectActuator e in tempList)
        {
            e.activate();
        }
        foreach (Structure s in allStructures)
        {
            s.onTurnStart();
        }*/
    }
    private void endOfTurnEffects()
    {
        EffectActuator[] tempList = new EffectActuator[endOfTurnEffectsList.Count];
        endOfTurnEffectsList.CopyTo(tempList);
        endOfTurnEffectsList.Clear();
        foreach (EffectActuator e in tempList)
        {
            e.activate();
        }
    }
    public void onSpellCastEffects(SpellCard spell)
    {
        GameEvents.TriggerSpellCastEvents(this, new GameEvents.SpellCastEventArgs() { spell = spell });
        /*
        foreach (Structure s in allStructures)
        {
            s.onAnySpellCast(spell);
        }
        foreach (Creature c in allCreatures)
        {
            c.onAnySpellCast(spell);
        }
        foreach(Card c in getAllCardsNotInPlay())
        {
            c.onAnySpellCast(spell);
        }*/
    }
    public void afterSpellCastEffects()
    {
        // TODO
    }

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
        setAllTilesToNotAttackable();
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

    // PUBLIC UTIL METHODS
    public void endTurn()
    {
        if (gameMode == GameMode.online)
        {
            if (activePlayer.locked)
            {
                showToast("Must finish resolving effects before ending turn");
                return;
            }
            // called when button is pressed
            // disable button
            endTurnButton.gameObject.SetActive(false);
            // lock local player
            NetInterface.Get().getLocalPlayer().locked = true;
            // reset player for new turn
            NetInterface.Get().getLocalPlayer().startOfTurn();
            // trigger effects
            endOfTurnEffects();
            switchActivePlayer();
            foreach (Creature c in allCreatures)
                c.resetForNewTurn();
            foreach (Structure s in allStructures)
                s.resetForNewTurn();
            afterMoveBox.hide();
            // send opponent message
            NetInterface.Get().syncEndTurn();
            return;
        }

        Debug.Log("Active player " + activePlayer.locked);
        Debug.Log("Nonactive player " + nonActivePlayer.locked);
        if (activePlayer.locked)
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
        afterMoveBox.hide();
        takeTurn();
    }

    public void startTurnForOnline()
    {
        if (gameMode != GameMode.online)
            throw new Exception("Only call this for online play");

        // called when recieving start turn message
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
        NetInterface.Get().getLocalPlayer().locked = false;
    }

    public List<Tile> getMovableTilesForCreature(Creature creature)
    {
        return board.getAllMovableTiles(creature);
    }

    public void moveCreatureToTile(Creature creature, Tile tile)
    {
        board.moveCreatureToTile(creature, tile);
        setAllTilesToDefault();
        createActionBox(creature);
    }

    public void createActionBox(Creature creature)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z++;
        afterMoveBox.show(creature.currentTile.x-2.4f, creature.currentTile.y-2, activePlayer, creature);
    }

    // Reset all temporary effects on all Tiles
    public void setAllTilesToDefault()
    {
        foreach (Tile t in board.allTiles)
        {
            t.setActive(false);
            t.setAttackable(false);
            t.setEffectableFalse();
        }
    }
    public void setAllTilesToNotActive()
    {
        foreach (Tile t in board.allTiles)
        {
            t.setActive(false);
        }
    }
    public void setAllTilesToNotAttackable()
    {
        foreach (Tile t in board.allTiles)
        {
            t.setAttackable(false);
        }
    }

    /*
     * Returns a list of all tiles that a creature could be deployed to
     */ 
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
    public void createCreatureOnTile(Creature creature, Tile tile, Player owner, CreatureCard sourceCard)
    {
        creature.initialize();
        createCreatureActual(sourceCard, tile, owner, creature);
        // sync creature creation
        NetInterface.Get().syncPermanentPlaced(sourceCard, tile);

        // trigger effects that need to be triggered
        creature.onCreation();
        foreach(Creature c in allCreatures)
        {
            c.onAnyCreaturePlayed(creature);
        }

        List<Card> allCards = new List<Card>(); // all cards not in play
        allCards.AddRange(player1.hand.getCardList());
        allCards.AddRange(player2.hand.getCardList());
        allCards.AddRange(player1.graveyard.getCardList());
        allCards.AddRange(player2.graveyard.getCardList());
        foreach (Card c in allCards)
        {
            c.onAnyCreaturePlayed(creature);
        }
    }

    private void createCreatureActual(CreatureCard sourceCard, Tile tile, Player owner, Creature creature)
    {
        // resize creature, stop treating it as a card and start treating it as a creature
        sourceCard.swapToCreature();

        // place creature in correct location
        Vector3 newPostion = tile.transform.position;
        newPostion.z = 1; // change z so that the card is always above tile and can be clicked
        creature.getRootTransform().position = newPostion;

        // set owner if it hasn't been set already
        // creature.owner = owner; changed so only source card knows the owner
        creature.controller = owner;

        // set creature to has moved and acted unless it is quick
        if (!creature.hasKeyword(Keyword.quick))
        {
            creature.hasMovedThisTurn = true;
            creature.hasDoneActionThisTurn = true;
            creature.updateHasActedIndicators();
        }

        // need to do this for tokens where source card doesn't have a controller (might not be needed anymore)
        if (sourceCard != null && sourceCard.owner == null)
            sourceCard.owner = owner;

        // move card from player's hand and parent it to the board
        if (owner.hand.getCardList().Contains(sourceCard))
            owner.hand.removeCard(sourceCard).getRootTransform().SetParent(board.transform);
        else
            creature.getRootTransform().SetParent(board.transform);

        tile.creature = creature;
        creature.currentTile = tile;
        // add creature to all creatures
        allCreatures.Add(creature);

        creature.updateFriendOrFoeBorder();
    }

    public void syncCreateCreatureOnTile(CreatureCard card, Tile tile, Player owner)
    {
        Debug.Log("Syncing creature placement");
        createCreatureActual(card, tile, owner, card.creature);
    }

    public void createStructureOnTile(Structure structure, Tile tile, Player controller, StructureCard sourceCard)
    {
        // if gamemode is online check to see if 
        createStructureOnTileActual(structure, tile, controller);
        NetInterface.Get().syncPermanentPlaced(sourceCard, tile);
        // trigger ETBS
        structure.onPlaced();
    }

    private void createStructureOnTileActual(Structure structure, Tile tile, Player controller)
    {
        StructureCard sourceCard = structure.sourceCard;

        // resize structure and stop treating it as a card and start treating is as a structure
        sourceCard.swapToStructure();

        // place structure in correct location
        Vector3 newPosition = tile.transform.position;
        newPosition.z = 1;
        structure.getRootTransform().position = newPosition;

        // parent structure to board
        structure.getRootTransform().SetParent(board.transform);

        // move card from player's hand and parent it to the board
        try
        {
            controller.hand.removeCard(sourceCard).getRootTransform().SetParent(board.transform);
        }
        catch (NullReferenceException e)
        {
            Debug.Log("Card not found in hand: " + e.Message);
        }

        tile.structure = structure;
        structure.tile = tile;
        structure.controller = controller;
        structure.sourceCard = sourceCard;
        if (sourceCard.owner != null) // normal cards will already have an owner
            structure.owner = sourceCard.owner;
        else // "token" cards will not have an owner at this point so just use the controller
        {
            structure.owner = controller;
            sourceCard.owner = controller;
        }

        // turn on FoF border
        structure.updateFriendOrFoeBorder();

        // add structure to all structures
        allStructures.Add(structure);
    }

    public void syncStructureOnTile(StructureCard card, Tile tile, Player owner)
    {
        createStructureOnTileActual(card.structure, tile, owner);
    }

    /*
     * When a creatures health drops below 1 this method sends it to grave and is responsible for triggering all effects
     */
    public void destroyCreature(Creature creature)
    {
        Debug.Log("Destroying creature");
        if (creature.currentTile != null) // this check is needed to make some online stuff work
        {
            creature.currentTile.creature = null;
            creature.currentTile = null;
        }
        foreach(Structure structure in board.getAllStructures())
        {
            structure.onAnyCreatureDeath(creature);
        }
        foreach (Creature c in allCreatures)
        {
            c.onAnyCreatureDeath(creature);
        }

        allCreatures.Remove(creature);
        creature.onDeath();
        creature.onLeavesTheField();
        creature.sendToGrave();
    }

    public void destroyStructure(Structure structure)
    {
        Debug.Log("Destroying structure");
        structure.tile.structure = null;
        structure.tile = null;
        foreach (Structure loopStructure in board.getAllStructures())
        {
            loopStructure.onAnyStructureDeath(structure);
        }

        allStructures.Remove(structure);
        structure.sendToGrave();
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
        Effect effect = creature.getEffect();
        if (effect == null)
        {
            showToast("This creature has no effect");
            return;
        }
        if (effect is SingleTileTargetEffect)
        {
            if ((effect as SingleTileTargetEffect).getValidTargetTiles(creature.controller, getOppositePlayer(creature.controller), creature.currentTile).Count == 0)
            {
                showToast("No valid targets for effect");
                return;
            }
            // creature.hasDoneActionThisTurn = true;
            string informationText = creature.sourceCard.getCardName() + "'s Effect";
            setUpSingleTileTargetEffect(effect as SingleTileTargetEffect, creature.controller, creature.currentTile, creature, null, informationText, false);
        }
        else // effect doesn't need a target
        {
            //creature.hasDoneActionThisTurn = true;
            effect.activate(creature.controller, null, creature.currentTile, null, creature, null);
            creature.updateHasActedIndicators();
        }
    }

    // Creature, Structure and textToDisplay can be null
    public void setUpSingleTileTargetEffect(SingleTileTargetEffect effect, Player effectOwner, Tile sourceTile, Creature creature, Structure structure, string textToDisplay, bool isPartOfChain)
    {
        if (!isPartOfChain)
            EffectsManager.Get().addEffect(new WrapperSingleTileTargetEffect(effect, effectOwner, sourceTile, creature, structure), textToDisplay, effectOwner);
        else
            EffectsManager.Get().addEffectToStartOfQueue(new WrapperSingleTileTargetEffect(effect, effectOwner, sourceTile, creature, structure), textToDisplay, effectOwner);
    }

    // an effect actuator whoes effect activates a SingleTileTargetEffect
    private class WrapperSingleTileTargetEffect : EffectActuator
    {
        //private SingleTileTargetEffect wrappedEffect;
        private Structure structure;

        // needs to be passed all the information the effect needs to be activated
        public WrapperSingleTileTargetEffect(SingleTileTargetEffect effect, Player effectOwner, Tile sourceTile, Creature creature, Structure structure)
        {
            //wrappedEffect = effect;
            this.sourceTile = sourceTile;
            sourcePlayer = effectOwner;
            targetPlayer = Get().getOppositePlayer(sourcePlayer);
            this.effect = new WrappedEffectActivator(effect, effectOwner, sourceTile, creature, structure);
        }

        // effect that activates a SingleTileTargetEffect
        private class WrappedEffectActivator : Effect
        {
            private SingleTileTargetEffect effect;
            private Player effectOwner;
            private Tile sourceTile;
            private Creature sourceCreature;
            Structure sourceStructure;

            // needs to be passed all information the effect to be activated needs
            public WrappedEffectActivator(SingleTileTargetEffect effect, Player effectOwner, Tile sourceTile, Creature sourceCreature, Structure sourceStructure)
            {
                this.effect = effect;
                this.effectOwner = effectOwner;
                this.sourceTile = sourceTile;
                this.sourceCreature = sourceCreature;
                this.sourceStructure = sourceStructure;
            }

            public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
            {

                List<Tile> validTargetTiles = effect.getValidTargetTiles(effectOwner, targetPlayer, this.sourceTile);
                bool effectValid = false;
                foreach (Tile tile in validTargetTiles)
                {
                    if (this.sourceCreature != null)
                        tile.setEffectable(true, this.sourceCreature, effect);
                    else if (sourceStructure != null)
                        tile.setEffectable(sourceStructure, effect);
                    else
                        tile.setEffectable(true, effectOwner, effect);
                    effectValid = true;
                }

                if (effectValid)
                {
                    if (effect.canBeCancelled())
                        Get().cancelEffectButton.enable();
                    effectOwner.heldEffect = effect;
                    effectOwner.readyEffect();
                }
                else
                {
                    if (sourceCreature != null)
                        Get().showToast("No valid targets for " + this.sourceCreature.cardName + "'s effect");
                    else if (sourceStructure != null)
                        Get().showToast("No valid targets for " + this.sourceStructure.getCardName() + "'s effect");

                    EffectsManager.Get().signalEffectFinished();
                }
            }
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
        if (effect is SingleTileTargetEffect)
        {
            string informationText = structure.sourceCard.getCardName() + "'s Effect";
            setUpSingleTileTargetEffect(effect as SingleTileTargetEffect, structure.controller, structure.tile, null, structure, informationText, false);
        }
        else
        {
            List<string> options = new List<string>();
            options.Add("Yes");
            options.Add("No");
            queueOptionSelectBoxEffect(options, new StructureEffectOptionHandler(effect, structure), "Are you sure you want to active the effect of " + structure.getCardName(), false, structure.controller);
        }
    }

    private class StructureEffectOptionHandler : OptionBoxHandler
    {
        private Effect effect;
        private Structure structure;

        public StructureEffectOptionHandler(Effect effect, Structure structure)
        {
            this.effect = effect;
            this.structure = structure;
        }

        public void receiveOptionBoxSelection(int selectedOptionIndex, string selectedOption)
        {
            if (selectedOptionIndex == 0)
                effect.activate(structure.controller, Get().getOppositePlayer(structure.controller), structure.tile, null, null, null);
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
            activePlayer.readyAttack();
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
        Tile creaturesTile = creature.currentTile;
        foreach (Tile tile in board.allTiles)
        {
            int xDiff = Math.Abs(creaturesTile.x - tile.x); // 0
            int yDiff = Math.Abs(creaturesTile.y - tile.y); // 1
            int distance = xDiff + yDiff; // 1
            if (distance != 0 && distance <= creature.range) // true
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

    // headerText can be null
    public void queueCardPickerEffect(Player effectOwner, List<Card> pickableCards, CanReceivePickedCards receiver, int minCards, int maxCards, bool isPartOfChain, string headerText)
    {
        if (pickableCards.Count >= minCards)
        {
            if (!isPartOfChain)
                EffectsManager.Get().addEffect(new WrappedCardPickerEffect(pickableCards, receiver, minCards, maxCards, headerText), effectOwner);
            else
                EffectsManager.Get().addEffectToStartOfQueue(new WrappedCardPickerEffect(pickableCards, receiver, minCards, maxCards, headerText), null, effectOwner);
        }
        else
            Debug.LogError("Not enough cards for card picker effect");
    }

    private class WrappedCardPickerEffect : EffectActuator
    {
        public WrappedCardPickerEffect(List<Card> pickableCards, CanReceivePickedCards receiver, int minCards, int maxCards, string headerText)
        {
            effect = new CardPickerEffect(pickableCards, receiver, minCards, maxCards, headerText);
        }

        private class CardPickerEffect : Effect
        {
            List<Card> pickableCards;
            int minCards;
            int maxCards;
            string headerText;
            CanReceivePickedCards receiver;

            public CardPickerEffect(List<Card> pickableCards, CanReceivePickedCards receiver, int minCards, int maxCards, string headerText)
            {
                this.pickableCards = pickableCards;
                this.minCards = minCards;
                this.maxCards = maxCards;
                this.headerText = headerText;
                this.receiver = receiver;
            }

            public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
            {
                CardPicker cardPicker = Instantiate(Get().cardPickerPrefab, new Vector3(0, 0, -1), Quaternion.identity);
                cardPicker.setUp(pickableCards, receiver, minCards, maxCards, headerText);
                Get().setPopUpGlassActive(true);
            }
        }
    }

    // headerText can be null
    public void queueOptionSelectBoxEffect(List<string> options, OptionBoxHandler handler, string headerText, bool isPartOfChain, Player effectOwner)
    {
        if (!isPartOfChain)
            EffectsManager.Get().addEffect(new WrappedOptionBoxEffect(options, handler, headerText), effectOwner);
        else
            EffectsManager.Get().addEffectToStartOfQueue(new WrappedOptionBoxEffect(options, handler, headerText), null, effectOwner);
    }

    private class WrappedOptionBoxEffect : EffectActuator
    {
        public WrappedOptionBoxEffect(List<string> options, OptionBoxHandler handler, string headerText)
        {
            effect = new OptionSelectBoxEffect(options, handler, headerText);
        }

        private class OptionSelectBoxEffect : Effect
        {
            List<string> options;
            OptionBoxHandler handler;
            string headerText;

            public OptionSelectBoxEffect(List<string> options, OptionBoxHandler handler, string headerText)
            {
                this.options = options;
                this.handler = handler;
                this.headerText = headerText;
            }

            public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
            {
                OptionSelectBox optionBox = Instantiate(Get().optionSelectBoxPrefab, Vector3.zero, Quaternion.identity);
                optionBox.setUp(options, handler, headerText);
            }
        }
    }

    public void queueXPickerEffect(CanRecieveXPick receiver, string headerText, int minValue, int maxValue, bool isPartOfChain, Player effectOwner)
    {
        if (!isPartOfChain)
            EffectsManager.Get().addEffect(new WrappedXPickerEffect(receiver, minValue, maxValue, headerText), "", effectOwner);
        else
            EffectsManager.Get().addEffectToStartOfQueue(new WrappedXPickerEffect(receiver, minValue, maxValue, headerText), "", effectOwner);
    }

    private class WrappedXPickerEffect : EffectActuator
    {
        public WrappedXPickerEffect(CanRecieveXPick receiver, int minValue, int maxValue, string headerText)
        {
            effect = new XPickerEffect(receiver, minValue, maxValue, headerText);
        }

        private class XPickerEffect : Effect
        {
            public CanRecieveXPick receiver;
            public int minValue;
            public int maxValue;
            public string headerText;

            public XPickerEffect(CanRecieveXPick receiver, int minValue, int maxValue, string headerText)
            {
                this.receiver = receiver;
                this.minValue = minValue;
                this.maxValue = maxValue;
                this.headerText = headerText;
            }

            public void activate(Player sourcePlayer, Player targetPlayer, Tile sourceTile, Tile targetTile, Creature sourceCreature, Creature targetCreature)
            {
                XPickerBox xPicker = Instantiate(Get().xPickerPrefab);
                xPicker.setUp(receiver, minValue, maxValue, headerText);
            }
        }
    }

    public void getOnAttackParticles(Vector3 position, Vector3 rotation)
    {
        ParticleSystem particleSystem =  Instantiate(onAttackParticles);
        //particleSystem.transform.position = position;
        particleSystem.transform.localEulerAngles = rotation;
        particleSystem.transform.position = position;
    }

    public OptionButton getOptionButtonPrefab()
    {
        return optionButton;
    }

    internal List<Tile> allTiles()
    {
        return board.allTiles;
    }

    public void showDamagedText(Vector3 position, int damage)
    {
        position.x += .2f; // offset so number is above health value
        damageText.showText(damage, position);
    }

}
