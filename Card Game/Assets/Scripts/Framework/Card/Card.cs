using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Card : MonoBehaviour, IHasCardTags, ICanBeCardViewed
{
    #region Enums
    public enum Tag
    {
        Human, Fairy, Arcane,
        Income,
        Gem
    }

    public enum CardType
    {
        Spell,
        Creature,
        Structure,
        Other
    }

    public enum ElementIdentity
    {
        Fire = 0, Water = 1, Wind = 2, Earth = 3, Nuetral = 4 // numbered so cards can be sorted by element
    }
    #endregion

    // basic fields
    public List<Tag> Tags { get; } = new List<Tag>();
    private bool hidden = true; // if true then all players can see the card
    public CardPile CardPile { get; private set; }
    [HideInInspector] public Player owner; // set this to readonly after done using TestCard
    public EmptyHandler onInitilization;
    [HideInInspector] public int cardId;

    // stats
    public StatsContainer Stats { get; } = new StatsContainer();
    public int GoldCost { get { return (int)Stats.StatList[StatType.GoldCost]; } set { Stats.SetValue(StatType.GoldCost, value); } }
    public int BaseGoldCost { get { return (int)Stats.StatList[StatType.BaseGoldCost]; } set { Stats.SetValue(StatType.BaseGoldCost, value); } }
    public int ManaCost { get { return (int)Stats.StatList[StatType.ManaCost]; } set { Stats.SetValue(StatType.ManaCost, value); } }
    public int BaseManaCost { get { return (int)Stats.StatList[StatType.BaseManaCost]; } set { Stats.SetValue(StatType.BaseManaCost, value); } }
    public string CardName { get { return (string)Stats.StatList[StatType.Name]; } set { Stats.SetValue(StatType.Name, value); } }
    public string TypeText { set { Stats.SetValue(StatType.TypeText, value); } }
    public string EffectText { get { return (string)Stats.StatList[StatType.EffectText]; } set { Stats.SetValue(StatType.EffectText, value); } }
    public Sprite Art { get { return (Sprite)Stats.StatList[StatType.Art]; } set { Stats.SetValue(StatType.Art, value); } }
    public ElementIdentity ElementalId { get { return Stats.GetValue<ElementIdentity>(StatType.ElementalId); } set { Stats.SetValue(StatType.ElementalId, value); } }
    public int TotalCost { get { return GoldCost + ManaCost; } }

    // graphics
    public CardVisual CardVisuals { get { return cardVisuals; } }
    [SerializeField] protected CardVisual cardVisuals;
    protected TextMeshPro[] tmps; // all text objects for this card
    [HideInInspector] public bool isBeingDragged = false;
    [HideInInspector] public bool positionLocked = false;
    [HideInInspector] public Vector3 positionInHand;
    public HashSet<CardViewer> viewersDisplayingThisCard;

    public List<ToolTipInfo> toolTipInfos = new List<ToolTipInfo>();
    public TransformManager TransformManager { get; private set; }
    [SerializeField] private StatChangePropogator statChangePropogator;

    #region Events
    public event EventHandler<AddedToCardPileArgs> E_AddedToCardPile;
    public class AddedToCardPileArgs : EventArgs
    {
        public AddedToCardPileArgs(CardPile previousCardPile, CardPile newCardPile, Card source)
        {
            this.previousCardPile = previousCardPile;
            this.newCardPile = newCardPile;
            this.source = source;
        }
        public CardPile previousCardPile { get; set; }
        public CardPile newCardPile { get; set; }
        public Card source { get; set; }
    }
    public delegate void AddedToCardPileHandler(object sender, AddedToCardPileArgs e);
    public void TriggerAddedToCardPileEffects(object sender, AddedToCardPileArgs args)
    {
        if (E_AddedToCardPile != null)
            E_AddedToCardPile.Invoke(sender, args);
    }
    #endregion

    protected virtual void Start()
    {
        positionInHand = transform.position;
    }
    protected virtual void Awake()
    {
        viewersDisplayingThisCard = new HashSet<CardViewer>();

        tmps = GetComponentsInChildren<TextMeshPro>();
        //setSpritesToSortingLayer(SpriteLayers.CardInHandMiddle);
        effectGraphic = EffectGraphic.NewEffectGraphic(this);
        TransformManager = GetComponentInChildren<TransformManager>();
        statChangePropogator.Source = Stats;
        Stats.AddType(StatType.BaseGoldCost);
        Stats.AddType(StatType.GoldCost);
        Stats.AddType(StatType.BaseManaCost);
        Stats.AddType(StatType.ManaCost);
    }

    // returns true if this card is the type passed to it
    public bool IsType(CardType type) => getCardType().Equals(type);

    /*
     * makes it so this card is only visible if it normally is
     * ex: cards in deck will not be visible but cards in hand will be
     */
    public void hide()
    {
        hidden = true;
    }

    // make visible to the controller of the card
    public void show()
    {
        hidden = false;
    }

    public abstract void Initialize();

    // move card to a pile and remove it from the old one
    public void MoveToCardPile(CardPile newPile, Card source)
    {
        ActualMove(newPile, source);
        NetInterface.Get().SyncMoveCardToPile(this, newPile, source);
    }

    public void SyncCardMovement(CardPile newPile, Card source)
    {
        ActualMove(newPile, source);
    }

    private void ActualMove(CardPile newPile, Card source)
    {
        if (CardPile != null)
        {
            if (newPile == CardPile) // if we're already in the new pile then do nothing
                return;
            CardPile.removeCard(this);
        }
        CardPile previousPile = CardPile;
        CardPile = newPile;

        newPile.addCard(this);
        TriggerAddedToCardPileEffects(this, new AddedToCardPileArgs(previousPile, newPile, source));
    }


    #region HoveringEffects
    private bool hovered = false;
    // when card becomse hovered show it in the main card preview
    private void OnMouseEnter()
    {
        if (!enabled)
            return;
        GameManager.Get().getCardViewer().SetCard(this);

        // also move card up a bit
        Vector3 oldPosition = positionInHand;
        Vector3 newPosition = new Vector3(oldPosition.x, oldPosition.y + hoverOffset, oldPosition.z);
        moveTo(newPosition);

        hovered = true;
        StartCoroutine(CheckHoverForTooltips());
    }
    private const float hoverOffset = .7f;
    // move card back down when it is no longer being hovered
    private void OnMouseExit()
    {
        if (!enabled)
            return;
        moveTo(positionInHand);
        hovered = false;
        foreach (CardViewer cv in viewersDisplayingThisCard)
            cv.ClearToolTips();
    }
    private void OnDisable()
    {
        foreach (CardViewer cv in viewersDisplayingThisCard)
            cv.ClearToolTips();
    }
    #endregion
    #region ClickAndDrag
    private Vector3 offset;
    void OnMouseDown()
    {
        if (!enabled)
            return;
        if (owner.isLocked())
            return;
        if (positionLocked)
        {
            return;
        }

        isBeingDragged = true;
        offset = transform.position -
            Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
        Board.instance.setAllTilesToDefault();
        NetInterface.Get().localPlayer.heldCreature = null;

        foreach (Tile tile in LegalTargetTiles)
        {
            tile.setActive(true);
        }

        //setSpritesToSortingLayer(SpriteLayers.CardBeingDragged);
        // clear tooltips
        foreach (CardViewer cv in viewersDisplayingThisCard)
        {
            cv.ClearToolTips();
        }
        TransformManager.clearQueue();
        TransformManager.Lock();
    }
    void OnMouseDrag()
    {
        if (!enabled)
            return;
        if (positionLocked || owner.isLocked())
        {
            Debug.Log("Card Position locked");
            return;
        }
        if (!isBeingDragged) // if being dragged has been cancelled
            return;

        Vector3 newPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1.0f);
        transform.position = Camera.main.ScreenToWorldPoint(newPosition) + offset;
        //isBeingDragged = true;
        Tile aboveTile = getTileMouseIsOver();
        if (aboveTile != null)
        {
            //setSpriteAlpha(.5f);
        }
        else
        {
            //setSpriteAlpha(1f);
        }
        TransformManager.Lock();
    }
    private void OnMouseUp()
    {
        if (!enabled)
            return;
        if (owner.isLocked())
        {
            Debug.Log("Owner is locked");
            return;
        }
        if (!isBeingDragged) // if drag was cancelled
            return;
        isBeingDragged = false;
        //setSpriteAlpha(1f);
        //setSpritesToSortingLayer(SpriteLayers.CardInHandMiddle);

        TransformManager.UnLock();
        // if card is above a Tile then play it
        Tile tile = getTileMouseIsOver();
        if (tile != null)
        {
            if (LegalTargetTiles.Contains(tile))
            {
                //Debug.Log("Attempting to play card");
                if (CanBePlayed())
                {
                    PayCosts(owner);
                    switch (getCardType())
                    {
                        case CardType.Spell:
                            owner.numSpellsCastThisTurn++;
                            break;
                        case CardType.Creature:
                            owner.numCreaturesThisTurn++;
                            break;
                        case CardType.Structure:
                            owner.numStructuresThisTurn++;
                            break;
                    }
                    Play(tile);
                }
                else
                {
                    GameManager.Get().ShowToast("You can't play this card right now");
                    moveTo(positionInHand);
                }
            }
            else
                moveTo(positionInHand);
        }
        else
            moveTo(positionInHand);
        Board.instance.setAllTilesToDefault();
    }
    private void cancelDrag()
    {
        Debug.Log("Cancel drag called");
        if (owner.isLocked() || !isBeingDragged)
            return;
        isBeingDragged = false;
        //setSpriteAlpha(1f);
        //setSpritesToSortingLayer(SpriteLayers.CardInHandMiddle);
        moveTo(positionInHand);
        
        Board.instance.setAllTilesToDefault();
        TransformManager.UnLock();
    }
    // change the alpha for all sprites related to this card
    private Tile getTileMouseIsOver()
    {
        Vector2 origin = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                                          Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.zero, 0.1f, LayerMask.GetMask("Tile"));
        if (hit.collider != null)
        {
            Tile objectHit = hit.transform.gameObject.GetComponent<Tile>();
            Debug.DrawLine(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Color.red);
            return objectHit;
        }
        return null;
    }
    #endregion

    private EffectGraphic effectGraphic; // initialized in awake
    public void showInEffectsView()
    {
        EffectGraphicsView.Get().addToQueue(effectGraphic);
    }

    #region MovingGraphicsMethods
    private const float smoothing = 9f; // speed at which cards snap into their place
    // methods for removing Card from scene by moving them way off screen
    // and for returning them to the scene afterwards
    public Vector3 positionOnScene;
    private bool onScene = true;
    public void removeGraphicsAndCollidersFromScene()
    {
        //if (!onScene) not sure if this is needed. Commenting it out fixed card not going to grave when killed over network
        //    return;

        positionOnScene = transform.position;
        transform.position = new Vector3(99999f, 99999f, 99999f);
        TransformManager.Lock();
        TransformManager.enabled = false;
        //interuptMove = true;
        onScene = false;
    }
    public void returnGraphicsAndCollidersToScene()
    {
        if (GameManager.gameMode == GameManager.GameMode.online)
        {
            // if card is in hand and the owner is not the local player then don't do anything
            if (CardPile is Hand && owner != NetInterface.Get().localPlayer)
            {
                return;
            }
        }

        TransformManager.enabled = true;
        TransformManager.UnLock();
        transform.position = positionOnScene;
        onScene = true;
    }
    public bool isOnScene()
    {
        return onScene;
    }

    // these methods and coroutines change the cards position smoothly
    public void moveTo(Vector3 position)
    {
        TransformStruct ts = new TransformStruct(TransformManager.transform);
        ts.position = position;
        TransformManager.moveToImmediate(ts);
    }
    #endregion
    #region CardViewer
    public void OnAddedToViewer(CardViewer cardViewer)
    {
    }

    public void OnRemovedFromViewer(CardViewer cardViewer)
    {
    }

    public abstract List<Tile> LegalTargetTiles { get; }

    public IHaveReadableStats ReadableStats => Stats;
    #endregion

    public bool OwnerCanPayCosts()
    {
        if (GoldCost > owner.getGold())
            return false;
        if (ManaCost > owner.getMana())
            return false;
        return true;
    }
    private void PayCosts(Player player)
    {
        player.addGold(-Mathf.Clamp(GoldCost, 0, 999999));
        player.addMana(-Mathf.Clamp(ManaCost, 0, 999999));
    }

    #region GettersAndSetters
    public virtual void resetToBaseStats()
    {
        GoldCost = BaseGoldCost;
        ManaCost = BaseManaCost;
    }
    public virtual void resetToBaseStatsWithoutSyncing()
    {
        GoldCost = BaseGoldCost;
        ManaCost = BaseManaCost;
    }
    #endregion
    #region Keyword
    private List<Keyword> keywordList = new List<Keyword>();
    public void AddKeyword(Keyword keyword)
    {
        keywordList.Add(keyword);
        toolTipInfos.Add(KeywordData.getData(keyword).info);
    }
    public void RemoveKeyword(Keyword keyword)
    {
        keywordList.Remove(keyword);
        toolTipInfos.Remove(KeywordData.getData(keyword).info);
    }
    public bool HasKeyword(Keyword keyword) => keywordList.Contains(keyword);
    public ReadOnlyCollection<Keyword> getKeywordList() => keywordList.AsReadOnly();
    [SerializeField] private float hoverTimeForToolTips = .5f;
    private float timePassed = 0;
    IEnumerator CheckHoverForTooltips()
    {
        while (timePassed < hoverTimeForToolTips)
        {
            timePassed += Time.deltaTime;
            if (!hovered || isBeingDragged)
            {
                timePassed = 0;
                yield break;
            }
            else
                yield return null;
        }
        timePassed = 0;
        
        // if we get here then enough time has passed so tell cardviewers to display tooltips
        foreach (CardViewer viewer in viewersDisplayingThisCard)
        {
            viewer.ShowToolTips(toolTipInfos);
        }
        yield return null;
    }
    #endregion

    #region OverrideMethods
    // ABSTRACT METHODS
    public abstract CardType getCardType();
    public abstract void Play(Tile t);
    public abstract bool CanBePlayed();

    #endregion
}
