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

    public static GameManager Instance { get; private set; }

    public static string p1DeckName;
    public static string p2DeckName;

    [SerializeField] private Player player1; // for online p1 is the local player and p2 is opponent
    [SerializeField] private Player player2;
    public Player ActivePlayer { get; private set; }
    public Player NonActivePlayer { get; private set; }

    public Board board;

    // singletone objects that GameManager is responsible for handling
    [SerializeField] private CardViewer cardPreview;
    [SerializeField] private TextMeshPro activePlayerText;
    [SerializeField] private MyButton cancelEffectButton;
    [SerializeField] private MyButton endTurnButton;
    [SerializeField] private DamageText damageText;
    [SerializeField] private EndGamePopUp endGamePopUp;

    // prefabs to instantiate later
    [SerializeField] public CardPicker cardPickerPrefab;
    [SerializeField] private OptionButton optionButton;
    [SerializeField] public OptionSelectBox optionSelectBoxPrefab;
    [SerializeField] private ParticleSystem onAttackParticles;

    // game mode for hotseat vs online. Testing allows cards to be cast without their costs
    public enum GameMode { hotseat, online }
    public static GameMode gameMode = GameMode.hotseat;
    
    void Awake()
    {
        // set gameManager to singleton instance
        if (Instance != null)
            throw new Exception("More than one game manager detected. Game manager should be singleton");
        Instance = this;
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
                StartCoroutine(OnlineGameSetupCoroutine());
                break;
        }
    }

    private void hotSeatSetup()
    {
        throw new NotImplementedException();
    }

    private readonly object activePlayerLock = new object();
    IEnumerator OnlineGameSetupCoroutine()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        Player localPlayer = NetInterface.Get().LocalPlayerIsP1 ? player1 : player2;
        NetInterface.Get().localPlayer = localPlayer;
        // disable end turn button if going second
        endTurnButton.gameObject.SetActive(NetInterface.Get().LocalPlayerIsP1);
        // lock local player if they are going second
        if (!NetInterface.Get().LocalPlayerIsP1)
            localPlayer.AddLock(activePlayerLock);

        // send opponent starting deck
        NetInterface.Get().SyncStartingDeck();
        stopwatch.Stop();
        //Debug.Log("Finished instantiating cards " + stopwatch.ElapsedMilliseconds + "ms");
        // wait for opponent to finish sending their deck
        while (!NetInterface.Get().opponentFinishedSendingCards) { yield return null; }

        // place starting engi
        int engiCoord = NetInterface.Get().LocalPlayerIsP1 ? 1 : 6;
        Tile engiTile = board.GetTileByCoordinate(engiCoord, engiCoord);
        Card engineer = CreateCardById((int) CardIds.Engineer, localPlayer);
        engineer.Play(engiTile);
        (engineer as CreatureCard).Creature.Counters.Add(CounterType.Build, 3);

        // place HQ
        int hqCoord = NetInterface.Get().LocalPlayerIsP1 ? 0 : board.boardWidth - 1;
        Tile hqTile = board.GetTileByCoordinate(hqCoord, hqCoord);
        Card hq = CreateCardById((int)CardIds.Headquarters, localPlayer);
        hq.Owner = NetInterface.Get().localPlayer;
        hq.Play(hqTile);
        // draw starting hand
        Deck localPlayerDeck = localPlayer.Deck;

        NetInterface.Get().SignalSetupComplete();

        // network setup is done so wait for opponent to catch up if needed
        while (!NetInterface.Get().finishedWithSetup) { yield return null; }
        NetInterface.Get().gameSetupComplete = true;

        // starting mulligan
        List<Card> mullList = localPlayerDeck.CardList.Take(STARTING_HAND_SIZE).ToList();
        CardPicker.CreateAndQueue(mullList, 0, STARTING_HAND_SIZE, "Select cards to return to deck", localPlayer, delegate (List<Card> cardList) 
        {
            mullList.RemoveAll(c => cardList.Contains(c));
            foreach (Card c in mullList)
                c.MoveToCardPile(localPlayer.Hand, null);
            while (localPlayer.Hand.CardList.Count < STARTING_HAND_SIZE)
                localPlayer.Deck.CardList[0].MoveToCardPile(localPlayer.Hand, null);

            localPlayer.Deck.Shuffle();

            ActivePlayer = localPlayer;
            NonActivePlayer = GetOppositePlayer(localPlayer);
        });
    }

    private void LoadDecks()
    {
        Destroy(player1.Deck.gameObject);
        Destroy(player2.Deck.gameObject);

        Deck newP1Deck = DeckUtilities.getDeckFromFileName(p1DeckName + ".dek");
        Deck newP2Deck = DeckUtilities.getDeckFromFileName(p2DeckName + ".dek");

        newP1Deck.transform.parent = player1.transform;
        newP2Deck.transform.parent = player2.transform;
        //player1.Deck = newP1Deck;
        //player2.Deck = newP2Deck;
        foreach (Card c in newP1Deck.CardList)
            c.Owner = player1;
        foreach (Card c in newP2Deck.CardList)
            c.Owner = player2;
        newP1Deck.Shuffle();
        newP2Deck.Shuffle();
    }

    private void OnDestroy()
    {
        clearEvents();
    }

    public Card CreateCardById(int id, Player owner)
    {
        Card newCard = ResourceManager.Get().InstantiateCardById(id);
        newCard.Owner = owner;
        newCard.Initialize();
        if (gameMode == GameMode.online)
            NetInterface.Get().SyncNewCardToOpponent(newCard);
        return newCard;
    }
    public Card CreateCardById(CardIds id, Player owner) => CreateCardById((int)id, owner);

    // GAME FLOW METHODS
    private void StartNewturn()
    {
        ActivePlayer.StartOfTurn();
        NonActivePlayer.StartOfTurn();
        TriggerTurnStartEvents(this);
        ActivePlayer.DoIncome();
        ActivePlayer.DrawCard();
    }

    #region TriggeredEffects
    public void onSpellCastEffects(SpellCard spell)
    {
        TriggerSpellCastEvents(this, new SpellCastArgs() { spell = spell });
    }
    #endregion

    public void doAttackOn(Damageable defender)
    {
        Creature attacker = ActivePlayer.heldCreature;
        int damageRoll = attacker.Attack(defender);
        NetInterface.Get().SyncAttack(attacker, defender, damageRoll);
        Board.Instance.SetAllTilesToDefault();
    }

    private void SwitchActivePlayer()
    {
        Player tempPlayer = ActivePlayer;
        ActivePlayer = NonActivePlayer;
        NonActivePlayer = tempPlayer;
        ActivePlayer.isActivePlayer = true;
        NonActivePlayer.isActivePlayer = false;

        activePlayerText.text = ActivePlayer.name + "'s turn";
    }

    public void EndTurn()
    {
        if (ActivePlayer.IsLocked())
        {
            Toaster.Instance.DoToast("Must finish resolving effects before ending turn");
            return;
        }
        if (gameMode == GameMode.online)
            OnlineTurnEnd();
        else
            LocalTurnEnd();

    }

    private void LocalTurnEnd()
    {
        TriggerTurnEndEvents(this);
        SwitchActivePlayer();
        foreach (Creature c in board.AllCreatures)
            c.ResetForNewTurn();
        foreach (Structure s in board.AllStructures)
            s.ResetForNewTurn();
        ActionBox.instance.gameObject.SetActive(false);
        StartNewturn();
    }

    private void OnlineTurnEnd()
    {
        // disable button
        endTurnButton.gameObject.SetActive(false);
        // lock local player
        NetInterface.Get().localPlayer.AddLock(activePlayerLock);
        // reset player for new turn
        NetInterface.Get().localPlayer.StartOfTurn();
        // trigger effects
        TriggerTurnEndEvents(this);
        SwitchActivePlayer();

        foreach (Creature c in board.AllCreatures)
            c.ResetForNewTurn();
        foreach (Structure s in board.AllStructures)
            s.ResetForNewTurn();
        ActionBox.instance.gameObject.SetActive(false);
        // send opponent message
        NetInterface.Get().SyncEndTurn();
        return;
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
        SwitchActivePlayer();
        Player localPlayer = NetInterface.Get().localPlayer;
        localPlayer.StartOfTurn();
        localPlayer.DoIncome();
        localPlayer.DrawCard();
        TriggerTurnStartEvents(this);
        // unlock local player
        NetInterface.Get().localPlayer.RemoveLock(activePlayerLock);
    }

    // Returns a list of all tiles that a creature could be deployed to
    public List<Tile> getAllDeployableTiles(Player player)
    {
        List<Tile> returnList = new List<Tile>();
        foreach(Tile tile in board.AllTiles)
        {
            if (tile.creature != null)
                continue;

            int x1 = tile.x + 1;
            int y1 = tile.y;
            if (canDeployFrom(board.GetTileByCoordinate(x1, y1), player))
                if (!returnList.Contains(tile))
                    returnList.Add(tile);

            int x2 = tile.x - 1;
            int y2 = tile.y;
            if (canDeployFrom(board.GetTileByCoordinate(x2, y2), player))
                if (!returnList.Contains(tile))
                    returnList.Add(tile);

            int x3 = tile.x;
            int y3 = tile.y + 1;
            if (canDeployFrom(board.GetTileByCoordinate(x3, y3), player))
                if (!returnList.Contains(tile))
                    returnList.Add(tile);

            int x4 = tile.x;
            int y4 = tile.y - 1;
            if (canDeployFrom(board.GetTileByCoordinate(x4, y4), player))
                if (!returnList.Contains(tile))
                    returnList.Add(tile);
        }

        // remove tiles that are already occupied
        returnList = returnList.Except(Board.Instance.GetAllTilesWithCreatures(true)).ToList();
        returnList = returnList.Except(Board.Instance.GetAllTilesWithStructures()).ToList();

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
            returnList.AddRange(board.GetAllTilesWithinExactRangeOfTile(structure.Tile, 2));
            // also get a list of already used tiles
            invalidTiles.Add(structure.Tile);
            // also get a list of tiles adjacent to structures
            invalidTiles.AddRange(board.GetAllTilesWithinExactRangeOfTile(structure.Tile, 1));
        }

        // remove all invalid tiles
        returnList = returnList.Except(invalidTiles).ToList();
        returnList = returnList.Except(Board.Instance.GetAllTilesWithCreatures(true)).ToList();
        returnList = returnList.Except(board.GetAllTilesOnRow(3)).ToList();
        returnList = returnList.Except(board.GetAllTilesOnRow(4)).ToList();

        return returnList;
    }
    // returns true if the tile is something that can be returned from
    private bool canDeployFrom(Tile tile, Player player)
    {
        if (tile == null)
            return false;
        if (tile.structure != null && tile.structure.Controller == player && tile.structure.canDeployFrom())
            return true;
        if (tile.creature != null && tile.creature.Controller == player && tile.creature.CanDeployFrom)
            return true;
        return false;
    }

    // places teh creature passed to it on the tile passed
    public void createCreatureOnTile(Creature creature, Tile tile, Player owner)
    {
        //creature.initialize();
        createCreatureActual(tile, owner, creature);
        // sync creature creation
        NetInterface.Get().SyncPermanentPlaced(creature.SourceCard, tile);

        // trigger effects that need to be triggered
        creature.TriggerOnDeployed(this);
        TriggerCreaturePlayedEvents(this, new CreaturePlayedArgs() { creature = creature });
    }

    private void createCreatureActual(Tile tile, Player owner, Creature creature)
    {
        // resize creature, stop treating it as a card and start treating it as a creature
        (creature.SourceCard as CreatureCard).SwapToCreature(tile);

        // set owner if it hasn't been set already
        creature.Controller = owner;

        // set creature to has moved and acted unless it is quick
        if (!creature.HasKeyword(Keyword.Quick))
        {
            creature.HasMovedThisTurn = true;
            creature.HasDoneActionThisTurn = true;
            creature.UpdateHasActedIndicators();
        }

        tile.creature = creature;
        creature.Tile = tile;

        creature.UpdateFriendOrFoeBorder();
    }

    public void syncCreateCreatureOnTile(CreatureCard card, Tile tile, Player owner)
    {
        Debug.Log("Syncing creature placement");
        // animate showing card
        //InformativeAnimationsQueue.Instance.AddAnimation(new ShowCardCmd(card, true, this));
        createCreatureActual(tile, owner, card.Creature);
    }
    private bool showCardFinished = false;
    private class ShowCardCmd : IQueueableCommand
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

        public bool IsFinished => Instance.showCardFinished;

        public void Execute()
        {
            gameManager.StartCoroutine(gameManager.showCardCrt(card, fromTop));
        }
    }
    IEnumerator showCardCrt(Card card, bool fromTop)
    {
        showCardFinished = false;
        TransformManager tm = card.TransformManager;
        tm.ClearQueue();
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
        NetInterface.Get().SyncPermanentPlaced(sourceCard, tile);
        // trigger ETBS
        structure.TriggerOnDeployed(this);
    }

    private void createStructureOnTileActual(Structure structure, Tile tile, Player controller)
    {
        Card sourceCard = structure.SourceCard;

        // parent structure to board
        structure.transform.SetParent(board.transform);

        // move card from player's hand and parent it to the board
        sourceCard.MoveToCardPile(board, null);

        // resize structure and stop treating it as a card and start treating is as a structure
        (sourceCard as StructureCard).swapToStructure(tile);

        tile.structure = structure;
        structure.Tile = tile;
        structure.Controller = controller;

        // turn on FoF border
        structure.UpdateFriendOrFoeBorder();
    }

    public void syncStructureOnTile(StructureCard card, Tile tile, Player owner)
    {
        //Debug.Log("Card is " + card);
        //InformativeAnimationsQueue.Instance.AddAnimation(new ShowCardCmd(card, true, this));
        createStructureOnTileActual(card.structure, tile, owner);
    }

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
        if (creature.Tile != null) // this check is needed to make some online stuff work
        {
            creature.Tile.creature = null;
            creature.Tile = null;
        }
        TriggerCreatureDeathEvents(this, new CreatureDeathArgs() { creature = creature });
        creature.SourceCard.MoveToCardPile(creature.SourceCard.Owner.Graveyard, null);
        creature.TriggerOnDeathEvents(this);
    }

    public void kill(Structure structure)
    {
        structure.Tile.structure = null;
        structure.Tile = null;
        structure.onRemoved();
    }

    public Player GetOppositePlayer(Player player) => player == player1 ? player2 : player1;

    public void makePlayerLose(Player player)
    {
        Toaster.Instance.DoToast("Player " + GetOppositePlayer(player).name + " wins!");
    }

    public void makePlayerWin(Player player)
    {
        Toaster.Instance.DoToast("Player " + player.name + " wins!");
    }

    public void Surrender()
    {
        NetInterface.Get().SendSurrenderMessage();
        // probably want to start a coroutine here that handles end of match
        NetInterface.Reset();
        SceneManager.LoadScene(ScenesEnum.MMDeckSelect);
    }

    public void setUpCreatureEffect(Creature creature)
    {
        // check for creature not having an effect
        if (creature.ActivatedEffects.Count == 0)
        {
            Toaster.Instance.DoToast("This creature has no effect");
            return;
        }
        else if (creature.ActivatedEffects.Count > 1)
        {
            throw new Exception("Not implemented");
        }
        else
        {
            // there is only 1 effect so just activate it
            // creature.activatedEffects[0].doEffect(this);
            creature.ActivatedEffects[0].Invoke();
        }
    }

    public void setUpStructureEffect(Structure structure)
    {
        Debug.Log("Setting up structure effect");
        Effect effect = structure.getEffect();
        if (effect == null)
        {
            Toaster.Instance.DoToast("This structure has no effect");
            return;
        }
        else
        {
            List<string> options = new List<string>();
            options.Add("Yes");
            options.Add("No");
            OptionSelectBox.CreateAndQueue(options, "Are you sure you want to activate the effect of " + structure.SourceCard.CardId, structure.Controller, delegate (int selectedIndex, string selectedOption)
            {
                if (selectedIndex == 0)
                    effect.activate(structure.Controller, Instance.GetOppositePlayer(structure.Controller), structure.Tile, null, null, null);
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
            tile.SetAttackable(true);
            validAttack = true;
        }

        if (validAttack)
        {
            ActivePlayer.heldCreature = creature;
        }
        else
        {
            Toaster.Instance.DoToast("No valid attacks");
            ActivePlayer.heldCreature = null;
        }
    }

    private List<Tile> getAttackableTilesFor(Creature creature)
    {
        List<Tile> returnList = new List<Tile>();
        Tile creaturesTile = creature.Tile;
        foreach (Tile tile in board.AllTiles)
        {
            int xDiff = Math.Abs(creaturesTile.x - tile.x); // 0
            int yDiff = Math.Abs(creaturesTile.y - tile.y); // 1
            int distance = xDiff + yDiff; // 1
            if (distance != 0 && distance <= creature.Range) // true
                if (tile.creature != null && tile.creature.Controller != creature.Controller)
                    returnList.Add(tile);
                else if (tile.structure != null && tile.structure.Controller != creature.Controller)
                    returnList.Add(tile);
        }
        return returnList;
    }

    public List<Creature> getAllCreaturesControlledBy(Player controller) => board.AllCreatures.FindAll(c => c.Controller == controller);

    // called when a player tries to draw a card when there are no cards left
    // haven't decided what to do in this situation yet
    public void playerHasDrawnOutDeck(Player player)
    {
        Toaster.Instance.DoToast("Someone is out of cards and this hasn't been coded yet :)");
    }

    public void showEndGamePopup(string message)
    {
        EndGamePopUp egp = Instantiate(endGamePopUp, Vector3.zero, Quaternion.identity);
        egp.setup(message);
    }

    public CardViewer getCardViewer() => cardPreview;

    public void PlayOnAttackParticles(Vector3 position, Vector3 rotation)
    {
        ParticleSystem particleSystem = Instantiate(onAttackParticles);
        particleSystem.transform.localEulerAngles = rotation;
        particleSystem.transform.position = position;
    }

    public OptionButton getOptionButtonPrefab() => optionButton;

    public void showDamagedText(Vector3 position, int damage)
    {
        position.x += .2f; // offset so number is above health value
        damageText.showText(damage, position);
    }
}
