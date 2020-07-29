using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Card : MonoBehaviour
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
    public string cardName;
    [HideInInspector] private bool hidden = true; // if true then all players can see the card
    [SerializeField] protected CardPile currentCardPile; // card pile the card is currently in. Use moveToCardPile to change
    public Player owner; // set this to readonly after done using TestCard
    public EmptyHandler onInitilization;

    // stats
    public StatsContainer Stats { get; private set; }
    public int goldCost { get { return Stats.Stats[StatType.GoldCost]; } set { Stats.setValue(StatType.GoldCost, value); } }
    public int baseGoldCost { get { return Stats.Stats[StatType.BaseGoldCost]; } set { Stats.setValue(StatType.BaseGoldCost, value); } }
    public int manaCost { get { return Stats.Stats[StatType.ManaCost]; } set { Stats.setValue(StatType.ManaCost, value); } }
    public int baseManaCost { get { return Stats.Stats[StatType.BaseManaCost]; } set { Stats.setValue(StatType.BaseManaCost, value); } }
    private ElementIdentity elementIdentity { get { return cardStatsScript.getElementIdentity(); }}

    // graphics
    [SerializeField] public CardStatsGetter cardStatsScript;
    protected SpriteRenderer[] sprites; // all sprites so card alpha can be changed all at once
    protected TextMeshPro[] tmps; // all text objects for this card
    [HideInInspector] public bool isBeingDragged = false;
    [HideInInspector] public bool positionLocked = false;
    [HideInInspector] public Vector3 positionInHand;
    public HashSet<CardViewer> viewersDisplayingThisCard;

    public List<ToolTipInfo> toolTipInfos = new List<ToolTipInfo>();
    public TransformManager TransformManager { get; private set; }

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

    // Start is called before the first frame update
    protected virtual void Start()
    {
        positionInHand = transform.position;
    }
    protected virtual void Awake()
    {
        viewersDisplayingThisCard = new HashSet<CardViewer>();

        sprites = GetComponentsInChildren<SpriteRenderer>();
        tmps = GetComponentsInChildren<TextMeshPro>();
        setSpritesToSortingLayer(SpriteLayers.CardInHandMiddle);
        effectGraphic = EffectGraphic.NewEffectGraphic(this);
        TransformManager = GetComponent<TransformManager>();
        cardStatsScript = GetComponent<CardStatsGetter>();
        //cardStatsScript.setCardCosts(this);
        Stats = GetComponent<StatsContainer>();
        Stats.addType(StatType.BaseGoldCost);
        Stats.addType(StatType.GoldCost);
        Stats.addType(StatType.BaseManaCost);
        Stats.addType(StatType.ManaCost);
    }

    // returns true if this card is the type passed to it
    public bool isType(CardType type)
    {
        return getCardType().Equals(type);
    }

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

    public abstract void initialize();

    // move card to a pile and remove it from the old one
    public void moveToCardPile(CardPile newPile, Card source)
    {
        actualMove(newPile, source);
        NetInterface.Get().syncMoveCardToPile(this, newPile, source);
    }

    public void syncCardMovement(CardPile newPile, Card source)
    {
        actualMove(newPile, source);
    }

    private void actualMove(CardPile newPile, Card source)
    {
        if (currentCardPile != null)
        {
            if (newPile == currentCardPile) // if we're already in the new pile then do nothing
                return;
            currentCardPile.removeCard(this);
        }
        CardPile previousPile = currentCardPile;
        currentCardPile = newPile;

        newPile.addCard(this);
        TriggerAddedToCardPileEffects(this, new AddedToCardPileArgs(previousPile, newPile, source));
    }

    public CardPile getCardPile() => currentCardPile;

    #region HoveringEffects
    private bool hovered = false;
    // when card becomse hovered show it in the main card preview
    private void OnMouseEnter()
    {
        if (!enabled)
            return;
        GameManager.Get().getCardViewer().setCard(this);

        // also move card up a bit
        Vector3 oldPosition = positionInHand;
        Vector3 newPosition = new Vector3(oldPosition.x, oldPosition.y + hoverOffset, oldPosition.z);
        moveTo(newPosition);

        hovered = true;
        StartCoroutine(checkHoverForTooltips());
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
            cv.clearToolTips();
    }
    private void OnDisable()
    {
        foreach (CardViewer cv in viewersDisplayingThisCard)
            cv.clearToolTips();
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
        NetInterface.Get().getLocalPlayer().heldCreature = null;

        foreach (Tile tile in legalTargetTiles)
        {
            tile.setActive(true);
        }

        setSpritesToSortingLayer(SpriteLayers.CardBeingDragged);
        // clear tooltips
        foreach (CardViewer cv in viewersDisplayingThisCard)
        {
            cv.clearToolTips();
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
            setSpriteAlpha(.5f);
        }
        else
        {
            setSpriteAlpha(1f);
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
        setSpriteAlpha(1f);
        setSpritesToSortingLayer(SpriteLayers.CardInHandMiddle);

        TransformManager.UnLock();
        // if card is above a Tile then play it
        Tile tile = getTileMouseIsOver();
        if (tile != null)
        {
            if (legalTargetTiles.Contains(tile))
            {
                //Debug.Log("Attempting to play card");
                if (canBePlayed())
                {
                    payCosts(owner);
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
                    play(tile);
                }
                else
                {
                    GameManager.Get().showToast("You can't play this card right now");
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
        setSpriteAlpha(1f);
        setSpritesToSortingLayer(SpriteLayers.CardInHandMiddle);
        moveTo(positionInHand);
        
        Board.instance.setAllTilesToDefault();
        TransformManager.UnLock();
    }
    // change the alpha for all sprites related to this card
    private void setSpriteAlpha(float value)
    {
        Color tmp = sprites[0].color;
        tmp.a = value;
        foreach (SpriteRenderer image in sprites)
        {
            image.color = tmp;
        }
    }
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
            if (currentCardPile is Hand && owner != NetInterface.Get().getLocalPlayer())
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

        //StopAllCoroutines();
        //StartCoroutine(moveToPositionCoroutine(position, smoothing));
    }
    #endregion
    #region Sprites
    public void setSpritesToSortingLayer(SpriteLayers layer)
    {
        foreach (SpriteRenderer spriteRenderer in sprites)
        {
            spriteRenderer.sortingLayerName = layer.Value;
        }

        foreach (TextMeshPro t in tmps)
        {
            t.sortingLayerID = SortingLayer.NameToID(layer.Value);
        }
    }
    public void setSpritesToSortingLayer(SpriteLayers layer, int orderInLayer)
    {
        setSpritesToSortingLayer(layer);
        foreach (SpriteRenderer spriteRenderer in sprites)
        {
            spriteRenderer.sortingOrder = orderInLayer;
        }

        foreach (TextMeshPro t in tmps)
        {
            t.sortingOrder = orderInLayer;
        }

        // move card art below icons
        cardStatsScript.getArtSprite().sortingOrder = orderInLayer - 1;
        // move background below all other sprites
        cardStatsScript.getBackgroundSprite().sortingOrder = orderInLayer - 2;
    }
    public void setSpriteMaskInteraction(SpriteMaskInteraction value)
    {
        foreach(SpriteRenderer sprite in sprites)
        {
            sprite.maskInteraction = value;
        }
    }
    public void setSpriteColor(Color color)
    {
        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.color = color;
        }
    }
    #endregion
    #region CardViewer
    public void addToCardViewer(CardViewer viewer)
    {
        viewersDisplayingThisCard.Add(viewer);
        cardStatsScript.setCardViewer(viewer);
    }
    public void removeFromCardViewer(CardViewer viewer)
    {
        if (!viewersDisplayingThisCard.Remove(viewer))
            Debug.Log("Attempting to remove card from viewer it hasn't been registered on");
    }
    #endregion

    public bool ownerCanPayCosts()
    {
        if (goldCost > owner.getGold())
            return false;
        if (manaCost > owner.getMana())
            return false;
        return true;
    }
    private void payCosts(Player player)
    {
        player.addGold(-goldCost);
        player.addMana(-manaCost);
    }

    #region GettersAndSetters
    public string getCardName()
    {
        return cardStatsScript.getCardName();
    }
    public int getTotalCost()
    {
        return manaCost + goldCost;
    }
    public string getEffectText()
    {
        return cardStatsScript.getEffectText();
    }
    public virtual void resetToBaseStats()
    {
        goldCost = baseGoldCost;
        manaCost = baseManaCost;
    }
    public virtual void resetToBaseStatsWithoutSyncing()
    {
        goldCost = baseGoldCost;
        manaCost = baseManaCost;
    }
    public ElementIdentity getElementIdentity() { return elementIdentity; }
    public void setElementIdentity(ElementIdentity eId) { cardStatsScript.setElementIdentity(eId); }
    #endregion
    #region Keyword
    private List<Keyword> keywordList = new List<Keyword>();
    public void addKeyword(Keyword keyword)
    {
        keywordList.Add(keyword);
        toolTipInfos.Add(KeywordData.getData(keyword).info);
    }
    public void removeKeyword(Keyword keyword)
    {
        keywordList.Remove(keyword);
        toolTipInfos.Remove(KeywordData.getData(keyword).info);
    }
    public bool hasKeyword(Keyword keyword)
    {
        return keywordList.Contains(keyword);
    }
    public ReadOnlyCollection<Keyword> getKeywordList()
    {
        return keywordList.AsReadOnly();
    }
    [SerializeField] private float hoverTimeForToolTips = .5f;
    private float timePassed = 0;
    IEnumerator checkHoverForTooltips()
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
            viewer.showToolTips(toolTipInfos);
        }
        yield return null;
    }
    #endregion

    private void Update()
    {
        if (isBeingDragged) // check here first to save CPU
        {
            if (Input.GetMouseButtonDown(1))
                cancelDrag();
        }
    }

    #region OverrideMethods
    // ABSTRACT METHODS
    public abstract CardType getCardType();
    public abstract void play(Tile t);
    public abstract bool canBePlayed();
    public abstract int cardId { get; }
    public abstract List<Tile> legalTargetTiles { get; }
    #endregion
}

public class CardComparator : IComparer<Card>
{
    public int Compare(Card x, Card y)
    {
        int elementCompareResult = x.getElementIdentity().CompareTo(y.getElementIdentity());
        if (elementCompareResult != 0)
            return elementCompareResult;
        int costCompareResult = x.getTotalCost().CompareTo(y.getTotalCost());
        if (costCompareResult != 0)
            return costCompareResult;
        // if cost and type are equal then go off names
        return x.getCardName().CompareTo(y.getCardName());
    }
}
public class CardComparatorByCostFirst : IComparer<Card>
{
    public int Compare(Card x, Card y)
    {
        int costCompareResult = x.getTotalCost().CompareTo(y.getTotalCost());
        if (costCompareResult != 0)
            return costCompareResult;
        int elementCompareResult = x.getElementIdentity().CompareTo(y.getElementIdentity());
        if (elementCompareResult != 0)
            return elementCompareResult;
        // if cost and type are equal then go off names
        return x.getCardName().CompareTo(y.getCardName());
    }
}
