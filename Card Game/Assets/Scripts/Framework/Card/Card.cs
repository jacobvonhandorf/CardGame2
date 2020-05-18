﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class Card : MonoBehaviour
{
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

    public enum CardKeywords
    {
        Quick // Creature that can move and attack the same turn it is played
    }

    private List<Tag> tags = new List<Tag>();
    private List<CardKeywords> keywords = new List<CardKeywords>();
    private string cardName;
    [HideInInspector] private bool hidden = true; // if true then all players can see the card
    [SerializeField] protected CardPile currentCardPile; // card pile the card is currently in. Use moveToCardPile to change
    public Player owner; // set this to readonly after done using TestCard

    [HideInInspector] public bool isBeingDragged = false;
    [HideInInspector] public bool positionLocked = false;
    [HideInInspector] public Vector3 positionInHand;

    // synced
    private int goldCost;
    private int baseGoldCost;
    private int manaCost;
    private int baseManaCost;
    private ElementIdentity elementIdentity;

    [SerializeField] protected CardStatsGetter cardStatsScript;

    protected SpriteRenderer[] sprites; // all sprites so card alpha can be changed all at once
    protected TextMeshPro[] tmps; // all text objects for this card

    public List<CardViewer> viewersDisplayingThisCard;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        positionInHand = cardStatsScript.cardRoot.position;
        tags = getTags();
    }

    private void Awake()
    {
        viewersDisplayingThisCard = new List<CardViewer>();

        try
        {
            cardStatsScript.setCardCosts(this);
        } catch (FormatException)
        {
            Debug.LogError("Error while parsing elements on card. Check that all cards have proper values set in their text fields");
        } catch (NullReferenceException)
        {
            Debug.LogError("Error while parsing elements on card. Make sure Card script is linked to StatsGetter script");
        }

        cardName = cardStatsScript.getCardName();
        sprites = getRootTransform().GetComponentsInChildren<SpriteRenderer>();
        tmps = getRootTransform().GetComponentsInChildren<TextMeshPro>();
        setSpritesToSortingLayer(SpriteLayers.CardInHandMiddle);
        elementIdentity = cardStatsScript.getElementIdentity();
    }

    /*
     * Returns true if this card has the the tag passed to this method
     */
    public bool hasTag(Tag tag)
    {
        return tags.Contains(tag);
    }

    /*
     * returns true if this card is the type passed to it
     */
    public bool isType(CardType type)
    {
        return getCardType().Equals(type);
    }

    public bool hasKeyword(CardKeywords keyword)
    {
        return keywords.Contains(keyword);
    }

    public void addKeyword(CardKeywords keyword)
    {
        keywords.Add(keyword);
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

    /*
     * Makes card not visible or affect game 
     * Used when a creature card is played so the 'card' doesn't exist anywhere
     * but will need to be put in grave if creature dies so it can't be destroyed
     */
    public void phaseOut()
    {
        gameObject.SetActive(false);
    }

    // reverses phaseOut()
    public void phaseIn()
    {
        gameObject.SetActive(true);
    }

    /*
     * Makes the card visible to both players regardless of where it is at
     */
    public void reveal()
    {
        throw new NotImplementedException();
    }

    // move card to a pile and remove it from the old one
    public void moveToCardPile(CardPile newPile)
    {
        actualMove(newPile);
        NetInterface.Get().syncMoveCardToPile(this, newPile);
    }

    public void syncCardMovement(CardPile newPile)
    {
        actualMove(newPile);
    }

    private void actualMove(CardPile newPile)
    {
        if (currentCardPile != null)
        {
            if (newPile == currentCardPile) // if we're already in the new pile then do nothing
                return;
            currentCardPile.removeCard(this);
        }
        currentCardPile = newPile;
        newPile.addCard(this);
    }

    public CardPile getCardPile()
    {
        return currentCardPile;
    }

    // when card becomse hovered show it in the main card preview
    private void OnMouseEnter()
    {
        GameManager.Get().getCardViewer().setCard(this);

        // also move card up a bit
        Vector3 oldPosition = positionInHand;
        Vector3 newPosition = new Vector3(oldPosition.x, oldPosition.y + hoverOffset, oldPosition.z);
        moveTo(newPosition, 10);
    }

    private const float hoverOffset = .7f;

    // move card back down when it is no longer being hovered
    private void OnMouseExit()
    {
        moveTo(positionInHand, 10);
    }

    // methods for drag and drop
    private Vector3 offset;
    void OnMouseDown()
    {
        if (owner.locked)
            return;
        if (positionLocked)
        {
            return;
        }

        isBeingDragged = true;
        offset = getRootTransform().position -
            Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));

        List<Tile> validTiles = getLegalTargetTiles();
        foreach (Tile tile in validTiles)
        {
            tile.setActive(true);
        }

        setSpritesToSortingLayer(SpriteLayers.CardBeingDragged);
    }


    void OnMouseDrag()
    {
        if (positionLocked || owner.locked)
        {
            Debug.Log("Card Position locked");
            return;
        }

        Vector3 newPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1.0f);
        cardStatsScript.cardRoot.position = Camera.main.ScreenToWorldPoint(newPosition) + offset;
        isBeingDragged = true;
        Tile aboveTile = getTileMouseIsOver();
        if (aboveTile != null)
        {
            setSpriteAlpha(.5f);
        }
        else
        {
            setSpriteAlpha(1f);
        }

    }

    private void OnMouseUp()
    {
        if (owner.locked)
        {
            Debug.Log("Owner is locked");
            return;
        }
        isBeingDragged = false;
        setSpriteAlpha(1f);
        setSpritesToSortingLayer(SpriteLayers.CardInHandMiddle);

        // if card is above a Tile then play it
        Tile tile = getTileMouseIsOver();
        if (tile != null)
        {
            if (getLegalTargetTiles().Contains(tile))
            {
                Debug.Log("Attempting to play card");
                if (canBePlayed())
                {
                    payCosts(owner);
                    play(tile);
                }
                else
                {
                    GameManager.Get().showToast("You can't play this card right now");
                    moveTo(positionInHand);
                }
            }
            else
            {
                moveTo(positionInHand);
            }
        }
        else
        {
            moveTo(positionInHand);
        }
        GameManager.Get().setAllTilesToNotActive();
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

    private const float smoothing = 9f; // speed at which cards snap into their place

    // methods for removing Card from scene by moving them way off screen
    // and for returning them to the scene afterwards
    public Vector3 positionOnScene;
    private bool onScene = true;
    public void removeGraphicsAndCollidersFromScene()
    {
        //if (!onScene) not sure if this is needed. Commenting it out fixed card not going to grave when killed over network
        //    return;

        positionOnScene = getRootTransform().position;
        getRootTransform().position = new Vector3(99999f, 99999f, 99999f);
        interuptMove = true;
        onScene = false;
        Debug.Log("Returning graphics to scene");
    }

    public void returnGraphicsAndCollidersToScene()
    {
        //if (onScene)
        //    return;
        if (GameManager.gameMode == GameManager.GameMode.online)
        {
            // if card is in hand and the owner is not the local player then don't do anything
            if (currentCardPile is Hand && owner != NetInterface.Get().getLocalPlayer())
            {
                return;
            }
        }
        Debug.Log("Returning graphics to scene");

        interuptMove = false;
        getRootTransform().position = positionOnScene;
        onScene = true;
    }

    public bool isOnScene()
    {
        return onScene;
    }

    // these methods and coroutines change the cards position smoothly
    Coroutine previousCoroutine = null;
    public void moveTo(Transform target)
    {
        StopAllCoroutines();
        previousCoroutine =  StartCoroutine(moveToCoroutine(target, smoothing));
    }

    public void moveTo(Vector3 position)
    {
        StopAllCoroutines();
        StartCoroutine(moveToPositionCoroutine(position, smoothing));
    }
    
    public void moveTo(Transform target, float speed)
    {
        StopAllCoroutines();
        StartCoroutine(moveToCoroutine(target, speed));
    }

    public void moveTo(Vector3 position, float speed)
    {
        StopAllCoroutines();
        StartCoroutine(moveToPositionCoroutine(position, speed));
    }

    private bool interuptMove = false; // only thing toggling this right now is removing/return graphics. Might need to refactor this if more things start affecting the card
    IEnumerator moveToCoroutine(Transform target, float speed)
    {
        if (interuptMove)
            yield break;
        Transform parent = getRootTransform();
        while (Vector3.Distance(parent.position, target.position) > 0.02f && !isBeingDragged && !interuptMove)
        {
            parent.position = Vector3.Lerp(parent.position, target.position, speed * Time.deltaTime);

            yield return null;
        }
    }

    IEnumerator moveToPositionCoroutine(Vector3 targetPosition, float speed)
    {
        if (interuptMove)
            yield break;
        Transform parent = getRootTransform();
        while (Vector3.Distance(parent.position, targetPosition) > 0.02f && !isBeingDragged && !interuptMove)
        {
            parent.position = Vector3.Lerp(parent.position, targetPosition, speed * Time.deltaTime);

            yield return null;
        }
    }

    public Transform getRootTransform()
    {
        return cardStatsScript.cardRoot;
    }

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

        // move background below all other sprites
        cardStatsScript.getBackgroundSprite().sortingOrder = orderInLayer - 1;
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

    public string getCardName()
    {
        return cardName;
    }

    public int getTotalCost()
    {
        return manaCost + goldCost;
    }

    public string getEffectText()
    {
        return cardStatsScript.getEffectText();
    }

    public int getGoldCost() { return goldCost; }
    public int getManaCost() { return manaCost; }
    public int getBaseGoldCost() { return baseGoldCost; }
    public int getBaseManaCost() { return baseManaCost; }
    public void setGoldCost(int newCost)
    {
        goldCost = newCost;
        cardStatsScript.setGoldCost(newCost, baseGoldCost);
    }
    public void setManaCost(int newCost)
    {
        manaCost = newCost;
        cardStatsScript.setManaCost(newCost, baseManaCost);
    }
    public void setBaseGoldCost(int newCost)
    {
        baseGoldCost = newCost;
        cardStatsScript.setGoldCost(goldCost, baseGoldCost);
    }
    public void setBaseManaCost(int newCost)
    {
        baseManaCost = newCost;
        cardStatsScript.setManaCost(manaCost, baseManaCost);
    }
    public virtual void resetToBaseStats()
    {
        setGoldCost(baseGoldCost);
        setManaCost(baseManaCost);
    }
    public virtual void resetToBaseStatsWithoutSyncing()
    {
        goldCost = baseGoldCost;
        manaCost = baseManaCost;
    }

    public ElementIdentity getElementIdentity() { return elementIdentity; }
    public void setElementIdentity(ElementIdentity eId) { cardStatsScript.setElementIdentity(eId); }

    // ABSTRACT METHODS
    public abstract CardType getCardType();
    public abstract List<Tile> getLegalTargetTiles();
    public abstract void play(Tile t);
    public abstract bool canBePlayed();
    public abstract int getCardId();

    // VIRTUAL METHODS
    public virtual void initialize() { }
    // triggered effects
    public virtual void onCardDrawn() { }
    public virtual void onGameStart() { }
    public virtual void onSentToGrave() { }
    public virtual void onCardAddedByEffect() { } // when card is added to a players hand by an effect
    public virtual void onAnyCreaturePlayed(Creature c) { }
    public virtual void onAnySpellCast(SpellCard s) { }

    protected virtual List<Tag> getTags()
    {
        return tags;
    }
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
