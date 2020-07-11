using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectGraphicsView : MonoBehaviour
{
    private const float DISPLAY_TIME = 2f;

    [SerializeField] private CardViewer cardViewer;
    [SerializeField] private GameObject cvGameObject;
    [SerializeField] private SpriteRenderer remainingTimeGraphic;

    private float remainingTimeDefaultWidth;

    private Queue<EffectGraphic> effectGraphics = new Queue<EffectGraphic>();

    #region Singleton
    private static EffectGraphicsView instance;
    private void Start()
    {
        instance = this;
        remainingTimeDefaultWidth = remainingTimeGraphic.size.x;
    }
    public static EffectGraphicsView Get()
    {
        return instance;
    }
    #endregion

    public void addToQueue(EffectGraphic newGraphic)
    {
        //effectGraphics.Enqueue(newGraphic);
        InformativeAnimationsQueue.instance.addAnimation(new AnimationCommand(newGraphic, this));
    }
    private class AnimationCommand : QueueableCommand
    {
        private EffectGraphic graphic;
        private EffectGraphicsView view;
        public AnimationCommand(EffectGraphic graphic, EffectGraphicsView view)
        {
            this.graphic = graphic;
            this.view = view;
        }

        public override bool isFinished => finished;
        public bool finished = false;

        public override void execute()
        {
            view.StartCoroutine(view.showGraphicCoroutine(graphic, this));
        }
    }
    #region ShowingGraphics
    private float timePassed = DISPLAY_TIME;
    private float timePassedCoeff = 1;
    private bool animationFinished = true;
    private Vector2 cachedV2 = new Vector2();
    private float newX;
    IEnumerator showGraphicCoroutine(EffectGraphic graphic, AnimationCommand cmd)
    {
        timePassed = DISPLAY_TIME;
        showGraphic(graphic);
        while (timePassed < DISPLAY_TIME)
        {
            newX = remainingTimeDefaultWidth - remainingTimeDefaultWidth * timePassed / DISPLAY_TIME;
            cachedV2.Set(newX, remainingTimeGraphic.size.y);
            remainingTimeGraphic.size = cachedV2;
            timePassed += Time.deltaTime * timePassedCoeff;
            yield return null;
        }
        cvGameObject.SetActive(false);
        cmd.finished = true;
    }
    private void showGraphic(EffectGraphic graphic)
    {
        switch (graphic)
        {
            case CardEffectGraphic g:
                showCardEffectGraphic(g);
                break;
            case TextEffectGraphic g:
                showTextEffectGraphic(g);
                break;
            default:
                throw new Exception("Unexpected graphic type");
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

    /*
    private void Update()
    {
        //doUpdate();
    }

    // broken out into seperate method so if there is a general animation manager later it can call this method
    private void doUpdate()
    {
        timePassed += Time.deltaTime * timePassedCoeff;
        if (timePassed > DISPLAY_TIME)
            showNextGraphic();

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
    */
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
