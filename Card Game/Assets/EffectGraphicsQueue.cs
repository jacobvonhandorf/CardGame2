using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectGraphicsQueue : MonoBehaviour
{
    private const float DISPLAY_TIME = 2f;

    [SerializeField] private CardViewer cardViewer;
    [SerializeField] private GameObject cvGameObject;
    [SerializeField] private SpriteRenderer remainingTimeGraphic;

    private float remainingTimeDefaultWidth;

    private Queue<EffectGraphic> effectGraphics = new Queue<EffectGraphic>();

    #region Singleton
    private static EffectGraphicsQueue instance;
    private void Start()
    {
        instance = this;
        remainingTimeDefaultWidth = remainingTimeGraphic.size.x;
    }
    public static EffectGraphicsQueue Get()
    {
        return instance;
    }
    #endregion

    public void addToQueue(EffectGraphic newGraphic)
    {
        effectGraphics.Enqueue(newGraphic);
    }

    #region ShowingGraphics
    private float timePassed = DISPLAY_TIME;
    private float timePassedCoeff = 1;
    private bool animationFinished = true;
    private void Update()
    {
        doUpdate();
    }

    private Vector2 cachedV2 = new Vector2();
    // broken out into seperate method so if there is a general animation manager later it can call this method
    public void doUpdate()
    {
        timePassed += Time.deltaTime * timePassedCoeff;
        if (timePassed > DISPLAY_TIME)
            showNextGraphic();
        // TODO also update meter below card. Be sure to use timePassed for this
        float newX = remainingTimeDefaultWidth - remainingTimeDefaultWidth * timePassed / DISPLAY_TIME;
        cachedV2.Set(newX, remainingTimeGraphic.size.y);
        remainingTimeGraphic.size = cachedV2;
    }
    private void showNextGraphic()
    {
        cardViewer.gameObject.SetActive(false);
        cvGameObject.SetActive(false);
        if (effectGraphics.Count > 0)
        {
            if (effectGraphics.Peek() is CardEffectGraphic)
            {
                showCardEffectGraphic((CardEffectGraphic)effectGraphics.Dequeue());
            }
            else if (effectGraphics.Peek() is TextEffectGraphic)
            {
                showTextEffectGraphic((TextEffectGraphic)effectGraphics.Dequeue());
            }
            else
            {
                throw new Exception("Unexpected effect graphic type");
            }
            timePassed = 0f;
        }
    }
    private void showTextEffectGraphic(TextEffectGraphic textEffectGraphic)
    {
        throw new NotImplementedException();
    }
    private void showCardEffectGraphic(CardEffectGraphic cardEffectGraphic)
    {
        cardEffectGraphic.card.addToCardViewer(cardViewer);
        cvGameObject.SetActive(true);
    }
    #endregion

    private void OnMouseEnter()
    {
        timePassedCoeff = 0;
    }
    private void OnMouseExit()
    {
        timePassedCoeff = 1;
    }
}

#region EffectGraphics
public abstract class EffectGraphic
{
    public static EffectGraphic NewEffectGraphic(Card c) { return new CardEffectGraphic(c); }
    public static EffectGraphic NewEffectGraphic(string header, string description) { return new TextEffectGraphic(header, description); }
}
public class CardEffectGraphic : EffectGraphic
{
    public Card card;
    public CardEffectGraphic(Card c)
    {
        card = c;
    }
}
public class TextEffectGraphic : EffectGraphic
{
    public string header;
    public string description;
    public TextEffectGraphic(string header, string description)
    {
        this.header = header;
        this.description = description;
    }
}
#endregion
